using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public abstract class TaskMenuItem
    {
        public TaskMenuItem(String name, String iconName, String category)
        {
            this.Name = name;
            this.IconName = iconName;
            this.Category = category;
        }

        public abstract void clicked();

        public String IconName { get; private set; }

        public String Name { get; private set; }

        public String Category { get; private set; }
    }
}
