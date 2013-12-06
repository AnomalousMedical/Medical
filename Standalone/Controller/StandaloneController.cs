using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Renderer;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;
using Logging;
using Medical;
using OgrePlugin;
using OgreWrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using MyGUIPlugin;
using Medical.GUI;
using Medical.Controller;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.Editor;

namespace Medical
{
    public delegate void SceneEvent(SimScene scene);

    public class StandaloneController : IDisposable
    {
        //Events
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;
        public event SceneEvent BeforeSceneLoadProperties;

        //Controller
        private MedicalController medicalController;
        private WindowListener windowListener;
        private MedicalStateController medicalStateController;
        private TemporaryStateBlender tempStateBlender;
        private MovementSequenceController movementSequenceController;
        private SimObjectMover teethMover;
        private ImageRenderer imageRenderer;
        private TimelineController timelineController;
        private AnomalousMvcCore mvcCore;
        private MyGUIViewHostFactory viewHostFactory;
        private PropFactory propFactory;
        private ExamController examController;
        private TaskController taskController;
        private SaveableClipboard clipboard;
        private AnatomyController anatomyController;
        private DownloadController downloadController;
        private PatientDataController patientDataController;
        private IdleHandler idleHandler;
        private BehaviorErrorManager behaviorErrorManager;

        //GUI
        private GUIManager guiManager;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private BackgroundScene background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private NotificationGUIManager notificationManager;

        //Platform
        private MainWindow mainWindow;
        private StandaloneApp app;
        private AtlasPluginManager atlasPluginManager;
		private bool shuttingDown = false;

        //Touch
        private TouchController touchController;

        public StandaloneController(StandaloneApp app)
        {
            this.app = app;

            MedicalConfig config = new MedicalConfig(FolderFinder.AnomalousMedicalUserRoot, FolderFinder.AnomalousMedicalAllUserRoot);
            CertificateStoreManager.Initialize(MedicalConfig.CertificateStoreFile, MedicalConfig.CertificateStoreUrl);
            guiManager = new GUIManager(this);
            guiManager.MainGUIShown += guiManager_MainGUIShown;
            guiManager.MainGUIHidden += guiManager_MainGUIHidden;

            MyGUIInterface.OSTheme = PlatformConfig.ThemeFile;

            mainWindow = new MainWindow(app.WindowTitle);

            //Setup DPI
            float pixelScale = mainWindow.WindowScaling;

#if ALLOW_OVERRIDE
            if (MedicalConfig.PixelScaleOverride > 0.5f)
            {
                pixelScale = MedicalConfig.PixelScaleOverride;
            }
#endif

            switch (MedicalConfig.ExtraScaling)
            {
                case UIExtraScale.Smaller:
                    pixelScale -= .15f;
                    break;
                case UIExtraScale.Larger:
                    pixelScale += .25f;
                    break;
            }

            ScaleHelper._setScaleFactor(pixelScale);

            //Initialize engine
            medicalController = new MedicalController();
            medicalController.initialize(app, mainWindow, createWindow);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);
            mainWindow.setPointerManager(PointerManager.Instance);
            idleHandler = new IdleHandler(medicalController.MainTimer.OnIdle);

            PointerManager.Instance.Visible = false;

            behaviorErrorManager = new BehaviorErrorManager();

