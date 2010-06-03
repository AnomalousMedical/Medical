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
        private MedicalController medicalController;

        public WindowListener(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        public void closing(OSWindow window)
        {
            medicalController.shutdown();
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            
        }
    }
}
