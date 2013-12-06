using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class NotificationGUIManager : IDisposable
    {
        private List<NotificationGUI> openNotifications = new List<NotificationGUI>();
        private StandaloneController standaloneController;
        private EventLayoutContainer layoutContainer = new EventLayoutContainer();

        public NotificationGUIManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            layoutContainer.LayoutChanged += layoutContainer_LayoutChanged;
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
            if (notification.Timeout > 0.0)
            {
                Coroutine.Start(timedNotification(notificationGUI, notification.Timeout));
            }
        }

        public void showNotification(String text, String imageKey, double timeout = -1)
        {
            showNotification(new AbstractNotification(text, imageKey, timeout));
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

        public SingleChildLayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
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

        private void positionNotification(NotificationGUI notification)
        {
            int additionalHeightOffset = layoutContainer.Location.y;
            foreach (NotificationGUI openNotification in openNotifications)
            {
                additionalHeightOffset += openNotification.Height;
            }
            openNotifications.Add(notification);
            notification.show(Right - notification.Width, additionalHeightOffset);
        }

        private void relayoutNotifications()
        {
            int currentHeight = layoutContainer.Location.y;
            foreach (NotificationGUI openNotification in openNotifications)
            {
                openNotification.setPosition(openNotification.Left, currentHeight);
                currentHeight += openNotification.Height;
            }
        }

        void layoutContainer_LayoutChanged(EventLayoutContainer obj)
        {
            relayoutNotifications();
        }

        private IEnumerator<YieldAction> timedNotification(NotificationGUI notificationGui, double waitTime)
        {
            yield return Coroutine.Wait(waitTime);
            notificationGui.closeNotification();
            yield break;
        }

        private int Right
        {
            get
            {
                return layoutContainer.Location.x + layoutContainer.WorkingSize.Width;
            }
        }
    }
}
