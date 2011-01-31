#define PROFILE_LOAD

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

namespace Medical
{
    public delegate void SceneEvent(SimScene scene);

    public class StandaloneController : IDisposable
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
        private SimObjectMover propMover;
        private ImageRenderer imageRenderer;
        private TimelineController timelineController;
        private PropFactory propFactory;

        //GUI
        private GUIManager guiManager;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;
        private MDILayoutManager mdiLayout;
        private MeasurementGrid measurementGrid;
        private SceneViewWindowPresetController windowPresetController;
        private HtmlHelpController htmlHelpController;
        private MyGUIImageDisplayFactory imageDisplayFactory;

        //Support Files
        private String camerasFile;
        private String layersFile;
        private List<String> movementSequenceDirectories;

        //Platform
        private MainWindow mainWindow;
        private App app;

        //Touch
        private TouchController touchController;

        public StandaloneController(App app, String documentPath, String camerasFile, String layersFile, List<String> movementSequenceDirectories)
        {
            this.app = app;
            this.camerasFile = camerasFile;
            this.layersFile = layersFile;
            this.movementSequenceDirectories = movementSequenceDirectories;

            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + documentPath);
            guiManager = new GUIManager(this);

#if MAC_OSX
            MyGUIInterface.Theme = "core_theme_osx.xml";
#endif

            //Engine core
            medicalController = new MedicalController();
            mainWindow = new MainWindow();
            Medical.Controller.WindowFunctions.setWindowIcon(mainWindow, Medical.Controller.WindowIcons.ICON_SKULL);
            medicalController.initialize(mainWindow, createWindow);
            mainWindow.setPointerManager(PointerManager.Instance);
        }

        public void Dispose()
        {
            if (touchController != null)
            {
                touchController.Dispose();
            }
            htmlHelpController.Dispose();
            guiManager.Dispose();
            watermark.Dispose();
            measurementGrid.Dispose();
            movementSequenceController.Dispose();
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            layerController.Dispose();
            navigationController.Dispose();
            mdiLayout.Dispose();
            medicalController.Dispose();
            mainWindow.Dispose();
        }

        public void go(ViewportBackground background)
        {
            //Splash screen
            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);
            SplashScreen splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, 100);
            splashScreen.Hidden += new EventHandler(splashScreen_Hidden);

            OgreInterface.Instance.OgrePrimaryWindow.OgreRenderWindow.windowMovedOrResized();

            splashScreen.updateStatus(10, "Initializing Core");

            //Help
            htmlHelpController = new HtmlHelpController();
            htmlHelpController.AddBook(MedicalConfig.ProgramDirectory + "/Doc/PiperJBOHelpFile.htb");

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

            //Navigation and layers
            navigationController = new NavigationController();
            layerController = new LayerController();

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

            //Prop Mover
            propMover = new SimObjectMover("Props", medicalController.PluginManager, medicalController.EventManager);
            this.SceneLoaded += propMover.sceneLoaded;
            this.SceneUnloading += propMover.sceneUnloading;
            medicalController.FixedLoopUpdate += propMover.update;

            //Props
            propFactory = new PropFactory(this);
            Arrow.createPropDefinition(propFactory);
            Ruler.createPropDefinition(propFactory);
            PointingHand.createPropDefinition(propFactory);
            Doppler.createPropDefinition(propFactory);
            Syringe.createPropDefinition(propFactory);
            //Mustache.createPropDefinition(propFactory);

            //Timeline
            timelineController = new TimelineController(this);
            imageDisplayFactory = new MyGUIImageDisplayFactory();
            MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(imageDisplayFactory);
            timelineController.ImageDisplayFactory = imageDisplayFactory;
            timelineController.SimObjectMover = propMover;

            //GUI
            guiManager.createGUI();
            guiManager.ScreenLayout.Center = mdiLayout;
            medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);

#if CREATE_MAINWINDOW_MENU
            guiManager.createMenuBar(mainWindow.MenuBar);
#endif

            //Create scene view windows
            sceneViewController.createFromPresets(windowPresetController.getPresetSet("Primary"));

            //MultiTouch
            if (MultiTouch.IsAvailable)
            {
                touchController = new TouchController(mainWindow, medicalController.MainTimer, sceneViewController);
            }
            else
            {
                Log.Info("MultiTouch not available");
            }

            splashScreen.updateStatus(40, "Loading Scene");

            if (changeScene(MedicalConfig.DefaultScene, splashScreen))
            {
                splashScreen.updateStatus(100, "");
                splashScreen.hide();

                medicalStateController.createNormalStateFromScene();

                Active = true;
                medicalController.start();
            }
        }

        public void closeMainWindow()
        {
            mainWindow.close();
        }

        public void onIdle()
        {
            medicalController.MainTimer.OnIdle();
        }

        public void openHelpTopic(int index)
        {
            htmlHelpController.Display(index);
        }

        public void shutdown()
        {
            app.exit();
        }

        /// <summary>
        /// Opens a scene as a "new" scene by opening the given file and clearing the states.
        /// </summary>
        /// <param name="filename"></param>
        public void openNewScene(String filename)
        {
            medicalStateController.clearStates();
            changeScene(filename, null);
            medicalStateController.createNormalStateFromScene();
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
                    guiManager.changeLeftPanel(null);
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

        public TimelineController TimelineController
        {
            get
            {
                return timelineController;
            }
        }

        public PropFactory PropFactory
        {
            get
            {
                return propFactory;
            }
        }

        public GUIManager GUIManager
        {
            get
            {
                return guiManager;
            }
        }

        public bool Active { get; set; }

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
                this.closeMainWindow();
            }
        }

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file, SplashScreen splashScreen)
        {
#if PROFILE_LOAD
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            bool success = false;
            sceneViewController.resetAllCameraPositions();
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
                success = true;
            }
#if PROFILE_LOAD
            sw.Stop();
            Log.Info("Scene loaded in {0} ms.", sw.ElapsedMilliseconds);
#endif
            return success;
        }

        private void loadExternalFiles(SimulationScene medicalScene)
        {
            String pathString = "{0}/{1}/{2}";
            String layersFile = String.Format(pathString, medicalController.CurrentSceneDirectory, medicalScene.LayersFileDirectory, this.layersFile);
            String cameraFile = String.Format(pathString, medicalController.CurrentSceneDirectory, medicalScene.CameraFileDirectory, this.camerasFile);
            String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;

            movementSequenceController.loadSequenceDirectories(sequenceDirectory, this.movementSequenceDirectories);

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

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                SceneViewWindowPresetSet twoWindows = new SceneViewWindowPresetSet("Two Windows");
                //twoWindows.Image = Resources.TwoWindowLayout;
                preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                twoWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Right;
                twoWindows.addPreset(preset);
                windowPresetController.addPresetSet(twoWindows);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
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
            }
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

        void splashScreen_Hidden(object sender, EventArgs e)
        {
            SplashScreen splashScreen = (SplashScreen)sender;
            splashScreen.Dispose();
        }
    }
}