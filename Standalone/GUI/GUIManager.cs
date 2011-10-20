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

namespace Medical.GUI
{
    public class GUIManager : IDisposable
    {
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

        private Taskbar timelineGUITaskbar;

        private bool mainGuiShowing = true;

        //Dialogs
        private DialogManager dialogManager;

        //Other GUI Elements
        private MyGUIContinuePromptProvider continuePrompt;
        private MyGUIQuestionProvider questionProvider;
        private MyGUIImageDisplayFactory imageDisplayFactory;
        private MyGUITextDisplayFactory textDisplayFactory;
        private List<FullscreenGUIPopup> fullscreenPopups = new List<FullscreenGUIPopup>();
        private MyGUIImageRendererProgress imageRendererProgress;

        public GUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
        }

        public void Dispose()
        {
            //Dialogs
            if (MedicalConfig.WindowsFile != null)
            {
                ConfigFile configFile = new ConfigFile(MedicalConfig.WindowsFile);
                dialogManager.saveDialogLayout(configFile);
                ConfigSection taskbarSection = configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION);
                PinnedTaskSerializer taskSerializer = new PinnedTaskSerializer(taskbarSection);
                taskbar.getPinnedTasks(taskSerializer);
                configFile.writeConfigFile();
            }
            else
            {
                Log.Warning("Could not save window configuration because the WindowsFile is not defined.");
            }

            //Other
            imageRendererProgress.Dispose();
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

            //Timeline taskbar
            timelineGUITaskbar = new Taskbar(standaloneController);
            timelineGUITaskbar.Alignment = TaskbarAlignment.Right;
            timelineGUITaskbar.SuppressLayout = true;
            timelineGUITaskbar.AppButtonVisible = false;
            timelineGUITaskbar.Visible = false;

            taskbar.Child = timelineGUITaskbar;
            timelineGUITaskbar.Child = innerBorderLayout;
            screenLayoutManager.Root = taskbar;

            //Task Menu
            taskMenu = new TaskMenu(standaloneController.DocumentController, standaloneController.TaskController);
            taskMenu.TaskItemOpened += new TaskDelegate(taskMenu_TaskItemOpened);
            taskMenu.TaskItemDropped += new TaskDragDropEventDelegate(taskMenu_TaskItemDropped);
            taskMenu.TaskItemDragged += new TaskDragDropEventDelegate(taskMenu_TaskItemDragged);

            standaloneController.TaskController.TaskAdded += new TaskDelegate(TaskController_TaskAdded);
            standaloneController.TaskController.TaskRemoved += new TaskDelegate(TaskController_TaskRemoved);

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

            timelineGUITaskbar.SuppressLayout = false;
            taskbar.SuppressLayout = false;
            taskbar.layout();

            continuePrompt = new MyGUIContinuePromptProvider();
            questionProvider = new MyGUIQuestionProvider(this);

            imageDisplayFactory = new MyGUIImageDisplayFactory(standaloneController.SceneViewController);
            textDisplayFactory = new MyGUITextDisplayFactory(standaloneController.SceneViewController);
        }

        public void giveGUIsToTimelineController(TimelineController timelineController)
        {
            timelineController.ContinuePrompt = continuePrompt;
            timelineController.QuestionProvider = questionProvider;
            timelineController.ImageDisplayFactory = imageDisplayFactory;
            timelineController.TextDisplayFactory = textDisplayFactory;
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

        public void setMainInterfaceEnabled(bool enabled, bool enableSharedInterface)
        {
            if (mainGuiShowing != enabled)
            {
                taskbar.SuppressLayout = true;
                timelineGUITaskbar.SuppressLayout = true;
                standaloneController.AtlasPluginManager.setMainInterfaceEnabled(enabled);
                if (enabled)
                {
                    taskbar.Visible = true;
                    timelineGUITaskbar.Visible = false;
                    dialogManager.reopenMainGUIDialogs();
                }
                else
                {
                    if (enableSharedInterface)
                    {
                        timelineGUITaskbar.Visible = true;
                    }
                    taskbar.Visible = false;
                    dialogManager.closeMainGUIDialogs();
                }
                mainGuiShowing = enabled;
                taskbar.SuppressLayout = false;
                timelineGUITaskbar.SuppressLayout = false;
                taskbar.layout();
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

        public void addFullscreenPopup(FullscreenGUIPopup popup)
        {
            popup.setPosition((int)innerBorderLayout.Location.x, (int)innerBorderLayout.Location.y);
            popup.setSize((int)innerBorderLayout.WorkingSize.Width, (int)innerBorderLayout.WorkingSize.Height);
            fullscreenPopups.Add(popup);
        }

        public void removeFullscreenPopup(FullscreenGUIPopup popup)
        {
            fullscreenPopups.Remove(popup);
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
            PinnedTaskSerializer pinnedTaskSerializer = new PinnedTaskSerializer(configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION));
            ConfigIterator configIterator = pinnedTaskSerializer.Tasks;
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
            int xPos = (int)innerBorderLayout.Location.x;
            int yPos = (int)innerBorderLayout.Location.y;
            int innerWidth = (int)innerBorderLayout.WorkingSize.Width;
            int innerHeight = (int)innerBorderLayout.WorkingSize.Height;
            if (taskMenu.Visible)
            {
                taskMenu.setPosition(xPos, yPos);
                taskMenu.setSize(innerWidth, innerHeight);
            }
            foreach (FullscreenGUIPopup fullscreenPopup in fullscreenPopups)
            {
                fullscreenPopup.setPosition(xPos, yPos);
                fullscreenPopup.setSize(innerWidth, innerHeight);
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
                if (item._TaskbarItem is PinnedTaskTaskbarItem)
                {
                    taskbar.removeItem(item._TaskbarItem);
                    int index = taskbar.getIndexForPosition(position);
                    taskbar.addItem(item._TaskbarItem, index);
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
        }

        void TaskController_TaskRemoved(Task task)
        {
            //Check to see that the task should show up on the timeline taskbar (TBD)
        }

        void TaskController_TaskAdded(Task task)
        {
            //Check to see that the task should show up on the timeline taskbar
            if (task.ShowOnTimelineTaskbar)
            {
                TimelineTaskbarItem timelineTaskbarItem = new TimelineTaskbarItem(task);
                timelineGUITaskbar.addItem(timelineTaskbarItem);
            }
        }
    }
}
