using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class CallbackNotificationGUI : NotificationGUI
    {
        private Action clickedCallback;

        public CallbackNotificationGUI(String text, String imageKey, NotificationGUIManager notificationManager, Action clickedCallback)
            :base(text, imageKey, notificationManager)
        {
            this.clickedCallback = clickedCallback;
        }

        protected override void clicked()
        {
            if (clickedCallback != null)
            {
                clickedCallback.Invoke();
            }
        }
    }
}
