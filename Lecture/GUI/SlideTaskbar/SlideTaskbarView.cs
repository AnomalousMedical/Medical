using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical;

namespace Lecture.GUI
{
    class SlideTaskbarView : MyGUIView
    {
        public event Action<SlideTaskbarView> DisplayNameChanged;
        private String displayName;

        private List<Task> tasks = new List<Task>();

        public SlideTaskbarView(String name, String displayName)
            :base(name)
        {
            this.DisplayName = displayName;
            this.ViewLocation = ViewLocations.Top;
        }

        public void addTask(Task task)
        {
            tasks.Add(task);
        }

        public IEnumerable<Task> Tasks
        {
            get
            {
                return tasks;
            }
        }

        public String DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
                if (DisplayNameChanged != null)
                {
                    DisplayNameChanged.Invoke(this);
                }
            }
        }

        protected SlideTaskbarView(LoadInfo info)
            :base(info)
        {

        }
    }
}
