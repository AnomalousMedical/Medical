using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public abstract class TaskMenuItem
    {
        protected const int DEFAULT_WEIGHT = 1000;

        public event TaskItemDelegate ItemClosed;
        public event TaskItemDelegate RequestShowInTaskbar;

        public TaskMenuItem(String uniqueName, String name, String iconName, String category)
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

        public String IconName { get; private set; }

        public String Name { get; private set; }

        public String Category { get; private set; }

        public String UniqueName { get; set; }

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

        /// <summary>
        /// Used only by GUIManager
        /// </summary>
        internal TaskMenuItemTaskbarItem _TaskbarItem { get; set; }
    }
}
