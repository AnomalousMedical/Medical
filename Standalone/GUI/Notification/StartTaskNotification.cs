using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class StartTaskNotification : NotificationGUI
    {
        private Task task;

        public StartTaskNotification(String text, String imageKey, NotificationGUIManager notificationManager, Task task)
            :base(text, imageKey, notificationManager)
        {
            this.task = task;
        }

        protected override void clicked()
        {
            task.clicked(null);
        }
    }
}
