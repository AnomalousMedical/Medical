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

namespace Medical.Controller
{
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
        private DrawingSplitController splitController = new DrawingSplitController();
        private MedicalInterface currentMedicalInterface;

        #endregion Fields

        #region Functions

        public void Dispose()
        {
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

        public void intialize()
        {
            //Create the log.
            logListener = new LogFileListener();
            logListener.openLogFile(MedicalConfig.DocRoot + "/log.log");
            Log.Default.addLogListener(logListener);

            hiddenEmbedWindow = new DrawingWindow();
            pluginManager = new PluginManager();
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
            splitController.initialize(mainForm.DrawingHost);
            splitController.createOneWaySplit();

            //Initialize GUI
            mainForm.initialize(this);
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
            if (currentMedicalInterface != null)
            {
                currentMedicalInterface.destroy();
            }
            mainTimer.stopLoop();
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

        public DrawingSplitController SplitController
        {
            get
            {
                return splitController;
            }
        }

        public MedicalForm MedicalForm
        {
            get
            {
                return mainForm;
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
        }

        #endregion
    }
}
