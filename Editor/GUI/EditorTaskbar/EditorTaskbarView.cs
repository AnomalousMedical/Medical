﻿using System;
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

        public EditorTaskbarView(String name, String caption, String closeAction = null)
            :base(name)
        {
            this.Caption = caption;
            this.ViewLocation = ViewLocations.Top;
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

        public String Caption { get; set; }

        public String CloseAction { get; set; }

        protected EditorTaskbarView(LoadInfo info)
            :base(info)
        {

        }
    }
}