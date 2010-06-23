using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using PCPlatform;
using Medical;

namespace Standalone
{
    class WindowListener : OSWindowListener
    {
        private StandaloneController controller;

        public WindowListener(StandaloneController controller)
        {
            this.controller = controller;
        }

        public void closing(OSWindow window)
        {
            controller.shutdown();
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            
        }
    }
}
