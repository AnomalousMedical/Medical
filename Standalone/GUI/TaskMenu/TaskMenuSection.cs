using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class TaskMenuSection
    {
        public delegate void TaskEvent(TaskMenuItem taskItem);

        public event TaskEvent TaskItemAdded;
        public event TaskEvent TaskItemRemoved;

        private Dictionary<String, TaskMenuItem> items = new Dictionary<String, TaskMenuItem>();

        public TaskMenuSection()
        {

        }

        public void addItem(TaskMenuItem item)
        {
            items.Add(item.UniqueName, item);
            if (TaskItemAdded != null)
            {
                TaskItemAdded.Invoke(item);
            }
        }

        public void removeItem(TaskMenuItem item)
        {
            items.Remove(item.UniqueName);
            if (TaskItemRemoved != null)
            {
                TaskItemRemoved.Invoke(item);
            }
        }

        public TaskMenuItem getItem(String uniqueName)
        {
            TaskMenuItem item = null;
            items.TryGetValue(uniqueName, out item);
            return item;
        }

        public IEnumerable<TaskMenuItem> Items
        {
            get
            {
                return items.Values;
            }
        }
    }
}
