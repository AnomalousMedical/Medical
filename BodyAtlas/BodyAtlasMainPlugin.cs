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
    class BodyAtlasMainPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private BodyAtlasAppMenu appMenu;
        private SystemMenu systemMenu;
        private LicenseManager licenseManager;
        private BodyAtlasController bodyAtlasController;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private AboutDialog aboutDialog;
        private ExamViewer examViewer;

        public BodyAtlasMainPlugin(LicenseManager licenseManager, BodyAtlasController bodyAtlasController)
        {
            this.licenseManager = licenseManager;
            this.bodyAtlasController = bodyAtlasController;
        }

        public void Dispose()
        {
            renderDialog.Dispose();
            options.Dispose();
            chooseSceneDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
            appMenu.Dispose();
            aboutDialog.Dispose();
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            standaloneController.DocumentController.addDocumentHandler(new PatientDocumentHandler(standaloneController, this));

            Gui.Instance.load("Medical.Resources.BodyAtlasImagesets.xml");

            appMenu = new BodyAtlasAppMenu(this, standaloneController);

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

            TaskMenu taskMenu = guiManager.TaskMenu;
            taskMenu.setAppMenu(appMenu);

            //Tasks Menu
            TaskMenuSection tasksSection = guiManager.TaskMenu.Tasks;

            tasksSection.addItem(new DialogOpenTaskMenuItem(chooseSceneDialog, "New", "FileToolstrip/ChangeScene", TaskMenuCategories.Patient));
            tasksSection.addItem(new DialogOpenTaskMenuItem(openPatientDialog, "Open", "FileToolstrip/Open", TaskMenuCategories.Patient));

            CallbackTaskMenuItem saveTaskItem = new CallbackTaskMenuItem("Save", "FileToolstrip/Save", TaskMenuCategories.Patient);
            saveTaskItem.OnClicked +=new CallbackTaskMenuItem.ClickedCallback(saveTaskItem_OnClicked);
            tasksSection.addItem(saveTaskItem);

            CallbackTaskMenuItem saveAsTaskItem = new CallbackTaskMenuItem("Save As", "FileToolstrip/SaveAs", TaskMenuCategories.Patient);
            saveAsTaskItem.OnClicked += new CallbackTaskMenuItem.ClickedCallback(saveAsTaskItem_OnClicked);
            tasksSection.addItem(saveAsTaskItem);

            CallbackTaskMenuItem helpTaskItem = new CallbackTaskMenuItem("Help", "FileToolstrip/Help", TaskMenuCategories.System);
            helpTaskItem.OnClicked += new CallbackTaskMenuItem.ClickedCallback(helpTaskItem_OnClicked);
            tasksSection.addItem(helpTaskItem);

            tasksSection.addItem(new MDIDialogOpenTaskMenuItem(renderDialog, "Render", "RenderIcon", TaskMenuCategories.Tools));
            tasksSection.addItem(new MDIDialogOpenTaskMenuItem(examViewer, "Exam Viewer", "ExamIcon", TaskMenuCategories.Patient));
            tasksSection.addItem(new DialogOpenTaskMenuItem(options, "Options", "FileToolstrip/Options", TaskMenuCategories.System));
            tasksSection.addItem(new DialogOpenTaskMenuItem(aboutDialog, "About", "FileToolstrip/About", TaskMenuCategories.System));

            CallbackTaskMenuItem logoutTaskItem = new CallbackTaskMenuItem("Log Out", "FileToolstrip/Exit", TaskMenuCategories.System);
            logoutTaskItem.OnClicked += new CallbackTaskMenuItem.ClickedCallback(logoutTaskItem_OnClicked);
            tasksSection.addItem(logoutTaskItem);

            CallbackTaskMenuItem exitTaskItem = new CallbackTaskMenuItem("Exit", "FileToolstrip/Exit", TaskMenuCategories.System);
            exitTaskItem.OnClicked += new CallbackTaskMenuItem.ClickedCallback(exitTaskItem_OnClicked);
            tasksSection.addItem(exitTaskItem);

            tasksSection.addItem(new CheckForUpdatesTaskMenuItem(standaloneController));
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

        void saveAsTaskItem_OnClicked()
        {
            saveAs();
        }

        void saveTaskItem_OnClicked()
        {
            save();
        }

        void logoutTaskItem_OnClicked()
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

        void exitTaskItem_OnClicked()
        {
            standaloneController.exit();
        }

        void helpTaskItem_OnClicked()
        {
            standaloneController.openHelpTopic(0);
        }
    }
}
