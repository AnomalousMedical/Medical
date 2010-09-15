using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;
using Medical.Controller;
using Logging;
using Engine.Platform;

namespace Medical.GUI
{
    class PiperJBOGUI : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private PiperJBORibbon basicRibbon;
        private MyGUILayoutContainer basicRibbonContainer;
        private StandaloneController standaloneController;
        private LeftPopoutLayoutContainer leftAnimatedContainer;
        private TopPopoutLayoutContainer topAnimatedContainer;
        private StateWizardPanelController stateWizardPanelController;
        private StateWizardController stateWizardController;
        private StateList stateList;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private AboutDialog aboutDialog;
        private OptionsDialog options;
        private CloneWindowDialog cloneWindowDialog;

        public PiperJBOGUI(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            Gui gui = Gui.Instance;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);
            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(PiperJBOGUI).AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);

            typeof(PiperJBOGUI).Assembly.GetManifestResourceNames();

            gui.load("Imagesets.xml");

            stateWizardPanelController = new StateWizardPanelController(gui, standaloneController.MedicalController, standaloneController.MedicalStateController, standaloneController.NavigationController, standaloneController.LayerController, standaloneController.SceneViewController, standaloneController.TemporaryStateBlender, standaloneController.MovementSequenceController, standaloneController.ImageRenderer, standaloneController.MeasurementGrid);
            stateWizardController = new StateWizardController(standaloneController.MedicalController.MainTimer, standaloneController.TemporaryStateBlender, standaloneController.NavigationController, standaloneController.LayerController, this);
            stateWizardController.StateCreated += new MedicalStateCreated(stateWizardController_StateCreated);
            stateWizardController.Finished += new StatePickerFinished(stateWizardController_Finished);

            createWizardPanels();

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.Root.SuppressLayout = true;
            basicRibbon = new PiperJBORibbon(this, standaloneController, stateWizardController);
            basicRibbonContainer = new MyGUILayoutContainer(basicRibbon.RibbonRootWidget);
            topAnimatedContainer = new TopPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            screenLayoutManager.Root.Top = topAnimatedContainer;
            topAnimatedContainer.setInitialPanel(basicRibbonContainer);

            leftAnimatedContainer = new LeftPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            ScreenLayout.Root.Left = leftAnimatedContainer;
            screenLayoutManager.Root.SuppressLayout = false;

            stateList = new StateList(standaloneController.MedicalStateController);

            chooseSceneDialog = new ChooseSceneDialog(standaloneController, this);
            savePatientDialog = new SavePatientDialog();
            openPatientDialog = new OpenPatientDialog();
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);
            aboutDialog = new AboutDialog();

            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            standaloneController.SceneViewController.ActiveWindowChanged += new SceneViewWindowEvent(SceneViewController_ActiveWindowChanged);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            standaloneController.ImageRenderer.ImageRendererProgress = new MyGUIImageRendererProgress();

            cloneWindowDialog = new CloneWindowDialog();
            cloneWindowDialog.CreateCloneWindow += new EventHandler(cloneWindowDialog_CreateCloneWindow);
        }

        public void Dispose()
        {
            aboutDialog.Dispose();
            chooseSceneDialog.Dispose();
            stateWizardController.Dispose();
            stateWizardPanelController.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            basicRibbon.Dispose();
        }

        public void windowChanged(OSWindow newWindow)
        {
            screenLayoutManager.changeOSWindow(newWindow);
        }

        public void changeTopPanel(LayoutContainer topContainer)
        {
            if (topContainer != null)
            {
                topContainer.Visible = true;
                topContainer.bringToFront();
            }
            topAnimatedContainer.changePanel(topContainer, 0.25f, animationCompleted);
        }

        public void resetTopPanel()
        {
            changeTopPanel(basicRibbonContainer);
        }

        public void changeLeftPanel(LayoutContainer leftContainer)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            else if (standaloneController.MedicalStateController.getNumStates() > 0)
            {
                leftContainer = stateList.LayoutContainer;
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            if (leftAnimatedContainer.CurrentContainer != leftContainer)
            {
                leftAnimatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
            }
        }

        public void showChooseSceneDialog()
        {
            chooseSceneDialog.open(true);
        }

        public void open()
        {
            openPatientDialog.open(true);
        }

        public void save()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please create some states using the wizards first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.save();
            }
        }

        public void saveAs()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0)
            {
                MessageBox.show("No information to save. Please create some states using the wizards first.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.saveAs();
            }
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
        }

        public void showOptions()
        {
            options.Visible = true;
        }

        public void changeActiveFile(PatientDataFile patientData)
        {
            if (patientData != null)
            {
                MainWindow.Instance.updateWindowTitle(String.Format("{0} {1}", patientData.FirstName, patientData.LastName));
                MedicalConfig.RecentDocuments.addDocument(patientData.BackingFile);
            }
            else
            {
                MainWindow.Instance.clearWindowTitle();
            }
        }

        public void toggleCloneWindow()
        {
            if (standaloneController.SceneViewController.HasCloneWindow)
            {
                standaloneController.SceneViewController.destroyCloneWindow();
            }
            else
            {
                cloneWindowDialog.open(true);
            }
        }

        void cloneWindowDialog_CreateCloneWindow(object sender, EventArgs e)
        {
            standaloneController.SceneViewController.createCloneWindow(cloneWindowDialog.createWindowInfo());
        }

        void SceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            stateWizardController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
            stateWizardPanelController.CurrentSceneView = standaloneController.SceneViewController.ActiveWindow;
        }

        #region StateWizard Callbacks

        public void startWizard(StateWizard wizard)
        {
            stateWizardPanelController.CurrentWizardName = wizard.Name;
            stateWizardController.startWizard(wizard);
            basicRibbon.AllowLayerShortcuts = false;
            standaloneController.MovementSequenceController.stopPlayback();
            #if CREATE_MAINWINDOW_MENU
            systemMenu.FileMenuEnabled = false;
            #endif
        }

        void stateWizardController_Finished()
        {
            basicRibbon.AllowLayerShortcuts = true;
            #if CREATE_MAINWINDOW_MENU
            systemMenu.FileMenuEnabled = true;
            #endif
        }

        void stateWizardController_StateCreated(MedicalState state)
        {
            standaloneController.MedicalStateController.addState(state);
        }

        #endregion StateWizard Callbacks

        public ScreenLayoutManager ScreenLayout
        {
            get
            {
                return screenLayoutManager;
            }
        }

        private void savePatientDialog_SaveFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = savePatientDialog.PatientData;
            changeActiveFile(patientData);
            standaloneController.saveMedicalState(patientData);
        }

        private void openPatientDialog_OpenFile(object sender, EventArgs e)
        {
            PatientDataFile patientData = openPatientDialog.CurrentFile;
            changeActiveFile(patientData);
            standaloneController.openPatientFile(patientData);
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (oldChild != null)
            {
                oldChild.Visible = false;
            }
        }

        private void standaloneController_SceneUnloading(SimScene scene)
        {
            basicRibbon.sceneUnloading(scene);
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            basicRibbon.sceneLoaded(scene);
            stateWizardPanelController.sceneChanged(standaloneController.MedicalController, scene.getDefaultSubScene().getSimElementManager<SimulationScene>());
            this.changeLeftPanel(null);
        }

        void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

