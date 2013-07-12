using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine.Platform;
using Medical.Controller;
using Engine;
using Logging;
using System.IO;

namespace Medical.GUI
{
    public class GUIManager : IDisposable
    {
        private static String INFO_SECTION = "__Info_Section_Reserved__";
        private static String INFO_VERSION = "Version";

        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;
        private HorizontalPopoutLayoutContainer leftAnimatedContainer;
        private HorizontalPopoutLayoutContainer rightAnimatedContainer;
        private VerticalPopoutLayoutContainer topAnimatedContainer;
        private VerticalPopoutLayoutContainer bottomAnimatedContainer;

        private Taskbar taskbar;
        private TaskMenu taskMenu;
        private BorderLayoutContainer innerBorderLayout;

        private bool mainGuiShowing = true;

        private bool saveWindowsOnExit = true;

        //Dialogs
        private DialogManager dialogManager;

        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIImageDisplayFactory imageDisplayFactory;
        private MyGUITextDisplayFactory textDisplayFactory;
        private List<LayoutContainer> fullscreenPopups = new List<LayoutContainer>();
        private MyGUIImageRendererProgress imageRendererProgress;
        private NotificationGUIManager notificationManager;

        //Helper classes
        GUITaskManager guiTaskManager;

        //Events
        public event Action MainGUIShown;
        public event Action MainGUIHidden;

        public GUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
        }

        public void Dispose()
        {
            //Dialogs
            if (saveWindowsOnExit && MedicalConfig.WindowsFile != null)
            {
                ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
                ConfigSection infoSection = configFile.createOrRetrieveConfigSection(INFO_SECTION);
                infoSection.setValue(INFO_VERSION, this.GetType().Assembly.GetName().Version.ToString());
                dialogManager.saveDialogLayout(configFile);
                guiTaskManager.savePinnedTasks(configFile);
                configFile.writeConfigFile();
            }
            else
            {
                Log.Warning("Could not save window configuration because the WindowsFile is not defined.");
            }
            dialogManager.DisposeIfNotNull();

            //Containers
            leftAnimatedContainer.DisposeIfNotNull();
            rightAnimatedContainer.DisposeIfNotNull();
            topAnimatedContainer.DisposeIfNotNull();
            bottomAnimatedContainer.DisposeIfNotNull();

            //Other
            imageRendererProgress.DisposeIfNotNull();
            continuePrompt.DisposeIfNotNull();
            taskMenu.DisposeIfNotNull();
            taskbar.DisposeIfNotNull();
            notificationManager.DisposeIfNotNull();
        }

        public void createGUI(MDILayoutManager mdiManager)
        {
            Gui gui = Gui.Instance;

            OgreResourceGroupManager.getInstance().addResourceLocation(typeof(GUIManager).AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);

            screenLayoutManager = new ScreenLayoutManager(standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);
            screenLayoutManager.ScreenSizeChanged += new ScreenSizeChanged(screenLayoutManager_ScreenSizeChanged);

            innerBorderLayout = new BorderLayoutContainer();
            innerBorderLayout.Center = mdiManager;

            //Dialogs
            dialogManager = new DialogManager(mdiManager);

            //Taskbar
            taskbar = new Taskbar(standaloneController);
            taskbar.SuppressLayout = true;
            taskbar.OpenTaskMenu += new GUI.Taskbar.OpenTaskMenuEvent(taskbar_OpenTaskMenu);
            taskbar.AlignmentChanged += new Action<GUI.Taskbar>(taskbar_AlignmentChanged);

            taskbar.Child = innerBorderLayout;
            screenLayoutManager.Root = taskbar;

            notificationManager = new NotificationGUIManager(taskbar, standaloneController);

            //Task Menu
            taskMenu = new TaskMenu(standaloneController.DocumentController, standaloneController.TaskController);

            guiTaskManager = new GUITaskManager(taskbar, taskMenu, standaloneController.TaskController);

            topAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Top = topAnimatedContainer;

            leftAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Left = leftAnimatedContainer;

            bottomAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Bottom = bottomAnimatedContainer;

            rightAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Right = rightAnimatedContainer;

            screenLayoutManager.Root.SuppressLayout = false;

            imageRendererProgress = new MyGUIImageRendererProgress();
            standaloneController.ImageRenderer.ImageRendererProgress = imageRendererProgress;

            taskbar.SuppressLayout = false;
            taskbar.layout();

            continuePrompt = new MyGUIContinuePromptProvider();

            imageDisplayFactory = new MyGUIImageDisplayFactory(standaloneController.SceneViewController);
            textDisplayFactory = new MyGUITextDisplayFactory(standaloneController.SceneViewController);
        }

        public void giveGUIsToTimelineController(TimelineController timelineController)
        {
            timelineController.ContinuePrompt = continuePrompt;
            timelineController.ImageDisplayFactory = imageDisplayFactory;
            timelineController.TextDisplayFactory = textDisplayFactory;
        }

        public void windowChanged(OSWindow newWindow)
        {
            screenLayoutManager.changeOSWindow(newWindow);
        }

        public void changeTopPanel(LayoutContainer topContainer, AnimationCompletedDelegate animationCompleted = null)
        {
            if (topContainer != null)
            {
                topContainer.Visible = true;
                topContainer.bringToFront();
            }
            topAnimatedContainer.changePanel(topContainer, 0.25f, animationCompleted);
        }

        public void changeLeftPanel(LayoutContainer leftContainer, AnimationCompletedDelegate animationCompleted = null)
        {
            if (leftContainer != null)
            {
                leftContainer.Visible = true;
                leftContainer.bringToFront();
            }
            if (leftAnimatedContainer.CurrentContainer != leftContainer)
            {
                leftAnimatedContainer.changePanel(leftContainer, 0.25f, animationCompleted);
            }
        }

