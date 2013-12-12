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
using libRocketPlugin;
using Engine;

namespace Medical.GUI
{
    class AnomalousMainPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;
        private AnomalousController bodyAtlasController;
        private DownloadManagerServer downloadServer;
        private ImageLicenseServer imageLicenseServer;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private AboutDialog aboutDialog;
        private AnatomyFinder anatomyFinder;
        private DownloadManagerGUI downloadManagerGUI;
        private SequencePlayer sequencePlayer = null;

        //Taskbar
        private AppButtonTaskbar taskbar;
        private TaskMenu taskMenu;
        private GUITaskManager guiTaskManager;
        private SingleChildChainLink taskbarLink;

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
            guiManager.SaveUIConfiguration -= guiManager_SaveUIConfiguration;
            guiManager.LoadUIConfiguration -= guiManager_LoadUIConfiguration;
            guiManager.MainGUIShown -= guiManager_MainGUIShown;
            guiManager.MainGUIHidden -= guiManager_MainGUIHidden;

            downloadServer.Dispose();
            selectionModeTask.Dispose();
            renderDialog.Dispose();
            options.Dispose();
            anatomyFinder.Dispose();
            chooseSceneDialog.Dispose();
            aboutDialog.Dispose();
            if (sequencePlayer != null)
            {
                sequencePlayer.Dispose();
            }
            guiManager.removeLinkFromChain(taskbarLink);
            IDisposableUtil.DisposeIfNotNull(taskbar);
            IDisposableUtil.DisposeIfNotNull(taskMenu);
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.BodyAtlasImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(this.GetType().Assembly));

            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;
            standaloneController.MovementSequenceController.GroupAdded += MovementSequenceController_GroupAdded;

            bool hasPremium = licenseManager.allowFeature(1);
            standaloneController.AnatomyController.ShowPremiumAnatomy = hasPremium;
            guiManager.SaveUIConfiguration += guiManager_SaveUIConfiguration;
            guiManager.LoadUIConfiguration += guiManager_LoadUIConfiguration;
            guiManager.MainGUIShown += guiManager_MainGUIShown;
            guiManager.MainGUIHidden += guiManager_MainGUIHidden;

            //Controllers
            downloadServer = new DownloadManagerServer(licenseManager);
            imageLicenseServer = new ImageLicenseServer(licenseManager);

            //Create Dialogs
            aboutDialog = new AboutDialog(licenseManager);

            chooseSceneDialog = new ChooseSceneDialog(guiManager);
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            anatomyFinder = new AnatomyFinder(standaloneController.AnatomyController, standaloneController.SceneViewController, standaloneController.MedicalController.EventManager);
            guiManager.addManagedDialog(anatomyFinder);

            options = new OptionsDialog(guiManager);
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);
            options.RequestRestart += new EventHandler(options_RequestRestart);

            renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, imageLicenseServer, standaloneController.GUIManager, standaloneController.NotificationManager);
            guiManager.addManagedDialog(renderDialog);

            downloadManagerGUI = new DownloadManagerGUI(standaloneController, downloadServer);

            //Taskbar
            taskbar = new AppButtonTaskbar();
            taskbar.OpenTaskMenu += taskbar_OpenTaskMenu;
            taskbar.setAppIcon("AppButton/WideImage", "AppButton/NarrowImage");
            taskbarLink = new SingleChildChainLink(GUILocationNames.Taskbar, taskbar);
            guiManager.addLinkToChain(taskbarLink);
            guiManager.pushRootContainer(GUILocationNames.Taskbar);

            //Task Menu
            taskMenu = new TaskMenu(standaloneController.DocumentController, standaloneController.TaskController, standaloneController.GUIManager);
            if (!hasPremium)
            {
                taskMenu.AdImageKey = "AnomalousMedical/PremiumAd";
            }
            taskMenu.ShowAdImage = !hasPremium;
            taskMenu.AdImageUrl = MedicalConfig.DefaultAdUrl;

            guiTaskManager = new GUITaskManager(taskbar, taskMenu, standaloneController.TaskController);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            //Tasks
            selectionModeTask = new SelectionModeTask(standaloneController.AnatomyController);
            selectionModeTask.ShowOnTimelineTaskbar = true;
            taskController.addTask(selectionModeTask);
            Slideshow.AdditionalTasks.addTask(selectionModeTask);

            //Patient Section
            taskController.addTask(new ShowPopupTask(chooseSceneDialog, "Medical.NewPatient", "New", "AnomalousMedical/ChangeScene", TaskMenuCategories.Patient, 0));

            //System Section
            CallbackTask shopTaskItem = new CallbackTask("Medical.Shop", "Store", "AnomalousMedical/Store", TaskMenuCategories.AnomalousMedical, int.MaxValue - 6, false);
            shopTaskItem.OnClicked += new CallbackTask.ClickedCallback(shopTaskItem_OnClicked);
            taskController.addTask(shopTaskItem);

            CallbackTask blogTaskItem = new CallbackTask("Medical.Blog", "Blog", "StandaloneIcons/Blog", TaskMenuCategories.AnomalousMedical, int.MaxValue - 7, false);
            blogTaskItem.OnClicked += new CallbackTask.ClickedCallback(blogTaskItem_OnClicked);
            taskController.addTask(blogTaskItem);

            downloadsTask = new ShowPopupTask(downloadManagerGUI, "Medical.DownloadManagerGUI", "My Downloads", "AnomalousMedical/Download", TaskMenuCategories.AnomalousMedical, int.MaxValue - 5);
            standaloneController.DownloadController.OpenDownloadGUITask = downloadsTask;
            taskController.addTask(downloadsTask);

            CallbackTask helpTaskItem = new CallbackTask("Medical.Help", "Help", "AnomalousMedical/Help", TaskMenuCategories.AnomalousMedical, int.MaxValue - 4, false);
            helpTaskItem.OnClicked += new CallbackTask.ClickedCallback(helpTaskItem_OnClicked);
            taskController.addTask(helpTaskItem);

            taskController.addTask(new ShowPopupTask(options, "Medical.Options", "Options", "AnomalousMedical/Options", TaskMenuCategories.System, int.MaxValue - 3));
            taskController.addTask(new DialogOpenTask(aboutDialog, "Medical.About", "About", "AnomalousMedical/About", TaskMenuCategories.System, int.MaxValue - 2));
            taskController.addTask(new VolumeControlTask());

            CallbackTask logoutTaskItem = new CallbackTask("Medical.LogOut", "Log Out", "AnomalousMedical/LogOut", TaskMenuCategories.System, int.MaxValue - 1, false);
            logoutTaskItem.OnClicked += new CallbackTask.ClickedCallback(logoutTaskItem_OnClicked);
            taskController.addTask(logoutTaskItem);

            CallbackTask exitTaskItem = new CallbackTask("Medical.Exit", "Exit", "AnomalousMedical/Exit", TaskMenuCategories.System, int.MaxValue, false);
            exitTaskItem.OnClicked += new CallbackTask.ClickedCallback(exitTaskItem_OnClicked);
            taskController.addTask(exitTaskItem);

            //Tools Section
            MDIDialogOpenTask renderTask = new MDIDialogOpenTask(renderDialog, "Medical.Render", "Render", "AnomalousMedical/RenderIcon", TaskMenuCategories.Tools);
            renderTask.ShowOnTimelineTaskbar = true;
            taskController.addTask(renderTask);

            //Navigation Section
            MDIDialogOpenTask anatomyFinderTask = new MDIDialogOpenTask(anatomyFinder, "Medical.AnatomyFinder", "Anatomy Finder", "AnomalousMedical/SearchIcon", TaskMenuCategories.Navigation);
            anatomyFinderTask.ShowOnTimelineTaskbar = true;
            taskController.addTask(anatomyFinderTask);
            Slideshow.AdditionalTasks.addTask(anatomyFinderTask);
        }

        void blogTaskItem_OnClicked(CallbackTask item)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.AnomalousMedicalBlogURL);
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

        public void sceneRevealed()
        {
            UpdateController.checkForUpdate(updateCheckCompleted, standaloneController.AtlasPluginManager, standaloneController.App.LicenseManager);

            if (!String.IsNullOrEmpty(MedicalConfig.StartupTask))
            {
                Task commandLineTask = standaloneController.TaskController.getTask(MedicalConfig.StartupTask);
                if (commandLineTask != null)
                {
                    commandLineTask.clicked(null);
                }
                else
                {
                    standaloneController.NotificationManager.showNotification(String.Format("Cannot load command line supplied task '{0}'\nPlease make sure it is named correctly and that you own this plugin.", MedicalConfig.StartupTask), MessageBoxIcons.Error);
                }
            }
            else if (MedicalConfig.FirstRun)
            {
                MedicalConfig.FirstRun = false;
                Task introTask = standaloneController.TaskController.getTask("DDPlugin.IntroductionTutorial.Task");
                if (introTask != null)
                {
                    introTask.clicked(null);
                }
            }
        }

        public long PluginId
        {
            get
            {
                return 0;
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

        public bool AllowUninstall
        {
            get
            {
                return false;
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

        void options_RequestRestart(object sender, EventArgs e)
        {
            standaloneController.restartWithWarning();
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
                standaloneController.NotificationManager.showTaskNotification("Update(s) Found\nClick Here to Download", downloadsTask.IconName, downloadsTask);
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

        void taskbar_OpenTaskMenu(int left, int top, int width, int height)
        {
            taskMenu.setSize(width, height);
            taskMenu.show(left, top);
        }

        void guiManager_SaveUIConfiguration(ConfigFile configFile)
        {
            guiTaskManager.savePinnedTasks(configFile);
        }

        void guiManager_LoadUIConfiguration(ConfigFile configFile)
        {
            guiTaskManager.loadPinnedTasks(configFile);
        }

        void guiManager_MainGUIHidden()
        {
            guiManager.deactivateLink(taskbarLink.Name);
            taskbar.Visible = false;
        }

        void guiManager_MainGUIShown()
        {
            taskbar.Visible = true;
            guiManager.pushRootContainer(taskbarLink.Name);
        }
    }
}
