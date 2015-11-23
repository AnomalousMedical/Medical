using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anomalous.GuiFramework;
using Engine;

namespace Medical.GUI
{
    public class AnatomyTaskManager
    {
        private Dictionary<String, List<Task>> tasks = new Dictionary<String, List<Task>>();
        private TaskController taskController;
        public event Action<String, IEnumerable<Task>> HighlightTasks;

        public AnatomyTaskManager(TaskController taskController)
        {
            this.taskController = taskController;
        }

        public void addTask(IEnumerable<String> anatomyNames, Task task)
        {
            if (anatomyNames != null)
            {
                foreach (var name in anatomyNames)
                {
                    addTask(name, task);
                }
            }
        }

        public void addTask(String anatomyName, Task task)
        {
            List<Task> taskList;
            if (!tasks.TryGetValue(anatomyName, out taskList))
            {
                taskList = new List<Task>();
                tasks.Add(anatomyName, taskList);
            }
            taskList.Add(task);
        }

        public void removeTask(IEnumerable<String> anatomyNames, Task task)
        {
            if (anatomyNames != null)
            {
                foreach (var name in anatomyNames)
                {
                    removeTask(name, task);
                }
            }
        }

        public void removeTask(String anatomyName, Task task)
        {
            List<Task> taskList;
            if (tasks.TryGetValue(anatomyName, out taskList))
            {
                taskList.Remove(task);
                if(taskList.Count == 0)
                {
                    tasks.Remove(anatomyName);
                }
            }
        }

        internal bool hasTasks(string anatomicalName)
        {
            return tasks.ContainsKey(anatomicalName);
        }

        internal IEnumerable<Task> tasksFor(string anatomicalName)
        {
            List<Task> taskList;
            if (tasks.TryGetValue(anatomicalName, out taskList))
            {
                return taskList;
            }
            return IEnumerableUtil<Task>.EmptyIterator;
        }

        public void highlightTasks(String highlightTaskCategory, IEnumerable<Task> highlightTasks)
        {
            
        }

        internal void highlightTasks(string anatomicalName)
        {
            String category = String.Format("Tasks for {0}", anatomicalName);
            var tasks = tasksFor(anatomicalName);
            if (HighlightTasks != null)
            {
                HighlightTasks.Invoke(category, tasks);
            }
        }
    }
}
