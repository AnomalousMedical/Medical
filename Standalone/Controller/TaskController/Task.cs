﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine;

namespace Medical
{
    public delegate void TaskMenuDelegate();
    public delegate void TaskDelegate(Task task);

    public abstract class Task
    {
        protected const int DEFAULT_WEIGHT = 1000;

        public event TaskDelegate ItemClosed;
        public event TaskDelegate RequestShowInTaskbar;
        public event TaskDelegate IconChanged;

        public Task(String uniqueName, String name, String iconName, String category)
        {
            this.UniqueName = uniqueName;
            this.Name = name;
            this.IconName = iconName;
            this.Category = category;
            Weight = DEFAULT_WEIGHT;
            ShowOnTaskbar = true;
            ShowOnTimelineTaskbar = false;
            Draggable = false;
        }

        /// <summary>
        /// Called when this item is clicked.
        /// </summary>
        public abstract void clicked(TaskPositioner taskPositioner);

        public virtual void dragged(IntVector2 position)
        {

        }

        public virtual void dragStarted(IntVector2 position)
        {

        }

        public virtual void dragEnded(IntVector2 position)
        {

        }

        public abstract bool Active
        {
            get;
        }

        public String IconName { get; protected set; }

        public String Name { get; protected set; }

        public String Category { get; protected set; }

        public String UniqueName { get; protected set; }

        public int Weight { get; set; }

        public bool ShowOnTaskbar { get; set; }

        public bool ShowOnTimelineTaskbar { get; set; }

        public bool Draggable { get; set; }

        protected void fireItemClosed()
        {
            if (ItemClosed != null)
            {
                ItemClosed.Invoke(this);
            }
        }

        protected void fireRequestShowInTaskbar()
        {
            if (RequestShowInTaskbar != null)
            {
                RequestShowInTaskbar.Invoke(this);
            }
        }

        protected void fireIconChanged()
        {
            if (IconChanged != null)
            {
                IconChanged.Invoke(this);
            }
        }
    }
}