#if CREATE_MAINWINDOW_MENU

        private SystemMenu systemMenu;

        public wx.MenuBar createMenuBar()
        {
            wx.MenuBar menu = new wx.MenuBar();
            systemMenu = new SystemMenu(menu, this, standaloneController);
            return menu;
        }

#endif

        private void createWizardPanels()
        {
            //Create single distortion wizards
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DOPPLER))
            {
                //Doppler
                StateWizard dopplerWizard = new StateWizard("Doppler", stateWizardController, WizardType.Exam);
                dopplerWizard.TextLine1 = "Doppler";
                dopplerWizard.ImageKey = "DistortionsToolstrip/Doppler";
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                dopplerWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(dopplerWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DENTITION))
            {
                //Teeth
                StateWizard teethWizard = new StateWizard("Dentition", stateWizardController, WizardType.Anatomy);
                teethWizard.TextLine1 = "Dentition";
                teethWizard.ImageKey = "DistortionsToolstrip/Dentition";
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                teethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(teethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC))
            {
                //Profile
                StateWizard profileWizard = new StateWizard("Cephalometric", stateWizardController, WizardType.Anatomy);
                profileWizard.TextLine1 = "Cephalometric";
                profileWizard.ImageKey = "DistortionsToolstrip/Cephalometric";
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileWizard);
            }

            

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MANDIBLE))
            {
                //Bone
                StateWizard boneWizard = new StateWizard("Mandible", stateWizardController, WizardType.Anatomy);
                boneWizard.TextLine1 = "Mandible";
                boneWizard.ImageKey = "DistortionsToolstrip/Mandible";
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                boneWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(boneWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_SPACE))
            {
                //Disc
                StateWizard discWizard = new StateWizard("Disc Space", stateWizardController, WizardType.Exam);
                discWizard.TextLine1 = "Disc Space";
                discWizard.ImageKey = "DistortionsToolstrip/DiscSpace";
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                discWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_DISC_CLOCK))
            {
                //Disc
                StateWizard discClockWizard = new StateWizard("Disc Clock Face", stateWizardController, WizardType.Anatomy);
                discClockWizard.TextLine1 = "Disc";
                discClockWizard.TextLine2 = "Clock Face";
                discClockWizard.ImageKey = "DistortionsToolstrip/DiscClockFace";
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                discClockWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(discClockWizard);
            }

            //Create combination distortion wizards

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CEPHALOMETRIC_DENTITION))
            {
                //Profile + Teeth
                StateWizard profileTeethWizard = new StateWizard("Cephalometric and Dentition", stateWizardController, WizardType.Exam);
                profileTeethWizard.TextLine1 = "Cephalometric";
                profileTeethWizard.TextLine2 = "and Dentition";
                profileTeethWizard.ImageKey = "DistortionsToolstrip/CephalometricAndDentition";
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                profileTeethWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(profileTeethWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_CLINICAL_DOPPLER))
            {
                //Clinical
                StateWizard clinicalWizard = new StateWizard("Clinical and Doppler", stateWizardController, WizardType.Exam);
                clinicalWizard.TextLine1 = "Clinical";
                clinicalWizard.TextLine2 = "and Doppler";
                clinicalWizard.ImageKey = "DistortionsToolstrip/ClinicalAndDoppler";
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDopplerPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.ProfileDistortionPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethHeightAdaptationPanel));
                clinicalWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(clinicalWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_RADIOGRAPHY))
            {
                //CT/Radiography Wizard
                StateWizard ctWizard = new StateWizard("Clinical and Radiography", stateWizardController, WizardType.Exam);
                ctWizard.TextLine1 = "Radiography";
                ctWizard.ImageKey = "DistortionsToolstrip/ClinicalAndRadiography";
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscSpacePanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                ctWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(ctWizard);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_WIZARD_MRI))
            {
                //MRI Wizard
                StateWizard mriWizard = new StateWizard("Clinical and MRI", stateWizardController, WizardType.Exam);
                mriWizard.TextLine1 = "MRI";
                mriWizard.ImageKey = "DistortionsToolstrip/ClinicalAndMRI";
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.DisclaimerPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.LeftFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightDiscClockFacePanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarGrowth));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightCondylarDegeneration));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.RightFossa));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TopTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.BottomTeethRemovalPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.TeethAdaptationPanel));
                mriWizard.addStatePanel(stateWizardPanelController.getPanel(WizardPanels.NotesPanel));
                stateWizardController.addWizard(mriWizard);
            }
        }
    }
}