        public void changeRightPanel(LayoutContainer rightContainer, AnimationCompletedDelegate animationCompleted = null)
        {
            if (rightContainer != null)
            {
                rightContainer.Visible = true;
                rightContainer.bringToFront();
            }
            if (rightAnimatedContainer.CurrentContainer != rightContainer)
            {
                rightAnimatedContainer.changePanel(rightContainer, 0.25f, animationCompleted);
            }
        }

        public void changeBottomPanel(LayoutContainer bottomContainer, AnimationCompletedDelegate animationCompleted = null)
        {
            if (bottomContainer != null)
            {
                bottomContainer.Visible = true;
                bottomContainer.bringToFront();
            }
            bottomAnimatedContainer.changePanel(bottomContainer, 0.25f, animationCompleted);
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            if (mainGuiShowing != enabled)
            {
                taskbar.SuppressLayout = true;
                standaloneController.AtlasPluginManager.setMainInterfaceEnabled(enabled);
                if (enabled)
                {
                    if (!taskbar.Visible)
                    {
                        taskbar.Visible = true;
                        dialogManager.reopenMainGUIDialogs();
                        notificationManager.reshowAllNotifications();
                        if (MainGUIShown != null)
                        {
                            MainGUIShown.Invoke();
                        }
                    }
                }
                else
                {
                    taskbar.Visible = false;
                    dialogManager.closeMainGUIDialogs();
                    notificationManager.hideAllNotifications();
                    if (MainGUIHidden != null)
                    {
                        MainGUIHidden.Invoke();
                    }
                }
                mainGuiShowing = enabled;
                taskbar.SuppressLayout = false;
                taskbar.layout();
            }
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
        }

        public void addManagedDialog(MDIDialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
        }

        public void removeManagedDialog(MDIDialog dialog)
        {
            dialogManager.removeManagedDialog(dialog);
        }

        public void addFullscreenPopup(LayoutContainer popup)
        {
            popup.Location = new IntVector2((int)innerBorderLayout.Location.x, (int)innerBorderLayout.Location.y);
            popup.WorkingSize = new IntSize2((int)innerBorderLayout.WorkingSize.Width, (int)innerBorderLayout.WorkingSize.Height);
            popup.layout();
            fullscreenPopups.Add(popup);
        }

        public void removeFullscreenPopup(LayoutContainer popup)
        {
            fullscreenPopups.Remove(popup);
        }

        public void autoDisposeDialog(MDIDialog autoDisposeDialog)
        {
            dialogManager.autoDisposeDialog(autoDisposeDialog);
        }

        /// <summary>
        /// Delete the windows file and tell the manager to not write a new one on close.
        /// </summary>
        /// <returns>True if the file was deleted. False otherwise.</returns>
        public bool deleteWindowsFile()
        {
            if (MedicalConfig.WindowsFile != null)
            {
                try
                {
                    File.Delete(MedicalConfig.WindowsFile);
                    saveWindowsOnExit = false;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Could not delete windows file. Reason: {0}", ex.Message);
                }
            }
            return false;
        }

        public Taskbar Taskbar
        {
            get
            {
                return taskbar;
            }
        }

        public TaskMenu TaskMenu
        {
            get
            {
                return taskMenu;
            }
        }

        public NotificationGUIManager NotificationManager
        {
            get
            {
                return notificationManager;
            }
        }

        internal void loadSavedUI()
        {
            taskbar.SuppressLayout = true;
            ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
            configFile.loadConfigFile();
            ConfigSection infoSection = configFile.createOrRetrieveConfigSection(INFO_SECTION);
            String versionString = infoSection.getValue(INFO_VERSION, "0.0.0.0");
            Version version;
            try
            {
                version = new Version(versionString);
            }
            catch (Exception)
            {
                version = new Version("0.0.0.0");
            }
            if (version > new Version("1.0.0.2818"))
            {
                dialogManager.loadDialogLayout(configFile);
            }
            guiTaskManager.loadPinnedTasks(configFile);
            taskbar.SuppressLayout = false;
            taskbar.layout();
        }

        private void screenLayoutManager_ScreenSizeChanged(int width, int height)
        {
            dialogManager.windowResized();
            continuePrompt.ensureVisible();
            int xPos = (int)innerBorderLayout.Location.x;
            int yPos = (int)innerBorderLayout.Location.y;
            int innerWidth = (int)innerBorderLayout.WorkingSize.Width;
            int innerHeight = (int)innerBorderLayout.WorkingSize.Height;
            if (taskMenu.Visible)
            {
                taskMenu.setPosition(xPos, yPos);
                taskMenu.setSize(innerWidth, innerHeight);
            }
            foreach (LayoutContainer fullscreenPopup in fullscreenPopups)
            {
                fullscreenPopup.Location = new IntVector2(xPos, yPos);
                fullscreenPopup.WorkingSize = new IntSize2(innerWidth, innerHeight);
                fullscreenPopup.layout();
            }
            notificationManager.screenSizeChanged();
        }

        public event ScreenSizeChanged ScreenSizeChanged
        {
            add
            {
                screenLayoutManager.ScreenSizeChanged += value;
            }
            remove
            {
                screenLayoutManager.ScreenSizeChanged -= value;
            }
        }

        void taskbar_OpenTaskMenu(int left, int top, int width, int height)
        {
            taskMenu.setSize(width, height);
            taskMenu.show(left, top);
        }

        void taskbar_AlignmentChanged(Taskbar obj)
        {
            notificationManager.screenSizeChanged();
        }
    }
}
