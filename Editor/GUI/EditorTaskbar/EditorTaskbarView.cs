using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class EditorTaskbarView : MyGUIView
    {
        private List<Task> tasks = new List<Task>();

        public EditorTaskbarView(String name, String file, String closeAction = null)
            :base(name)
        {
            this.File = file;
            this.ElementName = new BorderLayoutElementName(GUILocationNames.EditorBorderLayout, BorderLayoutLocations.Top);
            this.CloseAction = closeAction;
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

        public String File { get; set; }

        public String CloseAction { get; set; }

        protected EditorTaskbarView(LoadInfo info)
            :base(info)
        {

        }
    }
}