            windowListener = new WindowListener(this);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);
            OgreInterface.Instance.OgrePrimaryWindow.OgreRenderWindow.DeactivateOnFocusChange = false;
        }

        public void Dispose()
        {
			IDisposableUtil.DisposeIfNotNull(mvcCore);
			IDisposableUtil.DisposeIfNotNull(downloadController);
            if (DocumentController != null)
            {
                DocumentController.saveRecentDocuments();
            }
			IDisposableUtil.DisposeIfNotNull(touchController);
			IDisposableUtil.DisposeIfNotNull(atlasPluginManager);
            IDisposableUtil.DisposeIfNotNull(notificationManager);
            if (guiManager != null)
            {
                ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
                guiManager.saveUI(configFile);
                guiManager.Dispose();
                guiManager = null;
                configFile.writeConfigFile();
            }
			IDisposableUtil.DisposeIfNotNull(watermark);
			IDisposableUtil.DisposeIfNotNull(measurementGrid);
			IDisposableUtil.DisposeIfNotNull(anatomyController);
			IDisposableUtil.DisposeIfNotNull(medicalStateController);
			IDisposableUtil.DisposeIfNotNull(sceneViewController);
			IDisposableUtil.DisposeIfNotNull(background);
			IDisposableUtil.DisposeIfNotNull(mdiLayout);
			IDisposableUtil.DisposeIfNotNull(medicalController);
			IDisposableUtil.DisposeIfNotNull(mainWindow);

            //Stop any waiting background threads last.
            ThreadManager.cancelAll();
        }

        /// <summary>
        /// This function will try to load the certificate store again, It is only intended to be called during startup
        /// when the user is asked to connect to the internet, please do not call it any other time.
        /// </summary>
        public void retryLoadingCertificateStore()
        {
            CertificateStoreManager.Initialize(MedicalConfig.CertificateStoreFile, MedicalConfig.CertificateStoreUrl);
        }

        public void addWorkingArchive()
        {
#if ALLOW_OVERRIDE
            //Add working archive
            VirtualFileSystem.Instance.addArchive(MedicalConfig.WorkingResourceDirectory);
#endif
        }

        public void initializeControllers(BackgroundScene background)
        {
            //Background
            this.background = background;

            atlasPluginManager = new AtlasPluginManager(this);
            atlasPluginManager.PluginLoadError += new Medical.AtlasPluginManager.PluginMessageDelegate(atlasPluginManager_PluginLoadError);
            atlasPluginManager.manageInstalledPlugins();

            clipboard = new SaveableClipboard();

            //Documents
            DocumentController = new DocumentController();

            //Setup MyGUI listeners
            MyGUIInterface myGUI = MyGUIInterface.Instance;
            myGUI.RenderEnded += new EventHandler(myGUI_RenderEnded);
            myGUI.RenderStarted += new EventHandler(myGUI_RenderStarted);

            //MDI Layout
            mdiLayout = new MDILayoutManager();
            medicalController.MainTimer.addFixedUpdateListener(new MDIUpdate(medicalController.EventManager, mdiLayout));

            //SceneView
            sceneViewController = new SceneViewController(mdiLayout, medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow, myGUI.OgrePlatform.getRenderManager(), background);

            //Watermark
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

            //Measurement grid
            measurementGrid = new MeasurementGrid("MeasurementGrid", medicalController, sceneViewController);
            SceneUnloading += measurementGrid.sceneUnloading;
            SceneLoaded += measurementGrid.sceneLoaded;

            //Image Renderer
            imageRenderer = new ImageRenderer(medicalController, sceneViewController, idleHandler);
            imageRenderer.Watermark = watermark;
            imageRenderer.Background = background;
            imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;

            //Anatomy Controller
            anatomyController = new AnatomyController(imageRenderer, ScaleHelper.Scaled(50), ScaleHelper.Scaled(50));

            //Medical states
            medicalStateController = new MedicalStateController(imageRenderer, medicalController);
            SceneLoaded += medicalStateController.sceneLoaded;
            SceneUnloading += medicalStateController.sceneUnloading;
            tempStateBlender = new TemporaryStateBlender(medicalController.MainTimer, medicalStateController);

            //Movement sequences
            movementSequenceController = new MovementSequenceController(medicalController);
            this.SceneLoaded += movementSequenceController.sceneLoaded;

            //Teeth mover
            teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
            this.SceneLoaded += teethMover.sceneLoaded;
            this.SceneUnloading += teethMover.sceneUnloading;
            TeethController.TeethMover = teethMover;
            medicalController.FixedLoopUpdate += teethMover.update;
            imageRenderer.ImageRenderStarted += TeethController.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += TeethController.ScreenshotRenderCompleted;

            //Download
            downloadController = new DownloadController(app.LicenseManager, AtlasPluginManager);

            //Props
            propFactory = new PropFactory(this);
            Arrow.createPropDefinition(propFactory);
            Ruler.createPropDefinition(propFactory);
            PointingHand.createPropDefinition(propFactory);
            Doppler.createPropDefinition(propFactory);
            Syringe.createPropDefinition(propFactory);
            JVAProp.createPropDefinition(propFactory);
            Mustache.createPropDefinition(propFactory);
            CircularHighlight.createPropDefinition(propFactory);
            Plane.createPropDefinition(propFactory);
            PoseableHand.createPropDefinition(propFactory);
            BiteStick.createPropDefinition(propFactory);
            RangeOfMotionScale.createPropDefinition(propFactory);
            Pen.createPropDefinition(propFactory);
            Caliper.createPropDefinition(propFactory);
            SplintDefiner.createPropDefinition(propFactory);
            DentalFloss.createPropDefinition(propFactory);

            //Timeline
            timelineController = new TimelineController(this);

            viewHostFactory = new MyGUIViewHostFactory(mdiLayout);
            mvcCore = new AnomalousMvcCore(this, viewHostFactory);

            //Exams
            examController = new ExamController();

            //Patient data
            patientDataController = new PatientDataController(this);

            //Tasks
            taskController = new TaskController();

            //MultiTouch
            if (MedicalConfig.EnableMultitouch && MultiTouch.IsAvailable)
            {
                touchController = new TouchController(mainWindow, medicalController.MainTimer, sceneViewController);
            }
            else
            {
                Log.Info("MultiTouch not available");
            }

            //Coroutine
            Coroutine.SetTimerFixed(medicalController.MainTimer);
        }

        public void createGUI()
        {
            //GUI
            guiManager.createGUI(mdiLayout);
            
            notificationManager = new NotificationGUIManager(this);
            guiManager.giveGUIsToTimelineController(timelineController);
        }

        void guiManager_MainGUIHidden()
        {
            notificationManager.hideAllNotifications();
        }

        void guiManager_MainGUIShown()
        {
            notificationManager.reshowAllNotifications();
        }

        public void initializePlugins()
        {
            //Wizards
            ResourceManager.Instance.load("Medical.Resources.WizardImagesets.xml");

            ResourceManager.Instance.load("Medical.Resources.StandaloneIcons.xml");
            ResourceManager.Instance.load("Medical.Resources.CommonToolstrip.xml");

            atlasPluginManager.initialzePlugins();
            ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
            configFile.loadConfigFile();
            guiManager.loadSavedUI(configFile);

            //Load recent documents here, this way the document handlers are all loaded
            DocumentController.loadRecentDocuments(MedicalConfig.RecentDocsFile);

            guiManager.layout();
        }

        public void onIdle()
        {
            idleHandler.onIdle();
        }

        public void openHelpPage()
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.HelpURL);
        }

        public void exit()
        {
			if(!shuttingDown)
			{
				shuttingDown = true;
	            if (PlatformConfig.CloseMainWindowOnShutdown)
	            {
	                mainWindow.close();
	            }
            	app.exit();
			}
        }

        public void restartWithWarning()
        {
            restartWithWarning(null, false);
        }

        public void restartWithWarning(String noDownloadsMessage, bool autoStartUpdate)
        {
            String message = noDownloadsMessage;
            if (downloadController.Downloading)
            {
                message = "You are currently downloading some files. If you restart now you will lose your download progress.\nIt is reccomended to click no and let the downloads finish.\nAre you sure you want to restart Anomalous Medical?";
            }
            if (message == null)
            {
                restart();
            }
            else
            {
                MessageBox.show(message, "Restart", MessageBoxStyle.IconInfo | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        UpdateController.AutoStartUpdate = autoStartUpdate;
                        restart();
                    }
                });
            }
        }

        public void restart()
        {
            if (!shuttingDown)
            {
                shuttingDown = true;
                if (PlatformConfig.CloseMainWindowOnShutdown)
                {
                    mainWindow.close();
                }
                app.restart();
            }
        }

        /// <summary>
        /// Opens a scene as a "new" scene by opening the given file and clearing the states.
        /// </summary>
        /// <param name="filename"></param>
        public bool openNewScene(String filename)
        {
            medicalStateController.clearStates();
            bool success = changeScene(filename);
            examController.clear();
            patientDataController.clearData();
            return success;
        }

        public void sceneRevealed()
        {
            atlasPluginManager.sceneRevealed();
        }

        public void setWatermarkText(String text)
        {
            ((SideLogoWatermark)watermark).addText(text);
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public TemporaryStateBlender TemporaryStateBlender
        {
            get
            {
                return tempStateBlender;
            }
        }

        public SceneViewController SceneViewController
        {
            get
            {
                return sceneViewController;
            }
        }

        public MedicalStateController MedicalStateController
        {
            get
            {
                return medicalStateController;
            }
        }

        public MovementSequenceController MovementSequenceController
        {
            get
            {
                return movementSequenceController;
            }
        }

        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
        }

        public MeasurementGrid MeasurementGrid
        {
            get
            {
                return measurementGrid;
            }
        }

        public MDILayoutManager MDILayout
        {
            get
            {
                return mdiLayout;
            }
        }

        public PropFactory PropFactory
        {
            get
            {
                return propFactory;
            }
        }

        public AtlasPluginManager AtlasPluginManager
        {
            get
            {
                return atlasPluginManager;
            }
        }

        public GUIManager GUIManager
        {
            get
            {
                return guiManager;
            }
        }

        public StandaloneApp App
        {
            get
            {
                return app;
            }
        }

        public TimelineController TimelineController
        {
            get
            {
                return timelineController;
            }
        }

        public ExamController ExamController
        {
            get
            {
                return examController;
            }
        }

        public TaskController TaskController
        {
            get
            {
                return taskController;
            }
        }

        public SaveableClipboard Clipboard
        {
            get
            {
                return clipboard;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return anatomyController;
            }
        }

        public DocumentController DocumentController { get; private set; }

        public DownloadController DownloadController
        {
            get
            {
                return downloadController;
            }
        }

        public PatientDataController PatientDataController
        {
            get
            {
                return patientDataController;
            }
        }

        public AnomalousMvcCore MvcCore
        {
            get
            {
                return mvcCore;
            }
        }

        public MyGUIViewHostFactory ViewHostFactory
        {
            get
            {
                return viewHostFactory;
            }
        }

        public IdleHandler IdleHandler
        {
            get
            {
                return idleHandler;
            }
        }

        public NotificationGUIManager NotificationManager
        {
            get
            {
                return notificationManager;
            }
        }

        /// <summary>
        /// This controller enables sharing of plugins. If it does not exist the program cannot share plugins.
        /// Do not create this anywhere except in StoreManagerPlugin.
        /// </summary>
        public SharePluginController SharePluginController { get; set; }

        public void recreateMainWindow()
        {
            //sceneViewController.destroyCameras();
            //MyGUIInterface.Instance.destroyViewport();
            //medicalController.destroyInputHandler();

            //RendererWindow window = OgreInterface.Instance.recreatePrimaryWindow();

            //medicalController.recreateInputHandler(window.Handle);
            //MyGUIInterface.Instance.recreateViewport(window);
            //sceneViewController.changeRendererWindow(window);
            //sceneViewController.createCameras(medicalController.CurrentScene);
            //window.Handle.addListener(windowListener);
            //basicGUI.windowChanged(window.Handle);

            MessageBox.show("You will need to restart the program to apply your settings.\nWould you like to restart now?", "Apply Changes?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, displayParameterChangeCallback);
        }

        public void saveCrashLog()
        {
            if (medicalController != null)
            {
                medicalController.saveCrashLog();
            }
        }

        void displayParameterChangeCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                this.restart();
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        internal bool changeScene(String file)
        {
            bool success = false;
            sceneViewController.resetAllCameraPositions();
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            anatomyController.sceneUnloading();
            sceneViewController.destroyCameras();
            behaviorErrorManager.clear();
            if (medicalController.openScene(file))
            {
                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (BeforeSceneLoadProperties != null)
                {
                    BeforeSceneLoadProperties.Invoke(medicalController.CurrentScene);
                }
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();

                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                    sceneViewController.createFromPresets(medicalScene.WindowPresets.Default, false);

                    sceneViewController.createCameras(medicalController.CurrentScene);

                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }

                    anatomyController.sceneLoaded();
                }
                success = true;
                if (behaviorErrorManager.HasErrors)
                {
                    NotificationManager.showCallbackNotification("Errors loading the scene.\nClick for details.", MessageBoxIcons.Error, showLoadErrorGui);
                }
            }
            return success;
        }

        private void showLoadErrorGui()
        {
            RmlWindow errorGui = new RmlWindow(behaviorErrorManager);
            StringBuilder htmlString = new StringBuilder();
            foreach (BehaviorBlacklistEventArgs blacklist in behaviorErrorManager.BlacklistEvents)
            {
                if (blacklist.Behavior != null)
                {
                    htmlString.AppendFormat("<p>Behavior {0}, type='{1}', SimObject='{3}' blacklisted.  Reason: {2}</p>", blacklist.Behavior.Name, blacklist.Behavior.GetType().Name, blacklist.Message, blacklist.Behavior.Owner != null ? blacklist.Behavior.Owner.Name : "NullOwner");
                }
                else
                {
                    htmlString.AppendFormat("<p>Null Behavior blacklisted.  Reason: {0}</p>", blacklist.Message);
                }
            }
            errorGui.setBodyMarkup(htmlString.ToString());
            guiManager.autoDisposeDialog(errorGui);
            errorGui.Visible = true;
        }

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out WindowInfo defaultWindow)
        {
            defaultWindow = new WindowInfo(mainWindow, "Primary");
            defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
            defaultWindow.MonitorIndex = 0;

            if (MedicalConfig.EngineConfig.Fullscreen)
            {
                mainWindow.showFullScreen();
                mainWindow.setSize(MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            }
            else
            {
                mainWindow.Maximized = true;
                mainWindow.show();
            }
        }

        /// <summary>
        /// Called before MyGUI renders.
        /// </summary>
        void myGUI_RenderStarted(object sender, EventArgs e)
        {
            watermark.Visible = false;
            measurementGrid.HideCaption = true;
        }

        /// <summary>
        /// Called after MyGUI renders.
        /// </summary>
        void myGUI_RenderEnded(object sender, EventArgs e)
        {
            watermark.Visible = true;
            measurementGrid.HideCaption = false;
        }

        void medicalController_FullSpeedLoopUpdate(Clock time)
        {
            ThreadManager.doInvoke();
        }

        void atlasPluginManager_PluginLoadError(string message)
        {
            //At this point the plugins have not actually been loaded, so we must use a callback and have the downloadController fire its gui open task.
            //At the point where these can be clicked that task will be defined.
            NotificationManager.showCallbackNotification(String.Format("{0}\nClick here to download a working version.", message), "MessageBoxIcon", delegate()
            {
                Task downloadGUITask = downloadController.OpenDownloadGUITask;
                if (downloadGUITask != null)
                {
                    downloadGUITask.clicked(null);
                }
            });
        }
    }
}