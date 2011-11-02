using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class NotificationGUIManager : IDisposable
    {
        private Taskbar taskbar;
        private List<NotificationGUI> openNotifications = new List<NotificationGUI>();

        public NotificationGUIManager(Taskbar taskbar)
        {
            this.taskbar = taskbar;
        }

        public void Dispose()
        {
            foreach (NotificationGUI notification in openNotifications)
            {
                notification.Dispose();
            }
        }

        public void showNotification(String text, String imageKey)
        {
            NotificationGUI notification = new NotificationGUI(text, imageKey, this);
            positionNotification(notification);
        }

        public void showTaskNotification(String text, String imageKey, Task task)
        {
            NotificationGUI notification = new StartTaskNotification(text, imageKey, this, task);
            positionNotification(notification);
        }

        internal void notificationClosed(NotificationGUI notification)
        {
            openNotifications.Remove(notification);
        }

        private void positionNotification(NotificationGUI notification)
        {
            openNotifications.Add(notification);
            switch (taskbar.Alignment)
            {
                case TaskbarAlignment.Top:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, taskbar.Height);
                    break;
                case TaskbarAlignment.Right:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width - taskbar.Width, 0);
                    break;
                default:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, 0);
                    break;
            }
        }
    }
}
