using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Medical.GUI;
using OgrePlugin;
using Engine.Platform;
using Engine.Renderer;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Xml;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Engine.Resources;
using Medical.Properties;
using System.IO;
using BulletPlugin;

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
        private UpdateTimer mainTimer;
        private EventManager eventManager;
        private InputHandler inputHandler;
        private EventUpdateListener eventUpdate;
        private FixedMedicalUpdate fixedUpdate;
        private FullSpeedMedicalUpdate fullSpeedUpdate;

        //GUI
        private DrawingWindow hiddenEmbedWindow;

        //Controller
        private MedicalSceneController medicalScene;

        //Serialization
        private XmlSaver xmlSaver = new XmlSaver();

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

        public void intialize(OSWindow mainForm)
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.DocRoot + "/log.log");
            Log.Default.addLogListener(logListener);

            Resource.ResourceRoot = "s:\\export";
            Log.Default.sendMessage("Resource root is {0}.", LogLevel.ImportantInfo, "Medical", Path.GetFullPath(Resource.ResourceRoot));

            hiddenEmbedWindow = new DrawingWindow();
            pluginManager = new PluginManager(MedicalConfig.ConfigFile);
            pluginManager.OnConfigureDefaultWindow = createWindow;
            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BulletInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(Win32PlatformPlugin).Assembly);
            pluginManager.initializePlugins();
            pluginManager.RendererPlugin.PrimaryWindow.setEnabled(false);

            //Intialize the platform
            BulletInterface.Instance.ShapeMargin = 0.0f;
            systemTimer = pluginManager.PlatformPlugin.createTimer();
            mainTimer = new UpdateTimer(systemTimer, new WindowsFormsUpdate());
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
        public void Dispose()
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
            if (hiddenEmbedWindow != null)
            {
                hiddenEmbedWindow.Dispose();
            }

            MedicalConfig.save();
            logListener.closeLogFile();
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
            medicalScene.destroyScene();
            mainTimer.stopLoop();
        }

        /// <summary>
        /// Attempt to open the given scene file. Will return true if the scene was loaded correctly.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <returns>True if the scene was loaded, false on an error.</returns>
        public bool openScene(Stream file)
        {
            medicalScene.destroyScene();
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

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out DefaultWindowInfo defaultWindow)
        {
            defaultWindow = new DefaultWindowInfo(hiddenEmbedWindow);
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

        public UpdateTimer MainTimer
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

        #endregion Properties
    }
}
