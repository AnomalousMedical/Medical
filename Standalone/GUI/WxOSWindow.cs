using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using wx;

namespace Medical.GUI
{
    class WxOSWindow : OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();

        private Window wxWindow;
        private EventListener resizedListener;

        public WxOSWindow(Window wxWindow)
        {
            this.wxWindow = wxWindow;

            resizedListener = new EventListener(onResize);
            wxWindow.EVT_SIZE(resizedListener);
        }

        private void onResize(object sender, Event e)
        {
            e.Skip();
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
        }

        #region OSWindow Members

        public bool Focused
        {
            get { return true; }
        }

        public string WindowHandle
        {
            get { return wxWindow.Handle.ToString(); }
        }

        public int WindowHeight
        {
            get { return wxWindow.Height; }
        }

        public int WindowWidth
        {
            get { return wxWindow.Width; }
        }

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        #endregion
    }
}
