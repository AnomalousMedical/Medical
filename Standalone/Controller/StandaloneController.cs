﻿using System;
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
using System.Runtime.InteropServices;
using System.Reflection;
using MyGUIPlugin;
using Medical.GUI;
using Medical.Controller;
using System.IO;
using System.Diagnostics;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.Editor;
using Engine.Threads;
using Anomalous.OSPlatform;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;
using OgrePlugin.VirtualTexture;

namespace Medical
{
    public delegate void SceneEvent(SimScene scene);

    public class StandaloneController : IDisposable
    {
        static StandaloneController()
        {
            MedicalRmlSlideUpdater.Touch();
        }

        //Events
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;
        public event SceneEvent BeforeSceneLoadProperties;

        //Controller
        private MedicalController medicalController;
        private MedicalStateController medicalStateController;
        private MusclePositionController musclePositionController;
        private MovementSequenceController movementSequenceController;
        private ImageRenderer imageRenderer;
        private TimelineController timelineController;
        private AnomalousMvcCore mvcCore;
        private MyGUIViewHostFactory viewHostFactory;
        private PropFactory propFactory;
        private TaskController taskController;
        private SaveableClipboard clipboard;
        private AnatomyController anatomyController;
        private PatientDataController patientDataController;
        private IdleHandler idleHandler;
        private SceneStatsDisplayManager sceneStatsDisplayManager;
        private SceneViewLightManager lightManager;
        private LayerController layerController;
        private VirtualTextureSceneViewLink virtualTextureSceneViewLink;
        private AnatomyTaskManager anatomyTaskManager;

        //GUI
        private GUIManager guiManager;
        private SceneViewController sceneViewController;
        private BackgroundScene background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private NotificationGUIManager notificationManager;

        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIImageDisplayFactory imageDisplayFactory;
        private MyGUITextDisplayFactory textDisplayFactory;
        private MyGUIImageRendererProgress imageRendererProgress;

        //Platform
        private MainWindow mainWindow;
        private App app;
        private AtlasPluginManager atlasPluginManager;
		private bool shuttingDown = false;
        private MedicalConfig medicalConfig;

        public StandaloneController(App app)
        {
            medicalConfig = new MedicalConfig();
            this.app = app;

            guiManager = new GUIManager();
            guiManager.MainGUIShown += guiManager_MainGUIShown;
            guiManager.MainGUIHidden += guiManager_MainGUIHidden;

            MyGUIInterface.OSTheme = PlatformConfig.ThemeFile;

            String title;
            if(MedicalConfig.BuildName != null)
            {
                title = String.Format("{0} {1}", app.Title, MedicalConfig.BuildName);
            }
            else
            {
                title = app.Title;
            }
            mainWindow = new MainWindow(title);
            mainWindow.Closed += mainWindow_Closed;

            //Setup DPI
            float pixelScale = mainWindow.WindowScaling;

            if (MedicalConfig.PixelScaleOverride > 0.5f)
            {
                pixelScale = MedicalConfig.PixelScaleOverride;
            }

            float scaleFactor = pixelScale;
            pixelScale += MedicalConfig.PlatformExtraScaling * scaleFactor;

            switch (MedicalConfig.ExtraScaling)
            {
                case UIExtraScale.Smaller:
                    pixelScale -= .15f * scaleFactor;
                    break;
                case UIExtraScale.Larger:
                    pixelScale += .25f * scaleFactor;
                    break;
            }

            ScaleHelper._setScaleFactor(pixelScale);

            //Initialize engine
            medicalController = new MedicalController(mainWindow);
            idleHandler = new IdleHandler(medicalController.MainTimer.OnIdle);

            PointerManager.Instance.Visible = false;

            ((RenderWindow)OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget).DeactivateOnFocusChange = false;
        }

