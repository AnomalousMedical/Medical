using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgrePlugin;
using System.Reflection;
using libRocketPlugin;
using Engine;
using System.Xml;
using Engine.Saving.XMLSaver;
using System.IO;
using Engine.Platform;
using FreeImageAPI;
using Medical.GUI.AnomalousMvc;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
using Medical.Tasks;

namespace Medical.GUI
{
    class AnomalousMainPlugin : AtlasPlugin
    {
        private const int UNKNOWN_GROUP_WEIGHT = int.MaxValue / 2;

        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;
        private AnomalousController bodyAtlasController;
        private DownloadManagerServer downloadServer;
        private ImageLicenseServer imageLicenseServer;
        private SimObjectMover teethMover;

        //Dialogs
        private ChooseSceneDialog chooseSceneDialog;
        private OptionsDialog options;
        private RenderPropertiesDialog renderDialog;
        private AboutDialog aboutDialog;
        private AnatomyFinder anatomyFinder;
        private DownloadManagerGUI downloadManagerGUI;
        private SequencePlayer sequencePlayer = null;
        private BookmarksGUI bookmarks;
        private ViewsGui viewsGui;

        //Controllers
        private BookmarksController bookmarksController;
        private BuyScreenController buyScreens;

        //Taskbar
        private AppButtonTaskbar taskbar;
        private TaskMenu taskMenu;
        private GUITaskManager guiTaskManager;
        private SingleChildChainLink taskbarLink;
        private PremiumFeaturesTaskMenuAd taskMenuAd;

        //Tasks
        private SelectionModeTask selectionModeTask;
        private SelectionOperatorTask selectionOperatorTask;
        private CameraMovementModeTask cameraMovementModeTask;
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

            IDisposableUtil.DisposeIfNotNull(teethMover);
            IDisposableUtil.DisposeIfNotNull(bookmarks);
            IDisposableUtil.DisposeIfNotNull(bookmarksController);
            IDisposableUtil.DisposeIfNotNull(taskMenuAd);
            IDisposableUtil.DisposeIfNotNull(viewsGui);
            downloadServer.Dispose();
            if (selectionModeTask != null)
            {
                selectionModeTask.Dispose();
            }
            if (selectionOperatorTask != null)
            {
                selectionOperatorTask.Dispose();
            }
            if (cameraMovementModeTask != null)
            {
                cameraMovementModeTask.Dispose();
            }
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
            IDisposableUtil.DisposeIfNotNull(buyScreens);
            IDisposableUtil.DisposeIfNotNull(taskbar);
            IDisposableUtil.DisposeIfNotNull(taskMenu);
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.MainPlugin_MyGUI_Skin.xml");
            ResourceManager.Instance.load("Medical.Resources.BodyAtlasImagesets.xml");

            //Wizards
            ResourceManager.Instance.load("Medical.Resources.DistortionWizard.WizardImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(this.GetType().Assembly));

            if (VirtualFileSystem.Instance.exists("Watermark/AnomalousMedical.png"))
            {
                standaloneController.ImageRenderer.LoadLogo = () =>
                    {
                        using (Stream stream = VirtualFileSystem.Instance.openStream("Watermark/AnomalousMedical.png", Engine.Resources.FileMode.Open))
                        {
                            return new FreeImageBitmap(stream);
                        }
                    };
            }

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
            bookmarksController = new BookmarksController(standaloneController, ScaleHelper.Scaled(100), ScaleHelper.Scaled(100), hasPremium);

            //Create Dialogs
            aboutDialog = new AboutDialog(licenseManager);

            chooseSceneDialog = new ChooseSceneDialog(guiManager);
            chooseSceneDialog.ChooseScene += new EventHandler(chooseSceneDialog_ChooseScene);

            standaloneController.AnatomyController.ShowPremiumAnatomyChanged += AnatomyController_ShowPremiumAnatomyChanged;
            anatomyFinder = new AnatomyFinder(standaloneController.AnatomyController, standaloneController.SceneViewController, standaloneController.LayerController, standaloneController.AnatomyTaskManager);
            guiManager.addManagedDialog(anatomyFinder);

            options = new OptionsDialog(guiManager);
            options.VideoOptionsChanged += new EventHandler(options_VideoOptionsChanged);
            options.RequestRestart += new EventHandler(options_RequestRestart);

            renderDialog = new RenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, imageLicenseServer, standaloneController.GUIManager, standaloneController.NotificationManager);
            guiManager.addManagedDialog(renderDialog);

