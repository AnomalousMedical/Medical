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
using System.Drawing;
using MyGUIPlugin;
using Medical.GUI;
using SoundPlugin;

namespace Medical
{
    public delegate void LoopUpdate(Clock time);

    public class MedicalController : IDisposable
    {
        #region Fields

        //Engine
        private PluginManager pluginManager;
        private LogFileListener logListener;

        //Platform
        private SystemTimer systemTimer;
        private WxUpdateTimer mainTimer;
        private EventManager eventManager;
        private InputHandler inputHandler;
        private EventUpdateListener eventUpdate;
        private FixedMedicalUpdate fixedUpdate;
        private FullSpeedMedicalUpdate fullSpeedUpdate;

        //Controller
        private MedicalSceneController medicalScene;

        //Serialization
        private XmlSaver xmlSaver = new XmlSaver();

        //Scene
        private String currentSceneFile;
        private String currentSceneDirectory;

        #endregion Fields

        #region Events

        public event LoopUpdate FullSpeedLoopUpdate;
        public event LoopUpdate FixedLoopUpdate;

        #endregion Events

        #region Constructors

        public MedicalController()
        {

        }

        #endregion Constructors

        #region Functions

        /// <summary>
        /// Initialize the controller.
        /// </summary>
        /// <param name="mainForm">The form to use for input, or null to use the primary render window.</param>
        /// <param name="messagePump"></param>
        /// <param name="configureWindow"></param>
        public void initialize(OSWindow mainForm, ConfigureDefaultWindow configureWindow)
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.DocRoot + "/log.log");
            Log.Default.addLogListener(logListener);
#if ONLY_LOG_ERRORS
            Log.Default.setActiveMessageTypes(LogLevel.Error);
#endif

            //Config plugins
            MyGUIInterface.LogFile = MedicalConfig.DocRoot + "/MyGUI.log";

            //Create pluginmanager
            pluginManager = new PluginManager(MedicalConfig.ConfigFile);

            //Configure the filesystem
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            //Add primary archive
            archive.addArchive(MedicalConfig.PrimaryArchive);

            //Add any patch archives
            int i = 0;
            String patchArchive = MedicalConfig.getPatchArchiveName(i);
            while (File.Exists(patchArchive))
            {
                archive.addArchive(patchArchive);
                ++i;
                patchArchive = MedicalConfig.getPatchArchiveName(i);
            }

            //Add working archive
#if ALLOW_OVERRIDE
            archive.addArchive(MedicalConfig.WorkingResourceDirectory);
#endif

            //Configure plugins
            pluginManager.OnConfigureDefaultWindow = configureWindow;
            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BulletInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(MedicalController).Assembly);
            pluginManager.addPluginAssembly(typeof(MyGUIInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SoundPluginInterface).Assembly);
            pluginManager.initializePlugins();
            if(mainForm == null)
            {
                mainForm = pluginManager.RendererPlugin.PrimaryWindow.Handle;
            }

            //Intialize the platform
            BulletInterface.Instance.ShapeMargin = 0.005f;
            systemTimer = pluginManager.PlatformPlugin.createTimer();

            WxUpdateTimer win32Timer = new WxUpdateTimer(systemTimer);
            mainTimer = win32Timer;
            
            mainTimer.FramerateCap = MedicalConfig.EngineConfig.MaxFPS;
            inputHandler = pluginManager.PlatformPlugin.createInputHandler(mainForm, false, false, false);
            eventManager = new EventManager(inputHandler);
            eventUpdate = new EventUpdateListener(eventManager);
            mainTimer.addFixedUpdateListener(eventUpdate);
            pluginManager.setPlatformInfo(mainTimer, eventManager);
            fixedUpdate = new FixedMedicalUpdate(this);
            mainTimer.addFixedUpdateListener(fixedUpdate);
            fullSpeedUpdate = new FullSpeedMedicalUpdate(this);
            mainTimer.addFullSpeedUpdateListener(fullSpeedUpdate);

            //Initialize controllers
            medicalScene = new MedicalSceneController(pluginManager);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
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
            if (pluginManager != null)
            {
                pluginManager.Dispose();
            }

            MedicalConfig.save();
            logListener.closeLogFile();
        }

        public void saveCrashLog()
        {
            if (logListener != null)
            {
                DateTime now = DateTime.Now;
                String crashFile = String.Format("{0}/CrashLogs/log {1}-{2}-{3} {4}.{5}.{6}.log", MedicalConfig.DocRoot, now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second);
                logListener.saveCrashLog(crashFile);
            }
        }

        /// <summary>
        /// Show the form to the user and start the loop.
        /// </summary>
        public void start()
        {
            mainTimer.startLoop();
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
            inputHandler = new WxInputHandler(window as WxOSWindow);
            eventManager.changeInputHandler(inputHandler);
        }

        /// <summary>
        /// Attempt to open the given scene file. Will return true if the scene was loaded correctly.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>True if the scene was loaded, false on an error.</returns>
        public bool openScene(String filename)
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
                        medicalScene.loadScene(scenePackage);
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                Log.Error("Could not load scene {0}.", filename);
                return false;
            }
        }

        public void saveScene(String filename)
        {
            ScenePackage package = medicalScene.saveSceneToPackage();
            XmlTextWriter textWriter = null;
            try
            {
                textWriter = new XmlTextWriter(filename, Encoding.Default);
                textWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(package, textWriter);
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
        }

        public void addSimObject(SimObjectBase simObject)
        {
            medicalScene.addSimObject(simObject);
        }

        internal void _sendFullSpeedUpdate(Clock clock)
        {
            if (FullSpeedLoopUpdate != null)
            {
                FullSpeedLoopUpdate.Invoke(clock);
            }
        }

        internal void _sendFixedUpdate(Clock clock)
        {
            if (FixedLoopUpdate != null)
            {
                FixedLoopUpdate.Invoke(clock);
            }
        }

        #endregion Functions

        #region Properties

        public EventManager EventManager
        {
            get
            {
                return eventManager;
            }
        }

        public WxUpdateTimer MainTimer
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

        #endregion Properties
    }
}
