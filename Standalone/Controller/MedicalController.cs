using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using OgrePlugin;
using Engine.Platform;
using Engine.Renderer;
using System.Threading;
using System.Xml;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using System.IO;
using BulletPlugin;
using MyGUIPlugin;
using Medical.GUI;
using SoundPlugin;
using Medical.Controller;
using System.Globalization;
using libRocketPlugin;
using BEPUikPlugin;

namespace Medical
{
    public delegate void LoopUpdate(Clock time);

    public sealed class MedicalController : IDisposable
    {
        //Engine
        private PluginManager pluginManager;
        private LogFileListener logListener;

        //Platform
        private SystemTimer systemTimer;
        private NativeUpdateTimer mainTimer;
        private EventManager eventManager;
        private InputHandler inputHandler;
        private EventUpdateListener eventUpdate;
        private MedicalUpdate medicalUpdate;

        //Performance
        private SystemTimer performanceMetricTimer;

        //Controller
        private MedicalSceneController medicalScene;
        private RocketGuiManager rocketGuiManager;
        private FrameClearManager frameClearManager;

        //Serialization
        private XmlSaver xmlSaver = new XmlSaver();

        //Scene
        private String currentSceneFile;
        private String currentSceneDirectory;

        public event LoopUpdate OnLoopUpdate;

        public MedicalController()
        {

        }

        /// <summary>
        /// Initialize the controller.
        /// </summary>
        /// <param name="mainForm">The form to use for input, or null to use the primary render window.</param>
        /// <param name="messagePump"></param>
        /// <param name="configureWindow"></param>
        public void initialize(StandaloneApp app, OSWindow mainForm, ConfigureDefaultWindow configureWindow)
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.LogFile);
            Log.Default.addLogListener(logListener);
            Log.ImportantInfo("Running from directory {0}", FolderFinder.ExecutableFolder);

            //Create pluginmanager
            pluginManager = new PluginManager(MedicalConfig.ConfigFile);

            //Configure the filesystem
            VirtualFileSystem archive = VirtualFileSystem.Instance;

            //Setup microcode cache load
            OgreInterface.LoadMicrocodeCacheCallback = (rs, gpuProgMan) =>
            {
                String microcodeFile = rs.Name + ".mcc";
                if (File.Exists(microcodeFile))
                {
                    using (Stream stream = File.OpenRead(microcodeFile))
                    {
                        gpuProgMan.loadMicrocodeCache(stream);
                        Log.Info("Using microcode cache {0}", microcodeFile);
                    }
                }
                return true;
            };

