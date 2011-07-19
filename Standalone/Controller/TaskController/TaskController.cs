using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TaskController
    {
        public event TaskDelegate TaskAdded;
        public event TaskDelegate TaskRemoved;

        private Dictionary<String, Task> items = new Dictionary<String, Task>();

        public TaskController()
        {

        }

        public void addTask(Task item)
        {
            items.Add(item.UniqueName, item);
            if (TaskAdded != null)
            {
                TaskAdded.Invoke(item);
            }
        }

        public void removeTask(Task item)
        {
            items.Remove(item.UniqueName);
            if (TaskRemoved != null)
            {
                TaskRemoved.Invoke(item);
            }
        }

        public Task getTask(String uniqueName)
        {
            Task item = null;
            items.TryGetValue(uniqueName, out item);
            return item;
        }

        public IEnumerable<Task> Tasks
        {
            get
            {
                return items.Values;
            }
        }
    }
}
