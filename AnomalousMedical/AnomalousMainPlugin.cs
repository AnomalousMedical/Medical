using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgreWrapper;
using System.Diagnostics;
using System.Reflection;

namespace Medical.GUI
{
    class AnomalousMainPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private SystemMenu systemMenu;
        private LicenseManager licenseManager;
        private AnomalousController bodyAtlasController;
        private DownloadManagerServer downloadServer;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private OptionsDialog options;
        //private RenderPropertiesDialog renderDialog;
        private AboutDialog aboutDialog;
        private AnatomyFinder anatomyFinder;
        private DownloadManagerGUI downloadManagerGUI;
        private SequencePlayer sequencePlayer = null;

        //Tasks
        private SelectionModeTask selectionModeTask;
        private Task downloadsTask;

        public AnomalousMainPlugin(LicenseManager licenseManager, AnomalousController bodyAtlasController)
        {
            this.licenseManager = licenseManager;
            this.bodyAtlasController = bodyAtlasController;
        }

        public void Dispose()
        {
            downloadServer.Dispose();
            selectionModeTask.Dispose();
            //renderDialog.Dispose();
            options.Dispose();
            anatomyFinder.Dispose();
            chooseSceneDialog.Dispose();
            aboutDialog.Dispose();
            if (sequencePlayer != null)
            {
                sequencePlayer.Dispose();
            }
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            standaloneController.MovementSequenceController.GroupAdded += MovementSequenceController_GroupAdded;

            Gui.Instance.load("Medical.Resources.BodyAtlasImagesets.xml");

            bool hasPremium = licenseManager.allowFeature(1);
            if(!hasPremium)
            {
                guiManager.TaskMenu.AdImageKey = "AnomalousMedical/PremiumAd";
            }
            standaloneController.AnatomyController.ShowPremiumAnatomy = hasPremium;
            guiManager.TaskMenu.ShowAdImage = !hasPremium;

            //Controllers
            downloadServer = new DownloadManagerServer(licenseManager);

            //Create Dialogs
            aboutDialog = new AboutDialog(licenseManager);

            chooseSceneDialog = new ChooseSceneDialog(guiManager);
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            //standaloneController.AnatomyController.AllowIndividualSelection = standaloneController.App.LicenseManager.allowFeature(1);
            anatomyFinder = new AnatomyFinder(standaloneController.AnatomyController, standaloneController.SceneViewController);
            guiManager.addManagedDialog(anatomyFinder);

            options = new OptionsDialog(guiManager);
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);

            //renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer);
            //guiManager.addManagedDialog(renderDialog);

            downloadManagerGUI = new DownloadManagerGUI(standaloneController, downloadServer);

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.setAppIcon("AppButton/WideImage", "AppButton/NarrowImage");

            //Tasks
            selectionModeTask = new SelectionModeTask(standaloneController.AnatomyController);
            selectionModeTask.ShowOnTimelineTaskbar = true;

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            //Patient Section
            taskController.addTask(new ShowPopupTask(chooseSceneDialog, "Medical.NewPatient", "New", "FileToolstrip/ChangeScene", TaskMenuCategories.Patient, 0));

            //System Section
            CallbackTask shopTaskItem = new CallbackTask("Medical.Shop", "Store", "AnomalousMedical/Store", TaskMenuCategories.AnomalousMedical, int.MaxValue - 6, false);
            shopTaskItem.OnClicked += new CallbackTask.ClickedCallback(shopTaskItem_OnClicked);
            taskController.addTask(shopTaskItem);

            downloadsTask = new ShowPopupTask(downloadManagerGUI, "Medical.DownloadManagerGUI", "My Downloads", "AnomalousMedical/Download", TaskMenuCategories.AnomalousMedical, int.MaxValue - 5);
            standaloneController.DownloadController.OpenDownloadGUITask = downloadsTask;
            taskController.addTask(downloadsTask);

