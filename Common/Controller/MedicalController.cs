using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;
using Medical.GUI;
using OgrePlugin;
using PhysXPlugin;
using Engine.Platform;
using Engine.Renderer;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Xml;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Engine.Resources;

namespace Medical
{
    public delegate void LoopUpdate(Clock time);

    public class MedicalController : IDisposable, UpdateListener
    {
        #region Fields

        //Engine
        private PluginManager pluginManager;
        private LogFileListener logListener;

        //Platform
        private UpdateTimer mainTimer;
        private EventManager eventManager;
        private InputHandler inputHandler;
        private EventUpdateListener eventUpdate;

        //GUI
        private DrawingWindow hiddenEmbedWindow;
        private MedicalForm mainForm;

        //Controller
        private DrawingWindowController drawingWindowController = new DrawingWindowController();
        private MedicalInterface currentMedicalInterface;
        private MedicalSceneController medicalScene;
        private CommonController commonController = new CommonController();

        //Serialization
        private XmlSaver xmlSaver = new XmlSaver();

        #endregion Fields

        #region Events

        public event LoopUpdate LoopUpdate;

        #endregion Events

        #region Functions

        public void intialize()
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.DocRoot + "/log.log");
            Log.Default.addLogListener(logListener);

            Resource.ResourceRoot = null;

            hiddenEmbedWindow = new DrawingWindow();
            pluginManager = new PluginManager(MedicalConfig.ConfigFile);
            pluginManager.OnConfigureDefaultWindow = createWindow;
            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(PhysXInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(Win32PlatformPlugin).Assembly);
            pluginManager.initializePlugins();
            pluginManager.RendererPlugin.PrimaryWindow.setEnabled(false);

            //Create the GUI
            mainForm = new MedicalForm();

            //Intialize the platform
            mainTimer = pluginManager.PlatformPlugin.createTimer();
            inputHandler = pluginManager.PlatformPlugin.createInputHandler(mainForm, false, false, false);
            eventManager = new EventManager(inputHandler);
            eventUpdate = new EventUpdateListener(eventManager);
            mainTimer.addFixedUpdateListener(eventUpdate);
            mainTimer.addFullSpeedUpdateListener(this);
            pluginManager.setPlatformInfo(mainTimer, eventManager);

            //Initialize controllers
            drawingWindowController.initialize(this, eventManager, pluginManager.RendererPlugin, MedicalConfig.ConfigFile);
            medicalScene = new MedicalSceneController(pluginManager);
            medicalScene.OnSceneLoaded += new MedicalSceneControllerEvent(medicalScene_OnSceneLoaded);
            medicalScene.OnSceneUnloading += new MedicalSceneControllerEvent(medicalScene_OnSceneUnloading);
            commonController.initialize(this);

            //Initialize GUI
            mainForm.initialize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (currentMedicalInterface != null)
            {
                currentMedicalInterface.destroy();
            }
            commonController.destroy();
            if (eventManager != null)
            {
                eventManager.Dispose();
            }
            if (inputHandler != null)
            {
                pluginManager.PlatformPlugin.destroyInputHandler(inputHandler);
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
            mainForm.Show();
            mainTimer.startLoop();
        }

        /// <summary>
        /// Stop the loop and begin the process of shutting down the program.
        /// </summary>
        public void shutdown()
        {
            mainTimer.stopLoop();
            mainForm.saveWindows(MedicalConfig.WindowsFile);
        }

        /// <summary>
        /// Set the MedicalInterface for the specific area being displayed. This
        /// will create the appropriate controls.
        /// </summary>
        /// <param name="medInterface">The medical interface to set.</param>
        public void setMedicalInterface(MedicalInterface medInterface)
        {
            currentMedicalInterface = medInterface;
            currentMedicalInterface.initialize(this);

            mainForm.SuspendLayout();
            if (!mainForm.restoreWindows(MedicalConfig.WindowsFile, getDockContent))
            {
                drawingWindowController.createOneWaySplit();
            }
            mainForm.ResumeLayout();
        }

        public void showDockContent(DockContent content)
        {
            mainForm.showDockContent(content);
        }

        public void hideDockContent(DockContent content)
        {
            mainForm.hideDockContent(content);
        }

        public void addToolStrip(ToolStrip toolStrip)
        {
            mainForm.addToolStrip(toolStrip);
        }

        public void removeToolStrip(ToolStrip toolStrip)
        {
            mainForm.removeToolStrip(toolStrip);
        }

        public void createNewScene()
        {

        }

        public void openScene(String filename)
        {
            XmlTextReader textReader = new XmlTextReader(filename);
            ScenePackage scenePackage = xmlSaver.restoreObject(textReader) as ScenePackage;
            if (scenePackage != null)
            {
                medicalScene.destroyScene();
                medicalScene.loadScene(scenePackage);
            }
            else
            {
                MessageBox.Show(mainForm, String.Format("Could not load scene from {0}.", filename), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            textReader.Close();
        }

        public void saveScene(String filename)
        {

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

        /// <summary>
        /// Callback for when the scene is unloading.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="scene"></param>
        void medicalScene_OnSceneUnloading(MedicalSceneController controller, Engine.ObjectManagement.SimScene scene)
        {
            commonController.sceneUnloading();
            if (currentMedicalInterface != null)
            {
                currentMedicalInterface.sceneUnloading();
            }
            drawingWindowController.destroyCameras();
        }

        /// <summary>
        /// Callback for when the scene is loaded.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="scene"></param>
        void medicalScene_OnSceneLoaded(MedicalSceneController controller, Engine.ObjectManagement.SimScene scene)
        {
            drawingWindowController.createCameras(mainTimer, scene);
            commonController.sceneChanged();
            if (currentMedicalInterface != null)
            {
                currentMedicalInterface.sceneChanged();
            }
        }

        /// <summary>
        /// Restore function for restoring the window layout.
        /// </summary>
        /// <param name="persistString">The string describing the window.</param>
        /// <returns>The IDockContent associated with the given string.</returns>
        private IDockContent getDockContent(String persistString)
        {
            IDockContent content = commonController.getDockContent(persistString);
            if (content != null)
            {
                return content;
            }
            if (currentMedicalInterface != null)
            {
                content = currentMedicalInterface.getDockContent(persistString);
                if (content != null)
                {
                    return content;
                }
            }

            String name;
            Vector3 translation;
            Vector3 lookAt;
            if (DrawingWindowHost.RestoreFromString(persistString, out name, out translation, out lookAt))
            {
                return drawingWindowController.createDrawingWindowHost(name, translation, lookAt);
            }
            return null;
        }

        #endregion Functions

        #region Properties

        public DrawingWindowController DrawingWindowController
        {
            get
            {
                return drawingWindowController;
            }
        }

        #endregion Properties

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            Application.DoEvents();
            if (LoopUpdate != null)
            {
                LoopUpdate.Invoke(clock);
            }
        }

        #endregion
    }
}
