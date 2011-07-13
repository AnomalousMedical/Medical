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

            //Tasks Menu
            TaskMenuSection tasksSection = guiManager.TaskMenu.Tasks;

            foreach (StateWizard wizard in stateWizardController.WizardEnum)
            {
                tasksSection.addItem(new StartWizardTaskMenuItem(this, wizard));
            }

            //Timeline GUIs
            timelineWizard = new TimelineWizard(standaloneController);
            standaloneController.TimelineGUIFactory.addPrototype(new RemoveTopTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RemoveBottomTeethGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new DisclaimerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscSpaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDopplerGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUILeftPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new FossaGUIRightPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarDegenerationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightCondylarGrowthGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new LeftDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new RightDiscClockFaceGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new ProfileDistortionGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new TeethHeightAdaptationGUIPrototype(timelineWizard));
            standaloneController.TimelineGUIFactory.addPrototype(new NotesGUIPrototype(timelineWizard));
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
