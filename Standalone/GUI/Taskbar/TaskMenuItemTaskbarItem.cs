using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TaskMenuItemTaskbarItem : TaskbarItem
    {
        private Task item;

        public TaskMenuItemTaskbarItem(Task item)
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
