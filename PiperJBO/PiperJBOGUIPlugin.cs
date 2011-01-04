using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class PiperJBOGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;

        private MandibleMovementDialog mandibleMovementDialog;
        private LayersDialog layers;

        private NotesDialog notesDialog;
        private StateListPopup stateList;
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private OptionsDialog options;

        private AboutDialog aboutDialog;

        private PiperJBOAppMenu appMenu;
        private PiperJBOGUI mainGUI;

        public PiperJBOGUIPlugin()
        {

        }

        public void Dispose()
        {
            options.Dispose();
            chooseSceneDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
            appMenu.Dispose();
            aboutDialog.Dispose();
            mandibleMovementDialog.Dispose();
            layers.Dispose();
            notesDialog.Dispose();
            stateList.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, PiperJBOGUI mainGUI)
        {
            this.mainGUI = mainGUI;
            this.standaloneController = standaloneController;
            appMenu = new PiperJBOAppMenu(this, standaloneController);
            mainGUI.setAppMenu(appMenu);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            //PiperJBO
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(mandibleMovementDialog);

            layers = new LayersDialog(standaloneController.LayerController);
            dialogManager.addManagedDialog(layers);

            //Common
            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(notesDialog);

            stateList = new StateListPopup(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(stateList);

            aboutDialog = new AboutDialog();

            chooseSceneDialog = new ChooseSceneDialog();
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            savePatientDialog = new SavePatientDialog();
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog();
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            //Wizards
            createWizardPanels(mainGUI.StateWizardPanelController, mainGUI.StateWizardController);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new ShowToothContactsTaskbarItem());
            taskbar.addItem(new QuickViewTaskbarItem(standaloneController.NavigationController, standaloneController.SceneViewController, standaloneController.LayerController));
            taskbar.addItem(new DialogOpenTaskbarItem(layers, "Custom Layers", "ManualObject"));
            taskbar.addItem(new DistortionsTaskbarItem(mainGUI.StateWizardController, mainGUI));
            taskbar.addItem(new DialogOpenTaskbarItem(stateList, "States", "Joint"));
            taskbar.addItem(new DialogOpenTaskbarItem(notesDialog, "Notes", "Notes"));
            taskbar.addItem(new SequencesTaskbarItem(standaloneController.MovementSequenceController));
            taskbar.addItem(new DialogOpenTaskbarItem(mandibleMovementDialog, "Manual Movement", "MovementIcon"));
            taskbar.addItem(new WindowLayoutTaskbarItem(standaloneController));
            taskbar.addItem(new RenderTaskbarItem(standaloneController.SceneViewController, standaloneController.ImageRenderer));
            taskbar.addItem(new BackgroundColorTaskbarItem(standaloneController.SceneViewController));
            taskbar.addItem(new CloneWindowTaskbarItem(standaloneController));
        }

        public void sceneLoaded(SimScene scene)
        {
            mandibleMovementDialog.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            mandibleMovementDialog.sceneUnloading(scene);
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            layers.AllowShortcuts = enabled;
#if CREATE_MAINWINDOW_MENU
            systemMenu.FileMenuEnabled = enabled;
#endif
        }

#if CREATE_MAINWINDOW_MENU
        private SystemMenu systemMenu;
#endif

        public void createMenuBar(wx.MenuBar menu)
        {
#if CREATE_MAINWINDOW_MENU
            systemMenu = new SystemMenu(menu, this, standaloneController);
#endif
        }

        public void showOptions()
        {
            options.Visible = true;
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
        }

        internal void toggleCloneWindow()
        {
            throw new NotImplementedException();
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

        private void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        private void chooseSceneDialog_ChooseScene(object sender, EventArgs e)
        {
            changeActiveFile(null);
            standaloneController.openNewScene(chooseSceneDialog.SelectedFile);
        }

        private void createWizardPanels(StateWizardPanelController stateWizardPanelController, StateWizardController stateWizardController)
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
