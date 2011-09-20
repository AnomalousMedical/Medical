using System;
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
        }

        /// <summary>
        /// Called when this item is clicked.
        /// </summary>
        public abstract void clicked();

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

        public bool OnTaskbar
        {
            get
            {
                return _TaskbarItem != null;
            }
        }

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

        protected IntVector2 findGoodWindowPosition(int width, int height)
        {
            if (_TaskbarItem != null)
            {
                return _TaskbarItem.findGoodWindowPosition(width, height);
            }
            if (_TaskMenu != null)
            {
                return _TaskMenu.findGoodWindowPosition(this, width, height);
            }
            return new IntVector2();
        }

        /// <summary>
        /// Used only by GUIManager
        /// </summary>
        internal TaskTaskbarItem _TaskbarItem { get; set; }

        /// <summary>
        /// Used only by TaskMenu
        /// </summary>
        internal TaskMenu _TaskMenu { get; set; }
    }
}