        public void Dispose()
        {
            unloadScene();
            medicalController.unloadSceneAndResources();
            PluginManager.Instance.RendererPlugin.destroySceneViewLightManager(lightManager);
			IDisposableUtil.DisposeIfNotNull(mvcCore);
            IDisposableUtil.DisposeIfNotNull(anatomyController);
			IDisposableUtil.DisposeIfNotNull(atlasPluginManager);
            IDisposableUtil.DisposeIfNotNull(virtualTextureSceneViewLink);
            IDisposableUtil.DisposeIfNotNull(notificationManager);
            IDisposableUtil.DisposeIfNotNull(imageRendererProgress);
            IDisposableUtil.DisposeIfNotNull(continuePrompt);
            IDisposableUtil.DisposeIfNotNull(guiManager);
			IDisposableUtil.DisposeIfNotNull(measurementGrid);
			IDisposableUtil.DisposeIfNotNull(medicalStateController);
			IDisposableUtil.DisposeIfNotNull(sceneViewController);
			IDisposableUtil.DisposeIfNotNull(background);
			IDisposableUtil.DisposeIfNotNull(mdiLayout);
			IDisposableUtil.DisposeIfNotNull(medicalController);
			IDisposableUtil.DisposeIfNotNull(mainWindow);
        }

        /// <summary>
        /// Save the current app state as config files.
        /// </summary>
        public void saveConfiguration()
        {
            if (LicenseManager != null && LicenseManager.Valid)
            {
                MedicalConfig.save();
                if (DocumentController != null)
                {
                    DocumentController.saveRecentDocuments();
                }
                if (guiManager != null && MedicalConfig.WindowsFile != null)
                {
                    ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
                    guiManager.saveUI(configFile, UpdateController.CurrentVersion);
                    configFile.writeConfigFile();
                }
            }
        }

        /// <summary>
        /// This function will try to load the certificate store again, It is only intended to be called during startup
        /// when the user is asked to connect to the internet, please do not call it any other time.
        /// </summary>
        public void retryLoadingCertificateStore()
        {
            
        }

        public void addWorkingArchive()
        {
            //Add working archive
            if (!String.IsNullOrEmpty(MedicalConfig.WorkingResourceDirectory))
            {
                VirtualFileSystem.Instance.addArchive(MedicalConfig.WorkingResourceDirectory);
            }
        }

        public void initializeControllers(BackgroundScene background, LicenseManager licenseManager)
        {
            //Background
            this.background = background;
            this.LicenseManager = licenseManager;
            atlasPluginManager = new AtlasPluginManager(this);
            atlasPluginManager.PluginLoadError += new Medical.AtlasPluginManager.PluginMessageDelegate(atlasPluginManager_PluginLoadError);
            atlasPluginManager.manageInstalledPlugins();

            clipboard = new SaveableClipboard();

            //Documents
            DocumentController = new DocumentController();

            //MDI Layout
            mdiLayout = new MDILayoutManager();

            //SceneView
            MyGUIInterface myGUI = MyGUIInterface.Instance;
            sceneViewController = new SceneViewController(mdiLayout, medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow, myGUI.OgrePlatform.RenderManager, background);
            sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            sceneViewController.WindowDestroyed += sceneViewController_WindowDestroyed;
            sceneViewController.DefaultBackgroundColor = new Color(0.274f, 0.274f, 0.274f);
            sceneStatsDisplayManager = new SceneStatsDisplayManager(sceneViewController, OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget);
            sceneStatsDisplayManager.StatsVisible = MedicalConfig.EngineConfig.ShowStatistics;
            MedicalConfig.EngineConfig.ShowStatsToggled += engineConfig => sceneStatsDisplayManager.StatsVisible = engineConfig.ShowStatistics;
            lightManager = PluginManager.Instance.RendererPlugin.createSceneViewLightManager();

            //Measurement grid
            measurementGrid = new MeasurementGrid("MeasurementGrid", sceneViewController);
            SceneUnloading += measurementGrid.sceneUnloading;
            SceneLoaded += measurementGrid.sceneLoaded;

            //Image Renderer
            imageRenderer = new ImageRenderer(medicalController, sceneViewController, idleHandler);
            imageRenderer.Background = background;
            imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;

            //Anatomy Controller
            anatomyController = new AnatomyController();

            //Medical states
            medicalStateController = new MedicalStateController(imageRenderer, medicalController);
            SceneLoaded += medicalStateController.sceneLoaded;
            SceneUnloading += medicalStateController.sceneUnloading;

            //Movement sequences
            movementSequenceController = new MovementSequenceController(medicalController);
            this.SceneLoaded += movementSequenceController.sceneLoaded;
            musclePositionController = new MusclePositionController(medicalController.MainTimer, this);

            SceneLoaded += SleepyActorRepository.SceneLoaded;
            SceneUnloading += SleepyActorRepository.SceneUnloading;

            //Props
            propFactory = new PropFactory(this);

            //Timeline
            timelineController = new TimelineController(this);

            viewHostFactory = new MyGUIViewHostFactory(mdiLayout);
            mvcCore = new AnomalousMvcCore(this, viewHostFactory);

            //Patient data
            patientDataController = new PatientDataController(this);

            //Tasks
            taskController = new TaskController();

            anatomyTaskManager = new AnatomyTaskManager(taskController, guiManager);

            //Coroutine
            Coroutine.SetTimer(medicalController.MainTimer);

            //Notifications
            notificationManager = new NotificationGUIManager();

            layerController = new LayerController();

            //Create virtual texture manager
            virtualTextureSceneViewLink = new VirtualTextureSceneViewLink(this);
        }