            downloadManagerGUI = new DownloadManagerGUI(standaloneController, downloadServer);

            bookmarks = new BookmarksGUI(bookmarksController, standaloneController.GUIManager, standaloneController.SceneViewController);

            viewsGui = new ViewsGui(standaloneController.SceneViewController, standaloneController.AnatomyController);
            guiManager.addManagedDialog(viewsGui);

            //Taskbar
            taskbar = new AppButtonTaskbar();
            taskbar.OpenTaskMenu += taskbar_OpenTaskMenu;
            taskbar.setAppIcon("AppButton/Hamburger", "AppButton/Hamburger");
            taskbarLink = new SingleChildChainLink(GUILocationNames.Taskbar, taskbar);
            guiManager.addLinkToChain(taskbarLink);
            guiManager.pushRootContainer(GUILocationNames.Taskbar);

            //Task Menu
            taskMenu = new TaskMenu(standaloneController.DocumentController, standaloneController.TaskController, standaloneController.GUIManager, new LayoutElementName(GUILocationNames.FullscreenPopup));
            taskMenu.GroupComparison = TaskMenuCategories.Sorter;
            
            guiTaskManager = new GUITaskManager(taskbar, taskMenu, standaloneController.TaskController);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;
            standaloneController.AnatomyTaskManager.HighlightTasks += AnatomyTaskManager_HighlightTasks;

            //Tasks
            cameraMovementModeTask = new CameraMovementModeTask(standaloneController.SceneViewController);
            taskController.addTask(cameraMovementModeTask);
            Slideshow.AdditionalTasks.addTask(cameraMovementModeTask);

            selectionOperatorTask = new SelectionOperatorTask(standaloneController.AnatomyController);
            taskController.addTask(selectionOperatorTask);
            Slideshow.AdditionalTasks.addTask(selectionOperatorTask);

            var viewsTask = new PinableMDIDialogOpenTask(viewsGui, "Medical.Views", "Views", "AnomalousMedical/ViewIcon", TaskMenuCategories.Explore);
            taskController.addTask(viewsTask);

            //Patient Section
            taskController.addTask(new ShowPopupTask(chooseSceneDialog, "Medical.NewPatient", "New", "AnomalousMedical/ChangeScene", TaskMenuCategories.Explore, 0));

            downloadsTask = new ShowPopupTask(downloadManagerGUI, "Medical.DownloadManagerGUI", "Downloads", "AnomalousMedical/Download", TaskMenuCategories.System, int.MaxValue - 5);
            standaloneController.DownloadController.OpenDownloadGUITask = downloadsTask;
            taskController.addTask(downloadsTask);

            taskController.addTask(new DialogOpenTask(aboutDialog, "Medical.About", "About", "AnomalousMedical/About", TaskMenuCategories.System, int.MaxValue - 2));
            taskController.addTask(new VolumeControlTask());

            CallbackTask unhideAllAnatomy = new CallbackTask("Medical.UnhideAllAnatomy", "Unhide All", "AnatomyFinder.ShowAll", TaskMenuCategories.Explore, int.MaxValue - 2, false, (item) =>
            {
                LayerState undo = LayerState.CreateAndCapture();
                standaloneController.LayerController.unhideAll();
                standaloneController.LayerController.pushUndoState(undo);
            });
            taskController.addTask(unhideAllAnatomy);

            //Navigation Section
            PinableMDIDialogOpenTask anatomyFinderTask = new PinableMDIDialogOpenTask(anatomyFinder, "Medical.AnatomyFinder", "Anatomy Finder", "AnomalousMedical/SearchIcon", TaskMenuCategories.Explore);
            taskController.addTask(anatomyFinderTask);
            Slideshow.AdditionalTasks.addTask(anatomyFinderTask);

            ShowPopupTask bookmarkTask = null;

            standaloneController.AnatomyController.setCommandPermission(AnatomyCommandPermissions.Unrestricted, PlatformConfig.UnrestrictedEnvironment);
            standaloneController.AnatomyController.setCommandPermission(AnatomyCommandPermissions.PremiumActive, hasPremium);

