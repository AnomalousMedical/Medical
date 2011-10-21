using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class GUITaskManager
    {
        private const String TASKBAR_ALIGNMENT_SECTION = "__TaskbarAlignment__";

        private Taskbar taskbar;
        private TaskMenu taskMenu;
        private TaskController taskController;

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
            PinnedTaskSerializer taskSerializer = new PinnedTaskSerializer(taskbarSection);
            taskbar.getPinnedTasks(taskSerializer);
        }

        public void loadPinnedTasks(ConfigFile configFile)
        {
            PinnedTaskSerializer pinnedTaskSerializer = new PinnedTaskSerializer(configFile.createOrRetrieveConfigSection(TASKBAR_ALIGNMENT_SECTION));
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
    }
}
