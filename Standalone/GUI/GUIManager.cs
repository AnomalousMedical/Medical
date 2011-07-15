using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine.Platform;
using Medical.Controller;

namespace Medical.GUI
{
    public class GUIManager : IDisposable
    {
        private ScreenLayoutManager screenLayoutManager;
        private StandaloneController standaloneController;
        private HorizontalPopoutLayoutContainer leftAnimatedContainer;
        private HorizontalPopoutLayoutContainer rightAnimatedContainer;
        private VerticalPopoutLayoutContainer topAnimatedContainer;
        private VerticalPopoutLayoutContainer bottomAnimatedContainer;

        private Taskbar taskbar;
        private TaskMenu taskMenu;
        private BorderLayoutContainer innerBorderLayout;

        //Dialogs
        private DialogManager dialogManager;

        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIQuestionProvider questionProvider;
        private MyGUIImageDisplayFactory imageDisplayFactory;

        public GUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
        }

        public void Dispose()
        {
            //Dialogs
            dialogManager.saveDialogLayout(MedicalConfig.WindowsFile);

            //Other
            questionProvider.Dispose();
            continuePrompt.Dispose();
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            taskMenu.Dispose();
            MedicalConfig.TaskbarAlignment = taskbar.Alignment;
            taskbar.Dispose();
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
            taskbar.Alignment = MedicalConfig.TaskbarAlignment;
            taskbar.SuppressLayout = true;
            taskbar.OpenTaskMenu += new GUI.Taskbar.OpenTaskMenuEvent(taskbar_OpenTaskMenu);

            taskbar.Child = innerBorderLayout;
            screenLayoutManager.Root = taskbar;

            //Task Menu
            taskMenu = new TaskMenu(standaloneController.DocumentController);
            taskMenu.TaskItemOpened += new TaskItemDelegate(taskMenu_TaskItemOpened);

            topAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Top = topAnimatedContainer;

            leftAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Left = leftAnimatedContainer;

            bottomAnimatedContainer = new VerticalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Bottom = bottomAnimatedContainer;

            rightAnimatedContainer = new HorizontalPopoutLayoutContainer(standaloneController.MedicalController.MainTimer);
            innerBorderLayout.Right = rightAnimatedContainer;

            screenLayoutManager.Root.SuppressLayout = false;

            standaloneController.ImageRenderer.ImageRendererProgress = new MyGUIImageRendererProgress();

            taskbar.SuppressLayout = false;
            taskbar.layout();

            continuePrompt = new MyGUIContinuePromptProvider();
            questionProvider = new MyGUIQuestionProvider(this);

            imageDisplayFactory = new MyGUIImageDisplayFactory();
            standaloneController.MedicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(imageDisplayFactory);
        }

        public void giveGUIsToTimelineController(TimelineController timelineController)
        {
            timelineController.ContinuePrompt = continuePrompt;
            timelineController.QuestionProvider = questionProvider;
            timelineController.ImageDisplayFactory = imageDisplayFactory;
        }

        public void windowChanged(OSWindow newWindow)
        {
            screenLayoutManager.changeOSWindow(newWindow);
        }

        public void changeTopPanel(LayoutContainer topContainer, AnimationCompletedDelegate animationCompleted)
        {
            if (topContainer != null)
            {
                topContainer.Visible = true;
                topContainer.bringToFront();
            }
            topAnimatedContainer.changePanel(topContainer, 0.25f, animationCompleted);
        }

        public void resetTopPanel(AnimationCompletedDelegate animationCompleted)
        {
            changeTopPanel(null, animationCompleted);
        }

        public void changeLeftPanel(LayoutContainer leftContainer)
        {
            changeLeftPanel(leftContainer, null);
        }

        public void changeLeftPanel(LayoutContainer leftContainer, AnimationCompletedDelegate animationCompleted)
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

        public void changeRightPanel(LayoutContainer rightContainer, AnimationCompletedDelegate animationCompleted)
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

        public void changeBottomPanel(LayoutContainer bottomContainer, AnimationCompletedDelegate animationCompleted)
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
            standaloneController.AtlasPluginManager.setMainInterfaceEnabled(enabled);
            if (enabled)
            {
                taskbar.Visible = true;
                dialogManager.reopenDialogs();
            }
            else
            {
                taskbar.Visible = false;
                dialogManager.temporarilyCloseDialogs();
            }
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            standaloneController.AtlasPluginManager.createMenus(menu);
        }

        public void addManagedDialog(Dialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
        }

        public void addManagedDialog(MDIDialog dialog)
        {
            dialogManager.addManagedDialog(dialog);
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

        internal void loadDialogPositions()
        {
            dialogManager.loadDialogLayout(MedicalConfig.WindowsFile);
        }

        //private void animationCompleted(LayoutContainer oldChild)
        //{
        //    if (oldChild != null)
        //    {
        //        oldChild.Visible = false;
        //    }
        //}

        //private void leftAnimationCompleted(LayoutContainer oldChild)
        //{
        //    animationCompleted(oldChild);
        //    foreach (UIAnimationFinishedCallback animFinished in leftAnimationFinishedCBQueue)
        //    {
        //        animFinished.Invoke();
        //    }
        //    leftAnimationFinishedCBQueue.Clear();
        //}

        private void standaloneController_SceneUnloading(SimScene scene)
        {
            
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            this.changeLeftPanel(null);
        }

        private void screenLayoutManager_ScreenSizeChanged(int width, int height)
        {
            dialogManager.windowResized();
            continuePrompt.ensureVisible();
            if (taskMenu.Visible)
            {
                taskMenu.setPosition((int)innerBorderLayout.Location.x, (int)innerBorderLayout.Location.y);
                taskMenu.setSize((int)innerBorderLayout.WorkingSize.Width, (int)innerBorderLayout.WorkingSize.Height);
            }
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

        void taskMenu_TaskItemOpened(TaskMenuItem item)
        {
            if (item.ShowOnTaskbar && item._TaskbarItem == null)
            {
                item._TaskbarItem = new TaskMenuItemTaskbarItem(item);
                item.ItemClosed += item_ItemClosed;
                taskbar.addItem(item._TaskbarItem);
                taskbar.layout();
            }
        }

        void item_ItemClosed(TaskMenuItem item)
        {
            item.ItemClosed -= item_ItemClosed;
            taskbar.removeItem(item._TaskbarItem);
            item._TaskbarItem = null;
            taskbar.layout();
        }
    }
}