            if (PlatformConfig.UnrestrictedEnvironment || hasPremium)
            {
                //Explore
                selectionModeTask = new SelectionModeTask(standaloneController.AnatomyController);
                taskController.addTask(selectionModeTask);
                Slideshow.AdditionalTasks.addTask(selectionModeTask);

                bookmarkTask = new ShowPopupTask(bookmarks, "Medical.Bookmarks", "Bookmarks", "AnomalousMedical/FavoritesIcon", TaskMenuCategories.Explore);
                taskController.addTask(bookmarkTask);
                Slideshow.AdditionalTasks.addTask(bookmarkTask);
            }

            if (PlatformConfig.UnrestrictedEnvironment)
            {
                //System
                CallbackTask shopTaskItem = new CallbackTask("Medical.Shop", "Store", "AnomalousMedical/Store", TaskMenuCategories.System, int.MaxValue - 6, false);
                shopTaskItem.OnClicked += new CallbackTask.ClickedCallback(shopTaskItem_OnClicked);
                taskController.addTask(shopTaskItem);

                CallbackTask blogTaskItem = new CallbackTask("Medical.Blog", "Blog", "AnomalousMedical/Blog", TaskMenuCategories.System, int.MaxValue - 7, false);
                blogTaskItem.OnClicked += new CallbackTask.ClickedCallback(blogTaskItem_OnClicked);
                taskController.addTask(blogTaskItem);

                CallbackTask helpTaskItem = new CallbackTask("Medical.Help", "Help", "AnomalousMedical/Help", TaskMenuCategories.System, int.MaxValue - 4, false);
                helpTaskItem.OnClicked += new CallbackTask.ClickedCallback(helpTaskItem_OnClicked);
                taskController.addTask(helpTaskItem);

                taskController.addTask(new ShowPopupTask(options, "Medical.Options", "Options", "AnomalousMedical/Options", TaskMenuCategories.System, int.MaxValue - 3));

                CallbackTask logoutTaskItem = new CallbackTask("Medical.LogOut", "Log Out", "AnomalousMedical/LogOut", TaskMenuCategories.System, int.MaxValue - 1, false);
                logoutTaskItem.OnClicked += new CallbackTask.ClickedCallback(logoutTaskItem_OnClicked);
                taskController.addTask(logoutTaskItem);

                CallbackTask exitTaskItem = new CallbackTask("Medical.Exit", "Exit", "AnomalousMedical/Exit", TaskMenuCategories.System, int.MaxValue, false);
                exitTaskItem.OnClicked += new CallbackTask.ClickedCallback(exitTaskItem_OnClicked);
                taskController.addTask(exitTaskItem);

                //Tools Section
                MDIDialogOpenTask renderTask = new MDIDialogOpenTask(renderDialog, "Medical.Render", "Render", "AnomalousMedical/RenderIcon", TaskMenuCategories.Create);
                taskController.addTask(renderTask);
            }

            if(PlatformConfig.AllowFullscreenToggle)
            {
                CallbackTask toggleFullscreen = new CallbackTask("Medical.ToggleFullscreen", "Toggle Fullscreen", "AnomalousMedical/ToggleFullscreen", TaskMenuCategories.System, int.MaxValue - 2, false, (item) =>
                {
                    MainWindow.Instance.toggleFullscreen();
                });
                taskController.addTask(toggleFullscreen);

                //Fullscreen Toggle Shortcut
                var toggleFullscreenMessageEvent = new ButtonEvent(EventLayers.Gui, frameUp: (evtMgr) =>
                {
                    MainWindow.Instance.toggleFullscreen();
                });
                toggleFullscreenMessageEvent.addButton(KeyboardButtonCode.KC_RETURN);
                toggleFullscreenMessageEvent.addButton(KeyboardButtonCode.KC_LMENU);
                standaloneController.MedicalController.EventManager.addEvent(toggleFullscreenMessageEvent);
            }

            //Premium / Non Premium
            if (!hasPremium)
            {
                if (PlatformConfig.UnrestrictedEnvironment)
                {
                    buyScreens = new BuyScreenController(standaloneController);
                    taskMenuAd = new PremiumFeaturesTaskMenuAd(taskMenu);
                    selectionModeTask.SelectionModeChooser.ShowBuyMessage += SelectionModeChooser_ShowBuyMessage;
                    anatomyFinder.ShowBuyMessage += anatomyFinder_ShowBuyMessage;
                    bookmarks.ShowBuyMessage += bookmarks_ShowBuyMessage;
                }

                if (MedicalConfig.FirstRun)
                {
                    guiTaskManager.addPinnedTask(anatomyFinderTask);
                    guiTaskManager.addPinnedTask(viewsTask);
                    guiTaskManager.addPinnedTask(cameraMovementModeTask);
                    if (bookmarkTask != null)
                    {
                        guiTaskManager.addPinnedTask(bookmarkTask);
                    }
                    guiTaskManager.addPinnedTask(unhideAllAnatomy);
                }
            }

