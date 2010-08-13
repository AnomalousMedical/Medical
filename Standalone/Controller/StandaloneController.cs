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
using PCPlatform;
using OgrePlugin;
using OgreWrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using MyGUIPlugin;
using Medical.GUI;
using Medical.Controller;

namespace Standalone
{
    public delegate void SceneEvent(SimScene scene);

    class StandaloneController : IDisposable
    {
        //Events
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;

        //Controller
        private MedicalController medicalController;
        private WindowListener windowListener;
        private NavigationController navigationController;
        private LayerController layerController;
        private MedicalStateController medicalStateController;
        private TemporaryStateBlender tempStateBlender;
        private MovementSequenceController movementSequenceController;
        private SimObjectMover teethMover;
        private ImageRenderer imageRenderer;
        private OSMessagePump messagePump;

        //GUI
        private BasicGUI basicGUI;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private SceneViewWindowPresetController windowPresetController;

        public StandaloneController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/Articulometrics/Standalone");
        }

        public void Dispose()
        {
            basicGUI.Dispose();
            watermark.Dispose();
            measurementGrid.Dispose();
            movementSequenceController.Dispose();
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            layerController.Dispose();
            navigationController.Dispose();
            mdiLayout.Dispose();
            medicalController.Dispose();
        }

        public void go()
        {
            //Engine core
            medicalController = new MedicalController();
#if CALL_DO_EVENTS //If we need to call DoEvents use the WinformsMessagePump (OSX).
            messagePump = new WinformsMessagePump();
#else
            messagePump = new AgnosticMessagePump();
#endif
            medicalController.initialize(null, messagePump, createWindow);
            messagePump.processMessages();

            //Splash screen
            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);
            SplashScreen splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, 100);

            splashScreen.updateStatus(10, "Initializing Core");

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
            MyGUIInterface myGui = MyGUIInterface.Instance;
            sceneViewController = new SceneViewController(mdiLayout, medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow, myGui.OgrePlatform.getRenderManager());
            sceneViewController.AllowRotation = false;
            sceneViewController.AllowZoom = false;

            //Navigation and layers
            navigationController = new NavigationController(sceneViewController, medicalController.EventManager, medicalController.MainTimer);
            layerController = new LayerController();

            //Watermark
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

            //Background
            createBackground();

            //Measurement grid
            measurementGrid = new MeasurementGrid("MeasurementGrid", medicalController, sceneViewController);
            SceneUnloading += measurementGrid.sceneUnloading;
            SceneLoaded += measurementGrid.sceneLoaded;

            //Image Renderer
            imageRenderer = new ImageRenderer(medicalController, sceneViewController, layerController, navigationController);
            imageRenderer.Watermark = watermark;
            imageRenderer.Background = background;
            imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;
            
            //Medical states
            medicalStateController = new MedicalStateController(imageRenderer, medicalController);
            tempStateBlender = new TemporaryStateBlender(medicalController.MainTimer, medicalStateController);

            //Movement sequences
            movementSequenceController = new MovementSequenceController(medicalController);

            //Teeth mover
            teethMover = new SimObjectMover("Teeth", medicalController.PluginManager, medicalController.EventManager);
            this.SceneLoaded += teethMover.sceneLoaded;
            this.SceneUnloading += teethMover.sceneUnloading;
            TeethController.TeethMover = teethMover;
            medicalController.FixedLoopUpdate += teethMover.update;
            imageRenderer.ImageRenderStarted += TeethController.ScreenshotRenderStarted;
            imageRenderer.ImageRenderCompleted += TeethController.ScreenshotRenderCompleted;

            splashScreen.updateStatus(20, "Creating GUI");

            windowPresetController = new SceneViewWindowPresetController();
            createWindowPresets();

            //GUI
            basicGUI = new BasicGUI(this);
            basicGUI.ScreenLayout.Root.Center = mdiLayout;
            medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);

            //Create scene view windows
            //MDISceneViewWindow camera1 = sceneViewController.createWindow("Camera 1", new Vector3(0, -5, 170), new Vector3(0, -5, 0));
            //MDISceneViewWindow camera2 = sceneViewController.createWindow("Camera 2", new Vector3(0, -5, -170), new Vector3(0, -5, 0), camera1, WindowAlignment.Left);
            //MDISceneViewWindow camera3 = sceneViewController.createWindow("Camera 3", new Vector3(-170, -5, 0), new Vector3(0, -5, 0), camera1, WindowAlignment.Bottom);
            //MDISceneViewWindow camera4 = sceneViewController.createWindow("Camera 4", new Vector3(170, -5, 0), new Vector3(0, -5, 0), camera2, WindowAlignment.Bottom);
            sceneViewController.createFromPresets(windowPresetController.getPresetSet("Primary"));

            splashScreen.updateStatus(40, "Loading Scene");

            if (changeScene(MedicalConfig.DefaultScene, splashScreen))
            {
                //temp hack to show navigation arrows for initial scene
                navigationController.recalculateClosestNonHiddenStates();
                //end hack
                splashScreen.updateStatus(100, "");
                splashScreen.hide();
                medicalController.start();
            }
        }

        public void shutdown()
        {
            sceneViewController.destroyCameras();
            medicalController.shutdown();
        }

        /// <summary>
        /// Opens a scene as a "new" scene by opening the given file and clearing the states.
        /// </summary>
        /// <param name="filename"></param>
        public void openNewScene(String filename)
        {
            medicalStateController.clearStates();
            changeScene(filename, null);
        }

        public void saveMedicalState(PatientDataFile patientData)
        {
            if (medicalStateController.getNumStates() == 0)
            {
                medicalStateController.createNormalStateFromScene();
            }
            patientData.SavedStates = medicalStateController.getSavedState(medicalController.CurrentSceneFile);
            patientData.save();
        }

        public void openPatientFile(PatientDataFile dataFile)
        {
            if (dataFile.loadData())
            {
                SavedMedicalStates states = dataFile.SavedStates;
                if (states != null)
                {
                    changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName, null);
                    medicalStateController.setStates(states);
                    medicalStateController.blend(0.0f);
                    basicGUI.changeLeftPanel(null);
                }
                else
                {
                    MessageBox.show(String.Format("Error loading file {0}.\nCould not read state information.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
                dataFile.closeData();
            }
            else
            {
                MessageBox.show(String.Format("Error loading file {0}.\nCould not load patient data.", dataFile.BackingFile), "Load Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
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

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }

        public NavigationController NavigationController
        {
            get
            {
                return navigationController;
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

            MessageBox.show("You will need to restart the program to apply your settings.\nWould you like to shut down now?", "Apply Changes?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, displayParameterChangeCallback);   
        }

        void displayParameterChangeCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                this.shutdown();
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file, SplashScreen splashScreen)
        {
            sceneViewController.resetAllCameraPositions();
            navigationController.recalculateClosestNonHiddenStates();
            //StatusController.SetStatus(String.Format("Opening scene {0}...", VirtualFileSystem.GetFileName(file)));
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            sceneViewController.destroyCameras();
            background.destroyBackground();
            backgroundController.sceneUnloading();
            if (medicalController.openScene(file))
            {
                if (splashScreen != null)
                {
                    splashScreen.updateStatus(75, "Loading Scene Properties");
                }
                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();
                    backgroundController.sceneLoaded(ogreScene);
                    background.createBackground(ogreScene);

                    sceneViewController.createCameras(medicalController.CurrentScene);
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();

                    loadExternalFiles(medicalScene);
                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }
                }
                //StatusController.TaskCompleted();
                return true;
            }
            else
            {
                //StatusController.TaskCompleted();
                return false;
            }
        }

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private void createBackground()
        {
            background = new ViewportBackground("SourceBackground", "PiperJBOGraphicsBackground", 900, 500, 500, 5, 5);
            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBOGraphicsBackground", 900, 500, 500, 5, 5);
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBOMRIBackground", 900, 500, 500, 5, 5);
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBORadiographyBackground", 900, 500, 500, 5, 5);
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBOClinicalBackground", 900, 500, 500, 5, 5);
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBODentitionProfileBackground", 900, 500, 500, 5, 5);
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            //{
            //    background = new ViewportBackground("SourceBackground", "PiperJBODopplerBackground", 900, 500, 500, 5, 5);
            //}
            backgroundController = new BackgroundController(background, sceneViewController);
        }

        private void loadExternalFiles(SimulationScene medicalScene)
        {
            String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFileDirectory;
            String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory;
            String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;

            cameraFile += "/GraphicsCameras.cam";
            layersFile += "/GraphicsLayersStandaloneTemp.lay";
            movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Graphics",
                    sequenceDirectory + "/MRI",
                    sequenceDirectory + "/RadiographyCT",
                    sequenceDirectory + "/Clinical",
                    sequenceDirectory + "/DentitionProfile",
                    sequenceDirectory + "/Doppler");

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Graphics",
            //        sequenceDirectory + "/MRI",
            //        sequenceDirectory + "/RadiographyCT",
            //        sequenceDirectory + "/Clinical",
            //        sequenceDirectory + "/DentitionProfile",
            //        sequenceDirectory + "/Doppler");
            //    cameraFile += "/GraphicsCameras.cam";
            //    layersFile += "/GraphicsLayers.lay";
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/MRI",
            //        sequenceDirectory + "/RadiographyCT",
            //        sequenceDirectory + "/Clinical",
            //        sequenceDirectory + "/DentitionProfile",
            //        sequenceDirectory + "/Doppler");
            //    cameraFile += "/MRICameras.cam";
            //    layersFile += "/MRILayers.lay";
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/RadiographyCT",
            //        sequenceDirectory + "/Clinical",
            //        sequenceDirectory + "/DentitionProfile",
            //        sequenceDirectory + "/Doppler");
            //    cameraFile += "/RadiographyCameras.cam";
            //    layersFile += "/RadiographyLayers.lay";
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Clinical",
            //        sequenceDirectory + "/DentitionProfile",
            //        sequenceDirectory + "/Doppler");
            //    cameraFile += "/ClinicalCameras.cam";
            //    layersFile += "/ClinicalLayers.lay";
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/DentitionProfile",
            //        sequenceDirectory + "/Doppler");
            //    cameraFile += "/DentitionProfileCameras.cam";
            //    layersFile += "/DentitionProfileLayers.lay";
            //}
            //else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            //{
            //    movementSequenceController.loadSequenceDirectories(sequenceDirectory + "/Doppler");
            //    cameraFile += "/DopplerCameras.cam";
            //    layersFile += "/DopplerLayers.lay";
            //}
            layerController.loadLayerStateSet(layersFile);
            //Load camera file, merge baseline cameras if the cameras changed
            if (navigationController.loadNavigationSetIfDifferent(cameraFile))
            {
                navigationController.mergeNavigationSet(medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory + "/RequiredCameras.cam");
            }
        }

        private void createWindowPresets()
        {
            windowPresetController.clearPresetSets();
            SceneViewWindowPresetSet primary = new SceneViewWindowPresetSet("Primary");
            SceneViewWindowPreset preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            primary.addPreset(preset);
            primary.Hidden = true;
            windowPresetController.addPresetSet(primary);

            SceneViewWindowPresetSet oneWindow = new SceneViewWindowPresetSet("One Window");
            //oneWindow.Image = Resources.OneWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            windowPresetController.addPresetSet(oneWindow);

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL) ||
            //    UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT) ||
            //    UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI) ||
            //    UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            //{
                SceneViewWindowPresetSet twoWindows = new SceneViewWindowPresetSet("Two Windows");
                //twoWindows.Image = Resources.TwoWindowLayout;
                preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                twoWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Right;
                twoWindows.addPreset(preset);
                windowPresetController.addPresetSet(twoWindows);
            //}

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            //{
                SceneViewWindowPresetSet threeWindows = new SceneViewWindowPresetSet("Three Windows");
                //threeWindows.Image = Resources.ThreeWindowLayout;
                preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                threeWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 2", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Left;
                threeWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 3", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Right;
                threeWindows.addPreset(preset);
                windowPresetController.addPresetSet(threeWindows);

                SceneViewWindowPresetSet fourWindows = new SceneViewWindowPresetSet("Four Windows");
                //fourWindows.Image = Resources.FourWindowLayout;
                preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                fourWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Right;
                fourWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 3", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Bottom;
                fourWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 4", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 2";
                preset.WindowPosition = WindowAlignment.Bottom;
                fourWindows.addPreset(preset);
                windowPresetController.addPresetSet(fourWindows);
            //}
        }

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out WindowInfo defaultWindow)
        {
            defaultWindow = new WindowInfo("Piper's Joint Based Occlusion", MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
            if (!defaultWindow.Fullscreen)
            {
                defaultWindow.Width = 800;
                defaultWindow.Height = 600;
            }
            defaultWindow.MonitorIndex = 0;
            defaultWindow.WindowCreated += new EventHandler(defaultWindow_WindowCreated);
        }

        void defaultWindow_WindowCreated(object sender, EventArgs e)
        {
            OSWindow createdWindow = ((WindowInfoEventArgs)e).CreatedWindow.Handle;
            WindowInfo windowInfo = sender as WindowInfo;
            WindowFunctions.setWindowIcon(createdWindow, WindowIcons.ICON_SKULL);
            if (!windowInfo.Fullscreen)
            {
                WindowFunctions.maximizeWindow(createdWindow);
            }
            messagePump.processMessages();
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
    }
}