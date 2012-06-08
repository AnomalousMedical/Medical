using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public interface Notification
    {
        void clicked();

        String ImageKey { get; }

        String Text { get; }

        double Timeout { get; }
    }
}
