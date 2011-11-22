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
        private StandaloneController standaloneController;

        public NotificationGUIManager(Taskbar taskbar, StandaloneController standaloneController)
        {
            this.taskbar = taskbar;
            this.standaloneController = standaloneController;
        }

        public void Dispose()
        {
            foreach (NotificationGUI notification in openNotifications)
            {
                notification.Dispose();
            }
        }

        public void showNotification(Notification notification)
        {
            NotificationGUI notificationGUI = new NotificationGUI(notification, this);
            positionNotification(notificationGUI);
        }

        public void showNotification(String text, String imageKey)
        {
            showNotification(new AbstractNotification(text, imageKey));
        }

        public void showTaskNotification(String text, String imageKey, Task task)
        {
            showNotification(new StartTaskNotification(text, imageKey, task));
        }

        public void showRestartNotification(String text, String imageKey, bool autoStartPlatformUpdate)
        {
            showNotification(new RestartNotification(text, imageKey, standaloneController, autoStartPlatformUpdate));
        }

        public void showCallbackNotification(String text, String imageKey, Action clickedCallback)
        {
            showNotification(new CallbackNotificationGUI(text, imageKey, clickedCallback));
        }

        internal void notificationClosed(NotificationGUI notification)
        {
            openNotifications.Remove(notification);
            relayoutNotifications();
        }

        internal void hideAllNotifications()
        {
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.hide();
            }
        }

        internal void reshowAllNotifications()
        {
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.show(openNotification.Left, openNotification.Top);
            }
        }

        internal void screenSizeChanged()
        {
            int currentHeight = 0;
            switch (taskbar.Alignment)
            {
                case TaskbarAlignment.Top:
                    currentHeight = taskbar.Height;
                    foreach (NotificationGUI openNotification in openNotifications)
                    {
                        openNotification.setPosition(Gui.Instance.getViewWidth() - openNotification.Width, currentHeight);
                        currentHeight += openNotification.Height;
                    }
                    break;
                case TaskbarAlignment.Right:
                    foreach (NotificationGUI openNotification in openNotifications)
                    {
                        openNotification.setPosition(Gui.Instance.getViewWidth() - openNotification.Width - taskbar.Width, currentHeight);
                        currentHeight += openNotification.Height;
                    }
                    break;
                default:
                    foreach (NotificationGUI openNotification in openNotifications)
                    {
                        openNotification.setPosition(Gui.Instance.getViewWidth() - openNotification.Width, currentHeight);
                        currentHeight += openNotification.Height;
                    }
                    break;
            }
        }

        private void positionNotification(NotificationGUI notification)
        {
            int additionalHeightOffset = 0;
            foreach(NotificationGUI openNotification in openNotifications)
            {
                additionalHeightOffset += openNotification.Height;
            }
            openNotifications.Add(notification);
            switch (taskbar.Alignment)
            {
                case TaskbarAlignment.Top:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, taskbar.Height + additionalHeightOffset);
                    break;
                case TaskbarAlignment.Right:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width - taskbar.Width, additionalHeightOffset);
                    break;
                default:
                    notification.show(Gui.Instance.getViewWidth() - notification.Width, additionalHeightOffset);
                    break;
            }
        }

        private void relayoutNotifications()
        {
            int currentHeight = 0;
            if (taskbar.Alignment == TaskbarAlignment.Top)
            {
                currentHeight = taskbar.Height;
            }
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.setPosition(openNotification.Left, currentHeight);
                currentHeight += openNotification.Height;
            }
        }
    }
}
