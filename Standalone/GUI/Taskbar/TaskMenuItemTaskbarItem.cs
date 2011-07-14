using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TaskMenuItemTaskbarItem : TaskbarItem
    {
        private TaskMenuItem item;

        public TaskMenuItemTaskbarItem(TaskMenuItem item)
            :base(item.Name, item.IconName)
        {
            this.item = item;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            item.clicked();
        }
    }
}
