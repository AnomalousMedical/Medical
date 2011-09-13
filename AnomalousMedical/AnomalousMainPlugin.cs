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
    class AnomalousMainPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private SystemMenu systemMenu;
        private LicenseManager licenseManager;
        private AnomalousController bodyAtlasController;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private AboutDialog aboutDialog;
        private ExamViewer examViewer;

        public AnomalousMainPlugin(LicenseManager licenseManager, AnomalousController bodyAtlasController)
        {
            this.licenseManager = licenseManager;
            this.bodyAtlasController = bodyAtlasController;
        }

        public void Dispose()
        {
            examViewer.Dispose();
            renderDialog.Dispose();
            options.Dispose();
            chooseSceneDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
            aboutDialog.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            standaloneController.DocumentController.addDocumentHandler(new PatientDocumentHandler(standaloneController, this));

            Gui.Instance.load("Medical.Resources.BodyAtlasImagesets.xml");

            //Create Dialogs
            aboutDialog = new AboutDialog(licenseManager);

            chooseSceneDialog = new ChooseSceneDialog();
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            savePatientDialog = new SavePatientDialog();
            savePatientDialog.SaveFile += new EventHandler(savePatientDialog_SaveFile);

            openPatientDialog = new OpenPatientDialog();
            openPatientDialog.OpenFile += new EventHandler(openPatientDialog_OpenFile);

            options = new OptionsDialog();
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer);
            guiManager.addManagedDialog(renderDialog);

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.setAppIcon("AppButton/Image");

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            //Patient Section
            taskController.addTask(new DialogOpenTask(chooseSceneDialog, "Medical.NewPatient", "New", "FileToolstrip/ChangeScene", TaskMenuCategories.Patient, 0, false));
            taskController.addTask(new DialogOpenTask(openPatientDialog, "Medical.OpenPatient", "Open", "FileToolstrip/Open", TaskMenuCategories.Patient, 1, false));

            CallbackTask saveTaskItem = new CallbackTask("Medical.SavePatient", "Save", "FileToolstrip/Save", TaskMenuCategories.Patient, 2, false);
            saveTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveTaskItem_OnClicked);
            taskController.addTask(saveTaskItem);

            CallbackTask saveAsTaskItem = new CallbackTask("Medical.SavePatientAs", "Save As", "FileToolstrip/SaveAs", TaskMenuCategories.Patient, 3, false);
            saveAsTaskItem.OnClicked += new CallbackTask.ClickedCallback(saveAsTaskItem_OnClicked);
            taskController.addTask(saveAsTaskItem);

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "ExamIcon", TaskMenuCategories.Patient, 4));

            //System Section
            CallbackTask helpTaskItem = new CallbackTask("Medical.Help", "Help", "FileToolstrip/Help", TaskMenuCategories.System, int.MaxValue - 4, false);
            helpTaskItem.OnClicked += new CallbackTask.ClickedCallback(helpTaskItem_OnClicked);
            taskController.addTask(helpTaskItem);

            taskController.addTask(new DialogOpenTask(options, "Medical.Options", "Options", "FileToolstrip/Options", TaskMenuCategories.System, int.MaxValue - 3));
            taskController.addTask(new DialogOpenTask(aboutDialog, "Medical.About", "About", "FileToolstrip/About", TaskMenuCategories.System, int.MaxValue - 2));
            taskController.addTask(new CheckForUpdatesTask(standaloneController, int.MaxValue - 2));

            CallbackTask logoutTaskItem = new CallbackTask("Medical.LogOut", "Log Out", "FileToolstrip/Exit", TaskMenuCategories.System, int.MaxValue - 1, false);
            logoutTaskItem.OnClicked += new CallbackTask.ClickedCallback(logoutTaskItem_OnClicked);
            taskController.addTask(logoutTaskItem);

            CallbackTask exitTaskItem = new CallbackTask("Medical.Exit", "Exit", "FileToolstrip/Exit", TaskMenuCategories.System, int.MaxValue, false);
            exitTaskItem.OnClicked += new CallbackTask.ClickedCallback(exitTaskItem_OnClicked);
            taskController.addTask(exitTaskItem);

            //Tools Section
            taskController.addTask(new MDIDialogOpenTask(renderDialog, "Medical.Render", "Render", "RenderIcon", TaskMenuCategories.Tools));
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            if (systemMenu != null)
            {
                systemMenu.FileMenuEnabled = enabled;
            }
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            systemMenu = new SystemMenu(menu, this, standaloneController, licenseManager);
        }

        public void sceneRevealed()
        {

        }

        public void showOptions()
        {
            options.Visible = true;
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
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
            if (standaloneController.MedicalStateController.getNumStates() == 0 && standaloneController.ExamController.Count == 0)
            {
                MessageBox.show("No information to save. Please create some states or perform an exam.", "Nothing to save.", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
            }
            else
            {
                savePatientDialog.save();
            }
        }

        public void saveAs()
        {
            if (standaloneController.MedicalStateController.getNumStates() == 0 && standaloneController.ExamController.Count == 0)
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
                standaloneController.DocumentController.addToRecentDocuments(patientData.BackingFile);
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

        void saveAsTaskItem_OnClicked(Task item)
        {
            saveAs();
        }

        void saveTaskItem_OnClicked(Task item)
        {
            save();
        }

        void logoutTaskItem_OnClicked(Task item)
        {
            MessageBox.show("Logging out will delete your local license file. This will require you to log in the next time you use this program.\nYou will also not be able to use the software in offline mode until you log back in and save your password.", "Log Out", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No,
                delegate(MessageBoxStyle result)
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        standaloneController.App.LicenseManager.deleteLicense();
                        standaloneController.exit();
                    }
                });
        }

        void exitTaskItem_OnClicked(Task item)
        {
            standaloneController.exit();
        }

        void helpTaskItem_OnClicked(Task item)
        {
            standaloneController.openHelpTopic(0);
        }
    }
}
