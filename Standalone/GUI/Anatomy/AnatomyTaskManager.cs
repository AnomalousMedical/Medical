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
        private GUIManager guiManager;
        public event Action<String, IEnumerable<Task>> HighlightTasks;

        public AnatomyTaskManager(TaskController taskController, GUIManager guiManager)
        {
            this.taskController = taskController;
            this.guiManager = guiManager;
        }

        /// <summary>
        /// Add a task to be associated with anatomy. This will also add the task to the main TaskController, 
        /// so you only need to do it once.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="anatomyNames"></param>
        public void addTask(Task task, IEnumerable<String> anatomyNames)
        {
            taskController.addTask(task);
            if (anatomyNames != null)
            {
                foreach (var name in anatomyNames)
                {
                    List<Task> taskList;
                    if (!tasks.TryGetValue(name, out taskList))
                    {
                        taskList = new List<Task>();
                        tasks.Add(name, taskList);
                    }
                    taskList.Add(task);
                }
            }
        }

        /// <summary>
        /// Remove a task to be associated with anatomy. This will also remove the task from the main TaskController, 
        /// so you only need to do it once.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="anatomyNames"></param>
        public void removeTask(Task task, bool willReload, IEnumerable<String> anatomyNames)
        {
            taskController.removeTask(task, willReload);
            if (anatomyNames != null)
            {
                foreach (var name in anatomyNames)
                {
                    List<Task> taskList;
                    if (tasks.TryGetValue(name, out taskList))
                    {
                        taskList.Remove(task);
                        if (taskList.Count == 0)
                        {
                            tasks.Remove(name);
                        }
                    }
                }
            }
        }

        internal bool showShowTaskButton(string anatomicalName)
        {
            return guiManager.MainGuiShowing && tasks.ContainsKey(anatomicalName);
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
