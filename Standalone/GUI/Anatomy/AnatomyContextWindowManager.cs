using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindowManager
    {
        private AnatomyContextWindow anatomyWindow = new AnatomyContextWindow();

        public AnatomyContextWindowManager()
        {
            anatomyWindow.SmoothShow = true;
        }

        public void showWindow(Anatomy anatomy)
        {
            anatomyWindow.Visible = true;
            anatomyWindow.Anatomy = anatomy;
        }

        public void closeUnpinnedWindow()
        {
            anatomyWindow.Visible = false;
        }
    }
}