        public void createGUI(LayoutChain layoutChain)
        {
            MyGUIInterface.Instance.CommonResourceGroup.addResource(GetType().AssemblyQualifiedName, "EmbeddedScalableResource", true);

            //GUI
            guiManager.createGUI(mdiLayout, layoutChain, mainWindow);
            guiManager.ScreenSizeChanged += guiManager_ScreenSizeChanged;

            imageRendererProgress = new MyGUIImageRendererProgress();
            imageRenderer.ImageRendererProgress = imageRendererProgress;
            imageRenderer.ImageTextWriter = new RocketTextWriter();

            continuePrompt = new MyGUIContinuePromptProvider();

            imageDisplayFactory = new MyGUIImageDisplayFactory(sceneViewController);
            textDisplayFactory = new MyGUITextDisplayFactory(sceneViewController);

            giveGUIsToTimelineController(timelineController);
        }

        public void giveGUIsToTimelineController(TimelineController timelineController)
        {
            timelineController.ContinuePrompt = continuePrompt;
            timelineController.ImageDisplayFactory = imageDisplayFactory;
            timelineController.TextDisplayFactory = textDisplayFactory;
        }

        void guiManager_MainGUIHidden()
        {
            notificationManager.hideAllNotifications();
        }

        void guiManager_MainGUIShown()
        {
            notificationManager.reshowAllNotifications();
        }

        public void loadConfigAndCommonResources()
        {
            ResourceManager.Instance.load("Medical.Resources.StandaloneIcons.xml");
            ResourceManager.Instance.load("Medical.Resources.LockedFeature.xml");
            ResourceManager.Instance.load("Medical.Resources.CommonToolstrip.xml");
            ResourceManager.Instance.load("Medical.Resources.SlideshowIcons.xml");

            ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
            configFile.loadConfigFile();
            guiManager.loadSavedUI(configFile, new Version("1.0.0.2818"));

            //Load recent documents here, this way the document handlers are all loaded
            DocumentController.loadRecentDocuments(MedicalConfig.RecentDocsFile);

            guiManager.layout();
        }

        public void onIdle()
        {
            idleHandler.onIdle();
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

        public void restartWithWarning(String noDownloadsMessage, bool autoStartUpdate, bool asAdmin)
        {
            String message = noDownloadsMessage;
            if (message == null)
            {
                restart(asAdmin);
            }
            else
            {
                MessageBox.show(message, "Restart", MessageBoxStyle.IconInfo | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        UpdateController.AutoStartUpdate = autoStartUpdate;
                        restart(asAdmin);
                    }
                });
            }
        }

        public void restart(bool asAdmin)
        {
            if (!shuttingDown)
            {
                shuttingDown = true;
                if (PlatformConfig.CloseMainWindowOnShutdown)
                {
                    mainWindow.close();
                }
                app.restart(asAdmin);
            }
        }

        /// <summary>
        /// Opens a scene as a "new" scene by opening the given file and clearing the states.
        /// </summary>
        /// <param name="filename"></param>
        public void openNewScene(String filename)
        {
            foreach (var status in openNewSceneStatus(filename)) { }
        }

        public IEnumerable<SceneBuildStatus> openNewSceneStatus(String filename)
        {
            foreach(var status in openNewSceneStatus(filename, new VirtualFilesystemResourceProvider()))
            {
                yield return status;
            }
        }

