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
        private RecentDocuments recentDocuments;
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

        public BodyAtlasMainPlugin(LicenseManager licenseManager, BodyAtlasController bodyAtlasController)
        {
            this.licenseManager = licenseManager;
            this.bodyAtlasController = bodyAtlasController;
            recentDocuments = new RecentDocuments(MedicalConfig.RecentDocsFile);
        }

        public void Dispose()
        {
            recentDocuments.save();
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

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.setAppMenu(appMenu, "AppButton/Image");

            DialogOpenTaskbarItem renderTaskbarItem = new DialogOpenTaskbarItem(renderDialog, "Render", "RenderIcon");
            renderTaskbarItem.RightClicked += new EventHandler(renderTaskbarItem_RightClicked);
            taskbar.addItem(renderTaskbarItem);
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
                recentDocuments.addDocument(patientData.BackingFile);
            }
            else
            {
                MainWindow.Instance.clearWindowTitle();
            }
        }

        public RecentDocuments RecentDocuments
        {
            get
            {
                return recentDocuments;
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

        void renderTaskbarItem_OnClicked(object sender, EventArgs e)
        {
            renderDialog.render();
        }

        void renderTaskbarItem_RightClicked(object sender, EventArgs e)
        {
            renderDialog.render();
        }
    }
}
