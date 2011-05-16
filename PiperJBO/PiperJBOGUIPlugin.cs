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
    class PiperJBOGUIPlugin : AtlasPlugin
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

        public PiperJBOGUIPlugin(LicenseManager licenseManager)
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
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.guiManager = guiManager;
            this.standaloneController = standaloneController;

            stateWizardPanelController = new StateWizardPanelController(Gui.Instance, standaloneController.MedicalController, standaloneController.MedicalStateController, NavigationController, LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer, standaloneController.MeasurementGrid);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, NavigationController, LayerController, guiManager);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);
            stateWizardController.Finished += new StatePickerFinished(stateWizardController_Finished);

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");
        }

        public void createDialogs(DialogManager dialogManager)
        {
            //Wizards
            wizards = new PiperJBOWizards(StateWizardPanelController, StateWizardController, licenseManager);

            //Distortions Popup, must come after wizards
            distortionChooser = new DistortionChooser(StateWizardController, this);
            dialogManager.addManagedDialog(distortionChooser);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new DialogOpenTaskbarItem(distortionChooser, "Distortions", "RigidBody"));
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

            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, scene.getDefaultSubScene().getSimElementManager<SimulationScene>());
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
