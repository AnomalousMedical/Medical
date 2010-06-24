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

        //GUI
        private BasicGUI basicGUI;
        private SceneViewController sceneViewController;

        public StandaloneController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/Articulometrics/Standalone");
        }

        public void Dispose()
        {
            medicalStateController.Dispose();
            sceneViewController.Dispose();
            basicGUI.Dispose();
            layerController.Dispose();
            navigationController.Dispose();
            medicalController.Dispose();
        }

        public void go()
        {
            medicalController = new MedicalController();
            medicalController.initialize(null, new AgnosticMessagePump(), createWindow);
            windowListener = new WindowListener(this);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);

            navigationController = new NavigationController(medicalController.EventManager, medicalController.MainTimer);
            layerController = new LayerController();
            medicalStateController = new MedicalStateController(medicalController);
            tempStateBlender = new TemporaryStateBlender(medicalController.MainTimer, medicalStateController);

            sceneViewController = new SceneViewController(medicalController.EventManager, medicalController.MainTimer, medicalController.PluginManager.RendererPlugin.PrimaryWindow);

            basicGUI = new BasicGUI(this);
            basicGUI.ScreenLayout.Root.Center = sceneViewController.LayoutContainer;

            if (changeScene(MedicalConfig.DefaultScene))
            {
                medicalController.start();
            }
        }

        public void shutdown()
        {
            sceneViewController.destroyCameras();
            medicalController.shutdown();
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

        /// <summary>
        /// Change the scene to the specified filename.
        /// </summary>
        /// <param name="filename"></param>
        private bool changeScene(String file)
        {
            sceneViewController.resetAllCameraPositions();
            navigationController.recalculateClosestNonHiddenStates();
            //StatusController.SetStatus(String.Format("Opening scene {0}...", VirtualFileSystem.GetFileName(file)));
            //if (movementSequenceController.Playing)
            //{
            //    movementSequenceController.stopPlayback();
            //}
            //distortionController.setToDefault();
            if (SceneUnloading != null && medicalController.CurrentScene != null)
            {
                SceneUnloading.Invoke(medicalController.CurrentScene);
            }
            sceneViewController.destroyCameras();
            //background.destroyBackground();
            //backgroundController.sceneUnloading();
            if (medicalController.openScene(file))
            {
                SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
                if (defaultScene != null)
                {
                    OgreSceneManager ogreScene = defaultScene.getSimElementManager<OgreSceneManager>();
                    //backgroundController.sceneLoaded(ogreScene);
                    //background.createBackground(ogreScene);

                    sceneViewController.createCameras(medicalController.CurrentScene);
                    SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();

                    loadExternalFiles(medicalScene);
                    //distortionController.sceneChanged(medicalController.CurrentScene, medicalController.CurrentSceneDirectory + "/" + medicalScene.PresetDirectory);
                    //statePickerPanelController.sceneChanged(medicalController, medicalScene);
                    if (SceneLoaded != null)
                    {
                        SceneLoaded.Invoke(medicalController.CurrentScene);
                    }
                }
                //distortionController.setToDefault();
                //StatusController.TaskCompleted();
                return true;
            }
            else
            {
                //StatusController.TaskCompleted();
                return false;
            }
        }

        private void loadExternalFiles(SimulationScene medicalScene)
        {
            String layersFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.LayersFileDirectory;
            String cameraFile = medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory;
            String sequenceDirectory = medicalController.CurrentSceneDirectory + "/" + medicalScene.SequenceDirectory;

            cameraFile += "/GraphicsCameras.cam";
            layersFile += "/GraphicsLayersStandaloneTemp.lay";

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
    }
}