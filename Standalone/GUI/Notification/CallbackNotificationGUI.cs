using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class CallbackNotificationGUI : AbstractNotification
    {
        private Action clickedCallback;

        public CallbackNotificationGUI(String text, String imageKey, Action clickedCallback)
            :base(text, imageKey)
        {
            this.clickedCallback = clickedCallback;
        }

        public override void clicked()
        {
            if (clickedCallback != null)
            {
                clickedCallback.Invoke();
            }
        }
    }
}
