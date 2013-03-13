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
        private BackgroundController backgroundController;
        private ViewportBackground background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private SceneViewWindowPresetController windowPresetController;
        private BehaviorErrorGui errorGui;

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
            atlasPluginManager = new AtlasPluginManager(this);
            atlasPluginManager.PluginLoadError += new Medical.AtlasPluginManager.PluginMessageDelegate(atlasPluginManager_PluginLoadError);
            guiManager = new GUIManager(this);

            MyGUIInterface.OSTheme = PlatformConfig.ThemeFile;

            //Engine core
            medicalController = new MedicalController();
            mainWindow = new MainWindow(app.WindowTitle);
            medicalController.initialize(app, mainWindow, createWindow);
            mainWindow.setPointerManager(PointerManager.Instance);
            idleHandler = new IdleHandler(medicalController.MainTimer.OnIdle);

            PointerManager.Instance.Visible = false;

            behaviorErrorManager = new BehaviorErrorManager();
        }

        public void Dispose()
        {
            if (errorGui != null)
            {
                errorGui.Dispose();
            }
            mvcCore.Dispose();
            downloadController.Dispose();
            DocumentController.saveRecentDocuments();
            if (touchController != null)
            {
                touchController.Dispose();
            }
            atlasPluginManager.Dispose();
            guiManager.Dispose();
            watermark.Dispose();
            measurementGrid.Dispose();
            anatomyController.Dispose();
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            mdiLayout.Dispose();
            medicalController.Dispose();
            mainWindow.Dispose();

            //Stop any waiting background threads last.
            ThreadManager.cancelAll();
        }

        public void addWorkingArchive()
        {
#if ALLOW_OVERRIDE
            //Add working archive
            VirtualFileSystem.Instance.addArchive(MedicalConfig.WorkingResourceDirectory);
#endif
        }

        public void initializeControllers(ViewportBackground background)
        {
            atlasPluginManager.manageInstalledPlugins();

            clipboard = new SaveableClipboard();

            //Documents
            DocumentController = new DocumentController();

            //Setup MyGUI listeners
            MyGUIInterface myGUI = MyGUIInterface.Instance;
            myGUI.RenderEnded += new EventHandler(myGUI_RenderEnded);
            myGUI.RenderStarted += new EventHandler(myGUI_RenderStarted);

            windowListener = new WindowListener(this);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);
            OgreInterface.Instance.OgrePrimaryWindow.OgreRenderWindow.DeactivateOnFocusChange = false;

            //MDI Layout
            mdiLayout = new MDILayoutManager();
            medicalController.MainTimer.addFixedUpdateListener(new MDIUpdate(medicalController.EventManager, mdiLayout));

            //SceneView
            sceneViewController = new SceneViewController(mdiLayout, medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow, myGUI.OgrePlatform.getRenderManager());

            //Watermark
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

            //Background
            this.background = background;
            backgroundController = new BackgroundController(background, sceneViewController);

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
            anatomyController = new AnatomyController(imageRenderer);

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
            windowPresetController = new SceneViewWindowPresetController();
            app.createWindowPresets(windowPresetController);

            //GUI
            guiManager.createGUI(mdiLayout);
            guiManager.giveGUIsToTimelineController(timelineController);
            medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);

            //Create scene view windows
            sceneViewController.createFromPresets(windowPresetController.getPresetSet("Primary"));
        }

        public void initializePlugins()
        {
            //Wizards
            ResourceManager.Instance.load("Medical.Resources.WizardImagesets.xml");

            ResourceManager.Instance.load("Medical.Resources.StandaloneIcons.xml");

            Taskbar taskbar = GUIManager.Taskbar;
            TaskMenu taskMenu = GUIManager.TaskMenu;
            taskbar.SuppressLayout = true;
            taskMenu.SuppressLayout = true;
            atlasPluginManager.initialzePlugins();
            taskbar.SuppressLayout = false;
            taskMenu.SuppressLayout = false;
            taskbar.layout();
            guiManager.loadSavedUI();

            //Load recent documents here, this way the document handlers are all loaded
            DocumentController.loadRecentDocuments(MedicalConfig.RecentDocsFile);
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

        public SceneViewWindowPresetController PresetWindows
        {
            get
            {
                return windowPresetController;
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
            background.destroyBackground();
            backgroundController.sceneUnloading();
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
                    backgroundController.sceneLoaded(ogreScene);
                    background.createBackground(ogreScene);

                    sceneViewController.createCameras(medicalController.CurrentScene);
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();

                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }

                    anatomyController.sceneLoaded();
                }
                success = true;
                if (behaviorErrorManager.HasErrors)
                {
                    guiManager.NotificationManager.showCallbackNotification("Errors loading the scene.\nClick for details.", CommonResources.NoIcon, new Action(() =>
                        {
                            errorGui = new BehaviorErrorGui(behaviorErrorManager);
                            guiManager.addManagedDialog(errorGui);
                            errorGui.Closed += (evt, args) =>
                                {
                                    guiManager.removeManagedDialog(errorGui);
                                    errorGui.Dispose();
                                    errorGui = null;
                                };
                            errorGui.Visible = true;
                        }));
                }
            }
            return success;
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

        void medicalController_FixedLoopUpdate(Clock time)
        {
        }

        void atlasPluginManager_PluginLoadError(string message)
        {
            //At this point the plugins have not actually been loaded, so we must use a callback and have the downloadController fire its gui open task.
            //At the point where these can be clicked that task will be defined.
            guiManager.NotificationManager.showCallbackNotification(String.Format("{0}\nClick here to download a working version.", message), "pic_CoreMessageIcon", delegate()
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