using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class AbstractNotification : Notification
    {
        public AbstractNotification()
        {

        }

        public AbstractNotification(String text, String imageKey)
        {
            Text = text;
            ImageKey = imageKey;
        }

        public virtual void clicked()
        {

        }

        public String Text { get; set; }

        public String ImageKey { get; set; }
    }
}
