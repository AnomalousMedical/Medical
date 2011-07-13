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

        private List<TaskMenuItem> items = new List<TaskMenuItem>();

        public TaskMenuSection()
        {

        }

        public void addItem(TaskMenuItem item)
        {
            items.Add(item);
            if (TaskItemAdded != null)
            {
                TaskItemAdded.Invoke(item);
            }
        }

        public void removeItem(TaskMenuItem item)
        {
            items.Remove(item);
            if (TaskItemRemoved != null)
            {
                TaskItemRemoved.Invoke(item);
            }
        }

        public IEnumerable<TaskMenuItem> Items
        {
            get
            {
                return items;
            }
        }
    }
}
