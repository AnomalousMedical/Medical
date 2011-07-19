using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class TaskTaskbarItem : TaskbarItem
    {
        private Task item;

        public TaskTaskbarItem(Task item)
            :base(item.Name, item.IconName)
        {
            this.item = item;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            item.clicked();
        }

        public Task Task
        {
            get
            {
                return item;
            }
        }
    }
}
