using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.GUI
{
    class MyGUIImageDisplayFactory : IImageDisplayFactory, OSWindowListener
    {
        private List<MyGUIImageDisplay> displays = new List<MyGUIImageDisplay>();

        public IImageDisplay createImageDisplay()
        {
            MyGUIImageDisplay display = new MyGUIImageDisplay(this);
            displays.Add(display);
            return display;
        }

        internal void displayDisposed(MyGUIImageDisplay display)
        {
            displays.Remove(display);
        }

        #region OSWindowListener Members

        public void closed(OSWindow window)
        {
            
        }

        public void closing(OSWindow window)
        {
            
        }

        public void focusChanged(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            foreach (MyGUIImageDisplay display in displays)
            {
                display.screenResized(window.WindowWidth, window.WindowHeight);
            }
        }

        #endregion
    }
}
