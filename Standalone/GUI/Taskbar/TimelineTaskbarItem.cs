using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TimelineTaskbarItem : TaskbarItem
    {
        private Task task;

        public TimelineTaskbarItem(Task task)
            :base(task.Name, task.IconName)
        {
            this.task = task;
            task.IconChanged += new TaskDelegate(task_IconChanged);
        }

        public override void clicked(MyGUIPlugin.Widget source, EventArgs e)
        {
            task.clicked();
        }

        void task_IconChanged(Task task)
        {
            setIcon(task.IconName);
        }
    }
}
