using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class TimelineTaskbarItem : TaskbarItem, TaskPositioner
    {
        private Task task;

        public TimelineTaskbarItem(Task task)
            :base(task.Name, task.IconName)
        {
            this.task = task;
            task.IconChanged += task_IconChanged;
        }

        public override void Dispose()
        {
            task.IconChanged -= task_IconChanged;
            base.Dispose();
        }

        public override void clicked(MyGUIPlugin.Widget source, EventArgs e)
        {
            task.clicked(this);
        }

        void task_IconChanged(Task task)
        {
            setIcon(task.IconName);
        }

        public IntVector2 findGoodWindowPosition(int width, int height)
        {
            return findGoodPosition(width, height);
        }
    }
}
