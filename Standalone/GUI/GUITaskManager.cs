using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical.GUI
{
    public class GUITaskManager
    {
        private const String TASKBAR_ALIGNMENT_SECTION = "__TaskbarAlignment__";

        private Taskbar taskbar;
        private TaskMenu taskMenu;
        private TaskController taskController;

        private Dictionary<Task, TaskTaskbarItem> taskbarItems = new Dictionary<Task, TaskTaskbarItem>();

        public GUITaskManager(Taskbar taskbar, TaskMenu taskMenu, TaskController taskController)
        {
            this.taskController = taskController;

            this.taskbar = taskbar;

            this.taskMenu = taskMenu;
            taskMenu.TaskItemOpened += new TaskDelegate(taskMenu_TaskItemOpened);
            taskMenu.TaskItemDropped += new TaskDragDropEventDelegate(taskMenu_TaskItemDropped);
            taskMenu.TaskItemDragged += new TaskDragDropEventDelegate(taskMenu_TaskItemDragged);
        }

        public void savePinnedTasks(ConfigFile configFile)
        {
            ConfigSection taskbarSection = configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION);
            taskbarSection.setValue("MainTaskbarAlignment", taskbar.Alignment.ToString());
            PinnedTaskSerializer taskSerializer = new PinnedTaskSerializer(taskbarSection);
            taskbar.getPinnedTasks(taskSerializer);
        }

        public void loadPinnedTasks(ConfigFile configFile)
        {
            ConfigSection taskbarSection = configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION);
            PinnedTaskSerializer pinnedTaskSerializer = new PinnedTaskSerializer(taskbarSection);

            String taskbarAlignmentString = taskbarSection.getValue("MainTaskbarAlignment", taskbar.Alignment.ToString());
            try
            {
                taskbar.Alignment = (TaskbarAlignment)Enum.Parse(typeof(TaskbarAlignment), taskbarAlignmentString);
            }
            catch (Exception)
            {
                Log.Warning("Could not parse the taskbar alignment {0}. Using default.", taskbarAlignmentString);
            }

            ConfigIterator configIterator = pinnedTaskSerializer.Tasks;
            while (configIterator.hasNext())
            {
                String uniqueName = configIterator.next();
                Task item = taskController.getTask(uniqueName);
                if (item != null)
                {
                    addPinnedTaskbarItem(item, -1);
                }
            }
        }

        public void addPinnedTask(Task task, int index = -1)
        {
            addPinnedTaskbarItem(task, index);
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
                TaskTaskbarItem taskbarItem;
                taskbarItems.TryGetValue(item, out taskbarItem);

                if (taskbarItem is PinnedTaskTaskbarItem)
                {
                    taskbar.removeItem(taskbarItem);
                    int index = taskbar.getIndexForPosition(position);
                    taskbar.addItem(taskbarItem, index);
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
            taskbarItems.Remove(task);
            taskbar.removeItem(source);
            source.Dispose();
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
            TaskTaskbarItem taskbarItem;
            taskbarItems.TryGetValue(item, out taskbarItem);

            item.ItemClosed -= item_ItemClosed;
            taskbarItem.PinToTaskbar -= taskbarItem_PinToTaskbar;
            taskbar.removeItem(taskbarItem);
            taskbarItems.Remove(item);
            taskbarItem.Dispose();
            taskbar.layout();
        }

        private void addTaskbarItem(Task item)
        {
            TaskTaskbarItem taskbarItem;
            taskbarItems.TryGetValue(item, out taskbarItem);

            if (item.ShowOnTaskbar && taskbarItem == null)
            {
                taskbarItem = new TaskTaskbarItem(item);
                taskbar.addItem(taskbarItem);
                taskbar.layout();
                item.ItemClosed += item_ItemClosed;
                taskbarItem.PinToTaskbar += taskbarItem_PinToTaskbar;
                taskbarItems.Add(item, taskbarItem);
            }
        }

        private void addPinnedTaskbarItem(Task item, int index)
        {
            TaskTaskbarItem taskbarItem;
            taskbarItems.TryGetValue(item, out taskbarItem);

            if (taskbarItem != null)
            {
                item.ItemClosed -= item_ItemClosed;
                taskbarItem.PinToTaskbar -= taskbarItem_PinToTaskbar;
                taskbar.removeItem(taskbarItem);
                taskbarItems.Remove(item);
            }
            PinnedTaskTaskbarItem pinnedTaskItem = new PinnedTaskTaskbarItem(item);
            pinnedTaskItem.RemoveFromTaskbar += new EventDelegate<TaskTaskbarItem>(pinnedTaskItem_RemoveFromTaskbar);
            taskbarItems.Add(item, pinnedTaskItem);
            taskbar.addItem(pinnedTaskItem, index);
        }
    }
}