            //Configure plugins
            pluginManager.OnConfigureDefaultWindow = configureWindow;
            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BulletInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(MedicalController).Assembly);
            pluginManager.addPluginAssembly(typeof(MyGUIInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(RocketInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SoundPluginInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BEPUikInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SimulationPlugin).Assembly);
            pluginManager.initializePlugins();

            performanceMetricTimer = pluginManager.PlatformPlugin.createTimer();
            PerformanceMonitor.setupEnabledState(performanceMetricTimer);

            //Intialize the platform
            BulletInterface.Instance.ShapeMargin = 0.005f;
            systemTimer = pluginManager.PlatformPlugin.createTimer();

            mainTimer = new NativeUpdateTimer(systemTimer);

            if (OgreConfig.VSync)
            {
                //With vsync on its best to have no framerate cap, vsync caps for us
                mainTimer.FramerateCap = 0;
            }
            else
            {
                mainTimer.FramerateCap = MedicalConfig.EngineConfig.FPSCap;
            }

            inputHandler = pluginManager.PlatformPlugin.createInputHandler(mainForm, false, false, false);
            eventManager = new EventManager(inputHandler);
            Medical.Platform.GlobalContextEventHandler.setEventManager(eventManager);
            eventUpdate = new EventUpdateListener(eventManager);
            mainTimer.addUpdateListener(eventUpdate);
            pluginManager.setPlatformInfo(mainTimer, eventManager);
            medicalUpdate = new MedicalUpdate(this);
            mainTimer.addUpdateListener(medicalUpdate);

            //Initialize controllers
            medicalScene = new MedicalSceneController(pluginManager);
            rocketGuiManager = new RocketGuiManager();
            rocketGuiManager.initialize(pluginManager, eventManager, mainTimer);
            frameClearManager = new FrameClearManager(OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget);

            SoundConfig.initialize(MedicalConfig.ConfigFile);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            MedicalConfig.save();

            if (frameClearManager != null)
            {
                frameClearManager.Dispose();
            }
            if (rocketGuiManager != null)
            {
                rocketGuiManager.Dispose();
            }
            if (medicalScene != null)
            {
                medicalScene.destroyScene();
            }
            if (eventManager != null)
            {
                eventManager.Dispose();
            }
            if (inputHandler != null)
            {
                pluginManager.PlatformPlugin.destroyInputHandler(inputHandler);
            }
            if (systemTimer != null)
            {
                pluginManager.PlatformPlugin.destroyTimer(systemTimer);
            }
            if (performanceMetricTimer != null)
            {
                PerformanceMonitor.destroyEnabledState();
                pluginManager.PlatformPlugin.destroyTimer(performanceMetricTimer);
            }
            if (pluginManager != null)
            {
                //This is the main engine plugin manager, it should be last unless subsystems need to be shutdown before any additional disposing
                pluginManager.Dispose();
            }

            Log.Info("Medical Controller Shutdown");
            logListener.closeLogFile();
        }

        public void saveCrashLog()
        {
            if (logListener != null)
            {
                DateTime now = DateTime.Now;
                String crashFile = String.Format(CultureInfo.InvariantCulture, "{0}/log {1}-{2}-{3} {4}.{5}.{6}.log", MedicalConfig.CrashLogDirectory, now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second);
                logListener.saveCrashLog(crashFile);
            }
        }

        /// <summary>
        /// Stop the loop and begin the process of shutting down the program.
        /// </summary>
        public void shutdown()
        {
            mainTimer.stopLoop();
        }

        public void destroyInputHandler()
        {
            eventManager.destroyInputObjects();
            pluginManager.PlatformPlugin.destroyInputHandler(inputHandler);
        }

        public void recreateInputHandler(OSWindow window)
        {
            inputHandler = new NativeInputHandler(window as NativeOSWindow);
            eventManager.changeInputHandler(inputHandler);
        }

        /// <summary>
        /// Attempt to open the given scene file. Will return true if the scene was loaded correctly.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>True if the scene was loaded, false on an error.</returns>
        public IEnumerable<SceneBuildStatus> openScene(String filename)
        {
            medicalScene.destroyScene();
            VirtualFileSystem sceneArchive = VirtualFileSystem.Instance;
            if (sceneArchive.exists(filename))
            {
                currentSceneFile = VirtualFileSystem.GetFileName(filename);
                currentSceneDirectory = VirtualFileSystem.GetDirectoryName(filename);
                using (Stream file = sceneArchive.openStream(filename, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                {
                    XmlTextReader textReader = null;
                    ScenePackage scenePackage = null;
                    try
                    {
                        yield return new SceneBuildStatus()
                        {
                            Message = "Loading Scene File"
                        };
                        textReader = new XmlTextReader(file);
                        scenePackage = xmlSaver.restoreObject(textReader) as ScenePackage;
                    }
                    finally
                    {
                        if (textReader != null)
                        {
                            textReader.Close();
                        }
                    }
                    if (scenePackage != null)
                    {
                        foreach (var status in medicalScene.loadScene(scenePackage, SceneBuildOptions.SingleUseDefinitions))
                        {
                            yield return status;
                        }
                    }
                }
            }
            else
            {
                Log.Error("Could not load scene {0}.", filename);
            }
        }

        public void addSimObject(SimObjectBase simObject)
        {
            medicalScene.addSimObject(simObject);
        }

        internal void _sendUpdate(Clock clock)
        {
            if (OnLoopUpdate != null)
            {
                OnLoopUpdate.Invoke(clock);
            }
        }

        public EventManager EventManager
        {
            get
            {
                return eventManager;
            }
        }

        public NativeUpdateTimer MainTimer
        {
            get
            {
                return mainTimer;
            }
        }

        public SimScene CurrentScene
        {
            get
            {
                return medicalScene.CurrentScene;
            }
        }

        public IEnumerable<SimObjectBase> SimObjects
        {
            get
            {
                return medicalScene.SimObjects;
            }
        }

        public SimObject getSimObject(String name)
        {
            return medicalScene.getSimObject(name);
        }

        public String CurrentSceneFile
        {
            get
            {
                return currentSceneFile;
            }
        }

        public String CurrentSceneDirectory
        {
            get
            {
                return currentSceneDirectory;
            }
        }

        public PluginManager PluginManager
        {
            get
            {
                return pluginManager;
            }
        }
    }
}
