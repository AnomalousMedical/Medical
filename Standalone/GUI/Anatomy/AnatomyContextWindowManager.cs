using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindowManager
    {
        private AnatomyContextWindow currentAnatomyWindow;

        public AnatomyContextWindowManager()
        {
            
        }

        public void showWindow(Anatomy anatomy)
        {
            if (currentAnatomyWindow == null)
            {
                currentAnatomyWindow = new AnatomyContextWindow(this);
                currentAnatomyWindow.SmoothShow = true;
            }
            currentAnatomyWindow.Visible = true;
            currentAnatomyWindow.Anatomy = anatomy;
        }

        public void closeUnpinnedWindow()
        {
            if (currentAnatomyWindow != null)
            {
                currentAnatomyWindow.Visible = false;
            }
        }

        internal void alertWindowPinned()
        {
            currentAnatomyWindow = null;
        }
    }
}
