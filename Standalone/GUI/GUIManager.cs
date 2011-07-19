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

namespace Medical.GUI
{
    public class GUIManager : IDisposable
    {
        private const String SAVED_TASKBAR_ITEM_BASE = "TaskbarItem";
        private const String TASKBAR_ALIGNMENT_SECTION = "__TaskbarAlignment__";

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

        private List<String> pinnedTaskMenuItems = new List<string>();

        public GUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
        }

        public void Dispose()
        {
            //Dialogs
            ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
            dialogManager.saveDialogLayout(configFile);
            ConfigSection taskbarSection = configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION);
            int i = 0;
            foreach (String uniqueName in pinnedTaskMenuItems)
            {
                taskbarSection.setValue(SAVED_TASKBAR_ITEM_BASE + i++, uniqueName);
            }
            configFile.writeConfigFile();

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
            taskMenu = new TaskMenu(standaloneController.DocumentController, standaloneController.TaskController);
            taskMenu.TaskItemOpened += new TaskDelegate(taskMenu_TaskItemOpened);
            taskMenu.TaskItemDropped += new TaskDragDropEventDelegate(taskMenu_TaskItemDropped);
            taskMenu.TaskItemDragged += new TaskDragDropEventDelegate(taskMenu_TaskItemDragged);

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

        internal void loadSavedUI()
        {
            ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
            configFile.loadConfigFile();
            dialogManager.loadDialogLayout(configFile);
            ConfigIterator configIterator = new ConfigIterator(configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION), SAVED_TASKBAR_ITEM_BASE);
            TaskController taskController = standaloneController.TaskController;
            while (configIterator.hasNext())
            {
                String uniqueName = configIterator.next();
                Task item = taskController.getTask(uniqueName);
                if (item != null)
                {
                    addPinnedTaskbarItem(item, -1);
                }
                taskbar.layout();
            }
        }

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

        void taskMenu_TaskItemOpened(Task item)
        {
            addTaskbarItem(item);
        }

        void taskMenu_TaskItemDragged(Task item, IntVector2 position)
        {
            int oldGap = taskbar.GapIndex;
            taskbar.GapIndex = taskbar.getIndexForPosition(position);
            if (oldGap != taskbar.GapIndex)
            {
                taskbar.layout();
            }
        }

        void taskMenu_TaskItemDropped(Task item, IntVector2 position)
        {
            if (taskbar.containsPosition(position))
            {
                if (pinnedTaskMenuItems.Contains(item.UniqueName))
                {
                    taskbar.removeItem(item._TaskbarItem);
                    int index = taskbar.getIndexForPosition(position);
                    taskbar.addItem(item._TaskbarItem, index);
                    pinnedTaskMenuItems.Remove(item.UniqueName);
                    pinnedTaskMenuItems.Insert(index, item.UniqueName);
                }
                else
                {
                    addPinnedTaskbarItem(item, taskbar.getIndexForPosition(position));
                }
            }
            taskbar.clearGapIndex();
            taskbar.layout();
        }

        void pinnedTaskItem_RemoveFromTaskbar(TaskTaskbarItem source)
        {
            Task task = source.Task;
            task._TaskbarItem = null;
            taskbar.removeItem(source);
            taskbar.layout();
            pinnedTaskMenuItems.Remove(task.UniqueName);
            if (task.Active)
            {
                addTaskbarItem(task);
            }
        }

        void taskbarItem_PinToTaskbar(TaskTaskbarItem source)
        {
            addPinnedTaskbarItem(source.Task, taskbar.getIndexForPosition(new IntVector2(source.AbsoluteLeft, source.AbsoluteTop)));
            taskbar.layout();
        }

        void item_ItemClosed(Task item)
        {
            item.ItemClosed -= item_ItemClosed;
            item._TaskbarItem.PinToTaskbar -= taskbarItem_PinToTaskbar;
            taskbar.removeItem(item._TaskbarItem);
            item._TaskbarItem = null;
            taskbar.layout();
        }

        private void addTaskbarItem(Task item)
        {
            if (item.ShowOnTaskbar && item._TaskbarItem == null)
            {
                item._TaskbarItem = new TaskTaskbarItem(item);
                taskbar.addItem(item._TaskbarItem);
                taskbar.layout();
                item.ItemClosed += item_ItemClosed;
                item._TaskbarItem.PinToTaskbar += taskbarItem_PinToTaskbar;
            }
        }

        private void addPinnedTaskbarItem(Task item, int index)
        {
            if (item._TaskbarItem != null)
            {
                item.ItemClosed -= item_ItemClosed;
                item._TaskbarItem.PinToTaskbar -= taskbarItem_PinToTaskbar;
                taskbar.removeItem(item._TaskbarItem);
            }
            PinnedTaskTaskbarItem pinnedTaskItem = new PinnedTaskTaskbarItem(item);
            pinnedTaskItem.RemoveFromTaskbar += new EventDelegate<TaskTaskbarItem>(pinnedTaskItem_RemoveFromTaskbar);
            item._TaskbarItem = pinnedTaskItem;
            taskbar.addItem(pinnedTaskItem, index);
            if (index == -1)
            {
                pinnedTaskMenuItems.Add(item.UniqueName);
            }
            else
            {
                pinnedTaskMenuItems.Insert(index, item.UniqueName);
            }
        }
    }
}