            CallbackTask helpTaskItem = new CallbackTask("Medical.Help", "Help", "FileToolstrip/Help", TaskMenuCategories.AnomalousMedical, int.MaxValue - 4, false);
            helpTaskItem.OnClicked += new CallbackTask.ClickedCallback(helpTaskItem_OnClicked);
            taskController.addTask(helpTaskItem);

            taskController.addTask(new ShowPopupTask(options, "Medical.Options", "Options", "FileToolstrip/Options", TaskMenuCategories.System, int.MaxValue - 3));
            taskController.addTask(new DialogOpenTask(aboutDialog, "Medical.About", "About", "FileToolstrip/About", TaskMenuCategories.System, int.MaxValue - 2));

            CallbackTask logoutTaskItem = new CallbackTask("Medical.LogOut", "Log Out", "AnomalousMedical/LogOut", TaskMenuCategories.System, int.MaxValue - 1, false);
            logoutTaskItem.OnClicked += new CallbackTask.ClickedCallback(logoutTaskItem_OnClicked);
            taskController.addTask(logoutTaskItem);

            CallbackTask exitTaskItem = new CallbackTask("Medical.Exit", "Exit", "FileToolstrip/Exit", TaskMenuCategories.System, int.MaxValue, false);
            exitTaskItem.OnClicked += new CallbackTask.ClickedCallback(exitTaskItem_OnClicked);
            taskController.addTask(exitTaskItem);

            //Tools Section
            //taskController.addTask(new MDIDialogOpenTask(renderDialog, "Medical.Render", "Render", "RenderIcon", TaskMenuCategories.Tools));

            //Navigation Section
            MDIDialogOpenTask anatomyFinderTask = new MDIDialogOpenTask(anatomyFinder, "Medical.AnatomyFinder", "Anatomy Finder", "SearchIcon", TaskMenuCategories.Navigation);
            anatomyFinderTask.ShowOnTimelineTaskbar = true;
            taskController.addTask(anatomyFinderTask);
            taskController.addTask(selectionModeTask);
        }

        void shopTaskItem_OnClicked(CallbackTask item)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.AnomalousMedicalStoreURL);
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            anatomyFinder.sceneUnloading();
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            systemMenu = new SystemMenu(menu, this, standaloneController);
        }

        public void sceneRevealed()
        {
            UpdateController.checkForUpdate(updateCheckCompleted, standaloneController.AtlasPluginManager, standaloneController.App.LicenseManager);
        }

        public long PluginId
        {
            get
            {
                return -1;
            }
        }

        public String PluginName
        {
            get
            {
                return "Core";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "AnomalousMedicalCore/BrandingImage";
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }

        public void showOptions()
        {
            options.show(0, 0);
        }

        public void showAboutDialog()
        {
            aboutDialog.open(true);
        }

        public void showChooseSceneDialog()
        {
            chooseSceneDialog.show(0, 0);
        }

        private void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        private void chooseSceneDialog_ChooseScene(object sender, EventArgs e)
        {
            standaloneController.openNewScene(chooseSceneDialog.SelectedFile);
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
            standaloneController.openHelpPage();
        }

        void updateCheckCompleted(bool hasUpdate)
        {
            if (hasUpdate)
            {
                standaloneController.GUIManager.NotificationManager.showTaskNotification("Update(s) Found\nClick Here to Download", downloadsTask.IconName, downloadsTask);
            }
        }

        void MovementSequenceController_GroupAdded(MovementSequenceController controller, MovementSequenceGroup group)
        {
            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(sequencePlayer);

            MDIDialogOpenTask sequencePlayerTask = new MDIDialogOpenTask(sequencePlayer, "Medical.Sequences", "Sequences", "SequenceToolstrip/Sequence", TaskMenuCategories.Tools);
            standaloneController.TaskController.addTask(sequencePlayerTask);

            //We only care about the first one of these events that fires.
            standaloneController.MovementSequenceController.GroupAdded -= MovementSequenceController_GroupAdded;
        }
    }
}
