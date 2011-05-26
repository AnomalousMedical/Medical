using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgreWrapper;
using System.Diagnostics;

namespace Medical.GUI
{
    class PiperJBOAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private PiperJBOWizards wizards;
        private LicenseManager licenseManager;
        private NavigationController navigationController;
        private LayerController layerController;

        //Dialogs
        private DistortionChooser distortionChooser;

        //Wizards
        private StateWizardPanelController stateWizardPanelController;
        private StateWizardController stateWizardController;

        //Timeline Wizard
        private TimelineWizard timelineWizard;

        public PiperJBOAtlasPlugin(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
            navigationController = new NavigationController(); 
            layerController = new LayerController();
        }

        public void Dispose()
        {
            distortionChooser.Dispose();

            stateWizardController.Dispose();
            stateWizardPanelController.Dispose();

            navigationController.Dispose();

            timelineWizard.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            Gui.Instance.load("Medical.Resources.PiperJBOImagesets.xml");

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            stateWizardPanelController = new StateWizardPanelController(Gui.Instance, standaloneController.MedicalController, standaloneController.MedicalStateController, NavigationController, LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer, standaloneController.MeasurementGrid);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, NavigationController, LayerController, guiManager);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);
            stateWizardController.Finished += new StatePickerFinished(stateWizardController_Finished);

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);
            SceneViewController_ActiveWindowChanged(standaloneController.SceneViewController.ActiveWindow);

            //Create Dialogs
            //Wizards
            wizards = new PiperJBOWizards(StateWizardPanelController, StateWizardController, licenseManager);

            //Distortions Popup, must come after wizards
            distortionChooser = new DistortionChooser(StateWizardController, this);
            guiManager.addManagedDialog(distortionChooser);

            //Taskbar
            guiManager.Taskbar.addItem(new DialogOpenTaskbarItem(distortionChooser, "Distortions", "DistortionsIcon"));

            //Timeline GUIs
            timelineWizard = new TimelineWizard(standaloneController);
            standaloneController.TimelineController.GUIFactory.addPrototype(new RemoveTopTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new RemoveBottomTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new DisclaimerGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new RightDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new LeftDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new LeftDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new RightDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new FossaGUILeftPrototype(timelineWizard));
            standaloneController.TimelineController.GUIFactory.addPrototype(new FossaGUIRightPrototype(timelineWizard));
        }

        public void finishInitialization()
        {
            
        }

        public void sceneRevealed()
        {

        }

        public void sceneLoaded(SimScene scene)
        {
            String pathString = "{0}/{1}/{2}";
            MedicalController medicalController = standaloneController.MedicalController;
            SimSubScene defaultScene = medicalController.CurrentScene.getDefaultSubScene();
            SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
            StandaloneApp app = standaloneController.App;

            navigationController.loadNavigationSetIfDifferent(medicalController.CurrentSceneDirectory + "/" + medicalScene.CameraFileDirectory + "/RequiredCameras.cam");

            String layersFile = String.Format(pathString, medicalController.CurrentSceneDirectory, medicalScene.LayersFileDirectory, "StandaloneLayers.lay");
            layerController.loadLayerStateSet(layersFile);

            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, medicalScene);
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        #region StateWizard Callbacks

        public void startWizard(StateWizard wizard)
        {
            stateWizardPanelController.CurrentWizardName = wizard.Name;
            stateWizardController.startWizard(wizard);
            standaloneController.MovementSequenceController.stopPlayback();
            guiManager.setMainInterfaceEnabled(false);
        }

        void stateWizardController_Finished()
        {
            guiManager.setMainInterfaceEnabled(true);
        }

        void stateWizardController_StateCreated(MedicalState state)
        {
            standaloneController.MedicalStateController.addState(state);
        }

        #endregion StateWizard Callbacks

        void SceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            stateWizardController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardPanelController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
        }

        public StateWizardController StateWizardController
        {
            get
            {
                return stateWizardController;
            }
        }

        public StateWizardPanelController StateWizardPanelController
        {
            get
            {
                return stateWizardPanelController;
            }
        }

        public NavigationController NavigationController
        {
            get
            {
                return navigationController;
            }
        }

        public LayerController LayerController
        {
            get
            {
                return layerController;
            }
        }
    }
}