        public IEnumerable<SceneBuildStatus> openNewSceneStatus(String filename, ResourceProvider resourceProvider)
        {
            medicalStateController.clearStates();
            foreach(var status in changeSceneStatus(filename, resourceProvider))
            {
                yield return status;
            }
            patientDataController.clearData();
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }

        public MusclePositionController MusclePositionController
        {
            get
            {
                return musclePositionController;
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

        public LicenseManager LicenseManager { get; private set; }

        public TimelineController TimelineController
        {
            get
            {
                return timelineController;
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

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }

        public MainWindow MainWindow
        {
            get
            {
                return mainWindow;
            }
        }

        public VirtualTextureManager VirtualTextureManager
        {
            get
            {
                return virtualTextureSceneViewLink != null ? virtualTextureSceneViewLink.VirtualTextureManager : null;
            }
        }

        public AnatomyTaskManager AnatomyTaskManager
        {
            get
            {
                return anatomyTaskManager;
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

            MessageBox.show("You will need to restart the program to apply your settings.\nWould you like to restart now?", "Apply Changes?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No,
                result =>
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        this.restart(false);
                    }
                });
        }

        public void saveCrashLog()
        {
            if (medicalController != null)
            {
                medicalController.saveCrashLog();
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        internal void changeScene(String file)
        {
            foreach (var status in changeSceneStatus(file)) { }
        }

        internal IEnumerable<SceneBuildStatus> changeSceneStatus(String file)
        {
            foreach (var status in changeSceneStatus(file, new VirtualFilesystemResourceProvider()))
            {
                yield return status;
            }
        }

        internal IEnumerable<SceneBuildStatus> changeSceneStatus(String file, ResourceProvider resourceProvider)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sceneViewController.resetAllCameraPositions();
            unloadScene();
            SimObjectErrorManager.Clear();
            
            foreach(var status in medicalController.openScene(file, resourceProvider))
            {
                yield return status;
            }

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
                lightManager.sceneLoaded(medicalController.CurrentScene);

                if (SceneLoaded != null)
                {
                    SceneLoaded.Invoke(medicalController.CurrentScene);
                }

                anatomyController.sceneLoaded();
            }

            if (SimObjectErrorManager.HasErrors)
            {
                NotificationManager.showCallbackNotification("Errors loading the scene.\nClick for details.", MessageBoxIcons.Error, showLoadErrorGui);
            }

            sw.Stop();
            Logging.Log.Debug("Scene '{0}' loaded in {1} ms", file, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Called when a scene is unloading, this is called by changeScene and Dispose.
        /// </summary>
        private void unloadScene()
        {
            if (movementSequenceController != null && movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            if (anatomyController != null)
            {
                anatomyController.sceneUnloading();
            }
            if (sceneViewController != null)
            {
                sceneViewController.destroyCameras();
            }
            if(lightManager != null)
            {
                lightManager.sceneUnloading(medicalController.CurrentScene);
            }
        }

        private void showLoadErrorGui()
        {
            SceneErrorWindow errorGui = new SceneErrorWindow(guiManager);
            errorGui.Visible = true;
        }

        void atlasPluginManager_PluginLoadError(string message)
        {
            
        }

        void mainWindow_Closed(OSWindow sender)
        {
            exit();
        }

        void guiManager_ScreenSizeChanged(int width, int height)
        {
            continuePrompt.ensureVisible(width, height);
        }

        void sceneViewController_WindowDestroyed(SceneViewWindow window)
        {
            TransparencyController.removeTransparencyState(window.CurrentTransparencyState);
            window.RenderingStarted -= window_RenderingStarted;
            window.MadeActive -= window_MadeActive;
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            if (!TransparencyController.hasTransparencyState(window.CurrentTransparencyState))
            {
                TransparencyController.createTransparencyState(window.CurrentTransparencyState);
            }
            window.RenderingStarted += window_RenderingStarted;
            window.MadeActive += window_MadeActive;
        }

        void window_MadeActive(SceneViewWindow window)
        {
            TransparencyController.ActiveTransparencyState = window.CurrentTransparencyState;
        }

        void window_RenderingStarted(SceneViewWindow window, bool currentCameraRender)
        {
            TransparencyController.applyTransparencyState(window.CurrentTransparencyState);
        }
    }
}