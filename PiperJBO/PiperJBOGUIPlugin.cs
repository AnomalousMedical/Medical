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
        private PiperJBOWizards wizards;

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
            wizards = new PiperJBOWizards(mainGUI.StateWizardPanelController, mainGUI.StateWizardController);
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
    }
}
