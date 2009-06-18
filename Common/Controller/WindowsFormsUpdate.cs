using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Windows.Forms;

namespace Medical
{
    class WindowsFormsUpdate : UpdateListener
    {
        public void sendUpdate(Clock clock)
        {
            Application.DoEvents();
        }

        public void loopStarting()
        {

        }

        public void exceededMaxDelta()
        {

        }
    }
}
