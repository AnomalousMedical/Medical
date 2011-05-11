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
    class PiperJBOGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private PiperJBOAppMenu appMenu;
        private PiperJBOWizards wizards;
        private CloneWindowTaskbarItem cloneWindow;
        private RecentDocuments recentDocuments;
        private SystemMenu systemMenu;
        private LicenseManager licenseManager;
        private PiperJBOController piperJBOController;

        //Dialogs
        private MandibleMovementDialog mandibleMovementDialog;
        private NotesDialog notesDialog;
        private StateListPopup stateList;
        private ChooseSceneDialog chooseSceneDialog;
        private SavePatientDialog savePatientDialog;
        private OpenPatientDialog openPatientDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private WindowLayout windowLayout;
        private SequencePlayer sequencePlayer;
        private AnatomyFinder anatomyFinder;
        private BookmarksGUI bookmarks;
        private DistortionChooser distortionChooser;
        private AboutDialog aboutDialog;

        public PiperJBOGUIPlugin(LicenseManager licenseManager, PiperJBOController piperJBOController)
        {
            this.licenseManager = licenseManager;
            this.piperJBOController = piperJBOController;
            recentDocuments = new RecentDocuments(MedicalConfig.RecentDocsFile);
        }

        public void Dispose()
        {
            recentDocuments.save();
            sequencePlayer.Dispose();
            windowLayout.Dispose();
            //cameraControlDialog.Dispose();
            distortionChooser.Dispose();
            renderDialog.Dispose();
            options.Dispose();
            chooseSceneDialog.Dispose();
            savePatientDialog.Dispose();
            openPatientDialog.Dispose();
            appMenu.Dispose();
            aboutDialog.Dispose();
            mandibleMovementDialog.Dispose();
            notesDialog.Dispose();
            stateList.Dispose();
            anatomyFinder.Dispose();
            bookmarks.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.guiManager = guiManager;
            this.standaloneController = standaloneController;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");

            appMenu = new PiperJBOAppMenu(this, standaloneController);
            guiManager.setAppMenu(appMenu);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            //PiperJBO
            mandibleMovementDialog = new MandibleMovementDialog(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(mandibleMovementDialog);

            //Common
            notesDialog = new NotesDialog(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(notesDialog);

            stateList = new StateListPopup(standaloneController.MedicalStateController);
            dialogManager.addManagedDialog(stateList);

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
            dialogManager.addManagedDialog(renderDialog);

            windowLayout = new WindowLayout(standaloneController);
            dialogManager.addManagedDialog(windowLayout);

            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(sequencePlayer);

            anatomyFinder = new AnatomyFinder(piperJBOController.AnatomyController, standaloneController.SceneViewController);
            dialogManager.addManagedDialog(anatomyFinder);

            bookmarks = new BookmarksGUI(piperJBOController.BookmarksController);

            //Wizards
            wizards = new PiperJBOWizards(guiManager.StateWizardPanelController, guiManager.StateWizardController, licenseManager);

            //Distortions Popup, must come after wizards
            distortionChooser = new DistortionChooser(guiManager.StateWizardController, guiManager);
            dialogManager.addManagedDialog(distortionChooser);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new ShowPopupTaskbarItem(bookmarks, "Bookmarks", "FavoritesIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(anatomyFinder, "Anatomy Finder", "SearchIcon"));
            taskbar.addItem(new ShowToothContactsTaskbarItem());
            taskbar.addItem(new DialogOpenTaskbarItem(distortionChooser, "Distortions", "RigidBody"));
            taskbar.addItem(new DialogOpenTaskbarItem(stateList, "States", "Joint"));
            taskbar.addItem(new DialogOpenTaskbarItem(notesDialog, "Notes", "Notes"));
            taskbar.addItem(new DialogOpenTaskbarItem(sequencePlayer, "Sequences", "SequenceIconLarge"));
            taskbar.addItem(new DialogOpenTaskbarItem(mandibleMovementDialog, "Manual Movement", "MovementIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(windowLayout, "Window Layout", "WindowLayoutIconLarge"));

            if (!licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_TRIAL))
            {
                if (licenseManager.allowFeature((int)Features.PIPER_JBO_FEATURE_FULL_RENDERING))
                {
                    DialogOpenTaskbarItem renderTaskbarItem = new DialogOpenTaskbarItem(renderDialog, "Render", "RenderIconLarge");
                    renderTaskbarItem.RightClicked += new EventHandler(renderTaskbarItem_RightClicked);
                    taskbar.addItem(renderTaskbarItem);
                }
                else
                {
                    CallbackTaskbarItem renderTaskbarItem = new CallbackTaskbarItem("Render", "RenderIconLarge");
                    renderTaskbarItem.OnClicked += new EventHandler(renderTaskbarItem_OnClicked);
                    taskbar.addItem(renderTaskbarItem);
                }
            }

            cloneWindow = new CloneWindowTaskbarItem(standaloneController);
            if (PlatformConfig.AllowCloneWindows && licenseManager.allowFeature((int)Features.PIPER_JBO_FEATURE_CLONE_WINDOW))
            {
                taskbar.addItem(cloneWindow);
            }
        }

        public void finishInitialization()
        {
            bool keyValid = licenseManager.KeyValid;
            if (!keyValid)
            {
                if (licenseManager.IsExpired)
                {
                    MessageBox.show("Your trial has expired. Would you like to go to AnomalousMedical.com and upgrade?", "Trial Expired", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, goToWebsiteCallback);
                }
                else
                {
                    startKeyDialog();
                }
            }
        }

        private void goToWebsiteCallback(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                Process.Start("http://www.anomalousmedical.com");
            }
            startKeyDialog();
        }

        private void startKeyDialog()
        {
            licenseManager.KeyEnteredSucessfully += new EventHandler(licenseManager_KeyEnteredSucessfully);
            licenseManager.KeyInvalid += new EventHandler(licenseManager_KeyInvalid);
            licenseManager.showKeyDialog(piperJBOController.ProductID);
        }

        void licenseManager_KeyInvalid(object sender, EventArgs e)
        {
            standaloneController.closeMainWindow();
        }

        void licenseManager_KeyEnteredSucessfully(object sender, EventArgs e)
        {
            MessageBox.show("Please restart to apply your license changes.", "Restart required", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo, restartMessageClosed);
        }

        void restartMessageClosed(MessageBoxStyle result)
        {
            standaloneController.closeMainWindow();
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
            if (systemMenu != null)
            {
                systemMenu.FileMenuEnabled = enabled;
            }
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            systemMenu = new SystemMenu(menu, this, standaloneController, licenseManager);
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
            cloneWindow.toggleCloneWindow();
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
