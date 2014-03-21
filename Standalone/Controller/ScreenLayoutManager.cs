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

    public class ScreenLayoutManager
    {
        public event ScreenSizeChanged ScreenSizeChanged;

        private LayoutChain layoutChain;
        private OSWindow window;

        public ScreenLayoutManager(OSWindow window)
        {
            this.window = window;
            window.Resized += resized;
        }

        public void changeOSWindow(OSWindow newWindow)
        {
            if (window != null)
            {
                window.Resized -= resized;
            }
            this.window = newWindow;
            window.Resized += resized;
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

        void resized(OSWindow window)
        {
            layoutChain.WorkingSize = new IntSize2(window.WindowWidth, window.WindowHeight);
            layoutChain.layout();
            if (ScreenSizeChanged != null)
            {
                ScreenSizeChanged.Invoke(window.WindowWidth, window.WindowHeight);
            }
        }
    }
}
