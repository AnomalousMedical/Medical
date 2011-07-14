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

        public TaskMenuItem(String name, String iconName, String category)
        {
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

        public int Weight { get; set; }

        public bool ShowOnTaskbar { get; set; }

        protected void fireItemClosed()
        {
            if (ItemClosed != null)
            {
                ItemClosed.Invoke(this);
            }
        }

        /// <summary>
        /// Used only by GUIManager
        /// </summary>
        internal TaskMenuItemTaskbarItem _TaskbarItem { get; set; }
    }
}
