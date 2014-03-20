using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical;
using MyGUIPlugin;

namespace Medical
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
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            
        }

        public void closed(OSWindow window)
        {
            controller.exit();
        }

        public void focusChanged(OSWindow window)
        {

        }
    }
}
