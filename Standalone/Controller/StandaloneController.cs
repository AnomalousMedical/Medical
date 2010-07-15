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

        //GUI
        private BasicGUI basicGUI;
        private SceneViewController sceneViewController;
        private Watermark watermark;
        private BackgroundController backgroundController;
        private ViewportBackground background;

        public StandaloneController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/Articulometrics/Standalone");
        }

        public void Dispose()
        {
            basicGUI.Dispose();
            watermark.Dispose();
            movementSequenceController.Dispose();
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            layerController.Dispose();
            navigationController.Dispose();
            medicalController.Dispose();
        }

        public void go()
        {
            //Engine core
            medicalController = new MedicalController();
            medicalController.initialize(null, new AgnosticMessagePump(), createWindow);
            windowListener = new WindowListener(this);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);

            //SceneView
            sceneViewController = new SceneViewController(medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow);

            //Navigation and layers
            navigationController = new NavigationController(medicalController.EventManager, medicalController.MainTimer);
            layerController = new LayerController();

            //Watermark
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("/Watermark", "EngineArchive", "Watermark", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().createResourceGroup("__InternalMedical");
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            watermark = new SideLogoWatermark("AnomalousMedicalWatermark", "AnomalousMedical", 150, 44, 4, 4);

            //Image Renderer
            imageRenderer = new ImageRenderer(medicalController, sceneViewController, layerController, navigationController);
            imageRenderer.Watermark = watermark;
            imageRenderer.Background = background;
            //imageRenderer.ImageRenderStarted += measurementGrid.ScreenshotRenderStarted;
            //imageRenderer.ImageRenderCompleted += measurementGrid.ScreenshotRenderCompleted;
            
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

            //GUI
            MyGUIInterface myGUI = medicalController.PluginManager.getPlugin("MyGUIPlugin") as MyGUIInterface;
            myGUI.RenderEnded += new EventHandler(myGUI_RenderEnded);
            myGUI.RenderStarted += new EventHandler(myGUI_RenderStarted);
            basicGUI = new BasicGUI(this);
            basicGUI.ScreenLayout.Root.Center = sceneViewController.LayoutContainer;
            medicalController.FixedLoopUpdate += new LoopUpdate(medicalController_FixedLoopUpdate);
            medicalController.FullSpeedLoopUpdate += new LoopUpdate(medicalController_FullSpeedLoopUpdate);

            createBackground();

            //Create scene view windows
            sceneViewController.createWindow("DefaultWindow", new Vector3(0, -5, 150), new Vector3(0, -5, 0));

            if (changeScene(MedicalConfig.DefaultScene))
            {
                medicalController.start();
            }
        }

        void medicalController_FullSpeedLoopUpdate(Clock time)
        {
            ThreadManager.doInvoke();
        }

        void medicalController_FixedLoopUpdate(Clock time)
        {
            
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
            changeScene(filename);
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
                    changeScene(MedicalConfig.SceneDirectory + "/" + states.SceneName);
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

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
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

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out DefaultWindowInfo defaultWindow)
        {
            defaultWindow = new DefaultWindowInfo("Articulometrics", MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
            defaultWindow.MonitorIndex = 0;
        }

        /// <summary>
        /// Called before MyGUI renders.
        /// </summary>
        void myGUI_RenderStarted(object sender, EventArgs e)
        {
            watermark.Visible = false;
        }

        /// <summary>
        /// Called after MyGUI renders.
        /// </summary>
        void myGUI_RenderEnded(object sender, EventArgs e)
        {
            watermark.Visible = true;
        }
    }
}