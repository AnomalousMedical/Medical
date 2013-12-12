using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using OgreWrapper;

namespace Medical
{
    public delegate void ScreenSizeChanged(int width, int height);

    public class ScreenLayoutManager : OSWindowListener
    {
        public event ScreenSizeChanged ScreenSizeChanged;

        private LayoutChain layoutChain;
        private OSWindow window;

        public ScreenLayoutManager(OSWindow window)
        {
            this.window = window;
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

        public LayoutChain LayoutChain
        {
            get
            {
                return layoutChain;
            }
            set
            {
                layoutChain = value;
                layoutChain.Location = new IntVector2(0, 0);
                layoutChain.WorkingSize = new IntSize2(window.WindowWidth, window.WindowHeight);
            }
        }

        public void closing(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            layoutChain.WorkingSize = new IntSize2(window.WindowWidth, window.WindowHeight);
            layoutChain.layout();
            if (ScreenSizeChanged != null)
            {
                ScreenSizeChanged.Invoke(window.WindowWidth, window.WindowHeight);
            }
        }

        public void closed(OSWindow window)
        {

        }

        public void focusChanged(OSWindow window)
        {

        }
    }
}
