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
            Timeout = -1;
        }

        public AbstractNotification(String text, String imageKey, double timeout = -1)
        {
            Text = text;
            ImageKey = imageKey;
            Timeout = timeout;
        }

        public virtual void clicked()
        {

        }

        public String Text { get; set; }

        public String ImageKey { get; set; }

        public double Timeout { get; set; }
    }
}
