using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Standalone;
using Engine.Platform;
using Engine;
using OgreWrapper;

namespace Medical
{
    public class ScreenLayoutManager : OSWindowListener
    {
        private BorderLayoutContainer rootContainer = new BorderLayoutContainer();
        OSWindow window;

        public ScreenLayoutManager(OSWindow window)
        {
            this.window = window;
            rootContainer.WorkingSize = new Size2(window.WindowWidth, window.WindowHeight);
            window.addListener(this);
        }

        public void changeOSWindow(OSWindow newWindow)
        {
            if (window != null)
            {
                window.removeListener(this);
            }
            this.window = newWindow;
            window.addListener(this);
            resized(window);
        }

        public BorderLayoutContainer Root
        {
            get
            {
                return rootContainer;
            }
        }

        public void layout()
        {
            rootContainer.layout();
        }

        public void closing(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            rootContainer.WorkingSize = new Size2(window.WindowWidth, window.WindowHeight);
            layout();
        }
    }
}
