using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class StartTaskNotification : AbstractNotification
    {
        private Task task;

        public StartTaskNotification(String text, String imageKey, Task task)
            :base(text, imageKey)
        {
            this.task = task;
        }

        public override void clicked()
        {
            task.clicked(EmptyTaskPositioner.Instance);
        }
    }
}