            standaloneController.AtlasPluginManager.RequestDependencyDownload += AtlasPluginManager_RequestDependencyDownload;

            //Teeth mover
            teethMover = new SimObjectMover("Teeth", standaloneController.MedicalController.PluginManager.RendererPlugin, standaloneController.MedicalController.EventManager, standaloneController.SceneViewController);
            TeethToolController teethToolController = new TeethToolController(teethMover);
            standaloneController.ImageRenderer.ImageRenderStarted += teethToolController.ScreenshotRenderStarted;
            standaloneController.ImageRenderer.ImageRenderCompleted += teethToolController.ScreenshotRenderCompleted;

            standaloneController.ViewHostFactory.addFactory(new WizardComponentFactory(teethToolController));
        }

        private void AnatomyTaskManager_HighlightTasks(string arg1, IEnumerable<Task> arg2)
        {
            if (guiManager.MainGuiShowing)
            {
                taskMenu.setHighlightTasks(arg1, arg2);
                taskMenu.show(0, 0);
            }
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        public void sceneLoaded(SimScene scene)
        {
            teethMover.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            teethMover.sceneUnloading(scene);
            anatomyFinder.sceneUnloading();
        }

        public void setupUserBookmarks(bool instantBookmarkApply)
        {
            ResourceProvider bookmarksResourceProvider;
            if (bookmarksController.PremiumBookmarks)
            {
                bookmarksResourceProvider = createPremiumBookmarksResourceProvider();
            }
            else
            {
                bookmarksResourceProvider = createNonPremiumBookmarksResourceProvider();
            }
            bookmarksController.loadSavedBookmarks(bookmarksResourceProvider);

            var camera = standaloneController.SceneViewController.ActiveWindow;
            if (camera != null)
            {
                var bmk = bookmarksController.loadBookmark(String.Format("Cameras/{0}.bmk", camera.CurrentTransparencyState));
                if (bmk != null)
                {
                    if (instantBookmarkApply)
                    {
                        camera.setPosition(bmk.CameraPosition, 0.0f);
                        TransparencyController.ActiveTransparencyState = camera.CurrentTransparencyState;
                        bmk.Layers.instantlyApply();
                    }
                    else
                    {
                        bookmarksController.applyBookmark(bmk);
                    }
                }
            }
        }

        public void allPluginsLoaded()
        {
            if (standaloneController.AtlasPluginManager.addPlugin("Plugins/IntroductionTutorial/", true))//Ok to force to true since it comes from our main data file.
            {
                //Setup intro tutorial startup task
                standaloneController.TaskController.addTask(new RunIntroTutorial(standaloneController));
            }

            UpdateController.checkForUpdate(updateCheckCompleted, standaloneController.AtlasPluginManager, standaloneController.LicenseManager);

            if (!String.IsNullOrEmpty(MedicalConfig.StartupTask))
            {
                Task commandLineTask = standaloneController.TaskController.getTask(MedicalConfig.StartupTask);
                if (commandLineTask != null)
                {
                    commandLineTask.clicked(EmptyTaskPositioner.Instance);
                }
                else
                {
                    standaloneController.NotificationManager.showNotification(String.Format("Cannot load command line supplied task '{0}'\nPlease make sure it is named correctly and that you own this plugin.", MedicalConfig.StartupTask), MessageBoxIcons.Error);
                }
            }
            else if (MedicalConfig.FirstRun)
            {
                MedicalConfig.FirstRun = false;
            }

            guiTaskManager.setLoadingTasksToMissing(CommonResources.NoIcon);
            downloadManagerGUI.activateServerDownloadChecks();
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

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
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
            standaloneController.restartWithWarning(null, false, false);
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
                        standaloneController.LicenseManager.deleteLicense();
                        standaloneController.exit();
                    }
                });
        }

        void exitTaskItem_OnClicked(Task item)
        {
            standaloneController.exit();
        }

        void updateCheckCompleted(UpdateController.UpdateCheckResult result)
        {
            if (result > 0)
            {
                standaloneController.NotificationManager.showTaskNotification("Update(s) Found\nClick Here to Download", downloadsTask.IconName, downloadsTask);
            }
            if (!standaloneController.AtlasPluginManager.allDependenciesLoaded())
            {
                standaloneController.NotificationManager.showTaskNotification("Additional Files Needed\nClick Here to Download", downloadsTask.IconName, downloadsTask);
            }
        }

        void MovementSequenceController_GroupAdded(MovementSequenceController controller, MovementSequenceGroup group)
        {
            sequencePlayer = new SequencePlayer(standaloneController.MovementSequenceController, standaloneController.MusclePositionController);
            guiManager.addManagedDialog(sequencePlayer);

            PinableMDIDialogOpenTask sequencePlayerTask = new PinableMDIDialogOpenTask(sequencePlayer, "Medical.Sequences", "Sequences", "SequenceToolstrip/Sequence", TaskMenuCategories.Explore);
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

        void AnatomyController_ShowPremiumAnatomyChanged(AnatomyController source, bool isPremium)
        {
            standaloneController.AnatomyController.setCommandPermission(AnatomyCommandPermissions.PremiumActive, isPremium);
            bookmarksController.PremiumBookmarks = isPremium;
            if (isPremium)
            {
                bookmarksController.clearBookmarks();
                bookmarksController.clearBookmarkPaths();
                //Save the demo bookmarks so the user does not feel like they "lost" them.
                //This isn't the most efficient way, but it respects the source and destination data
                //that is setup somewhere else. Also its only really going to copy 5 or 6 things one
                //time for most users.
                var oldBookmarksResourceProvider = createNonPremiumBookmarksResourceProvider();
                var newBookmarksResourceProvider = createPremiumBookmarksResourceProvider();
                if (!newBookmarksResourceProvider.listFiles("*.bmk").Any())
                {
                    foreach (String file in oldBookmarksResourceProvider.listFiles("*.bmk"))
                    {
                        using (Stream stream = oldBookmarksResourceProvider.openFile(file))
                        {
                            using (Stream write = newBookmarksResourceProvider.openWriteStream(Path.GetFileName(file)))
                            {
                                stream.CopyTo(write);
                            }
                        }
                    }
                }
                bookmarksController.loadSavedBookmarks(newBookmarksResourceProvider);

                //Turn off the ad
                if (taskMenuAd != null)
                {
                    taskMenuAd.Dispose();
                    taskMenuAd = null;
                }
            }
        }

        void bookmarks_ShowBuyMessage()
        {
            buyScreens.showScreen(BuyScreens.Bookmarks);
        }

        void anatomyFinder_ShowBuyMessage()
        {
            buyScreens.showScreen(BuyScreens.AnatomyFinder);
        }

        void SelectionModeChooser_ShowBuyMessage()
        {
            buyScreens.showScreen(BuyScreens.SelectionMode);
        }

        void AtlasPluginManager_RequestDependencyDownload(AtlasPlugin obj)
        {
            downloadManagerGUI.addAutoDownloadItems(obj.DependencyPluginIds);
            var downloadGUITask = standaloneController.DownloadController.OpenDownloadGUITask;
            if (downloadGUITask != null)
            {
                downloadGUITask.clicked(EmptyTaskPositioner.Instance);
            }
        }

        void blogTaskItem_OnClicked(CallbackTask item)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.AnomalousMedicalBlogURL);
        }

        void shopTaskItem_OnClicked(CallbackTask item)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.AnomalousMedicalStoreURL);
        }

        void helpTaskItem_OnClicked(Task item)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.HelpURL);
        }

        private ResourceProvider createNonPremiumBookmarksResourceProvider()
        {
            return new EmbeddedResourceProvider(this.GetType().Assembly, "Medical.Resources.Bookmarks.");
        }

        private static ResourceProvider createPremiumBookmarksResourceProvider()
        {
            ResourceProvider bookmarksResourceProvider;
            if (!Directory.Exists(MedicalConfig.BookmarksFolder))
            {
                Directory.CreateDirectory(MedicalConfig.BookmarksFolder);
            }
            bookmarksResourceProvider = new FilesystemResourceProvider(MedicalConfig.BookmarksFolder);
            return bookmarksResourceProvider;
        }
    }
}
