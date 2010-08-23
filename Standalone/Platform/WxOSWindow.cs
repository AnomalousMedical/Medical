using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using wx;

namespace Medical
{
    public class WxOSWindow : OSWindow
    {
        private List<OSWindowListener> listeners = new List<OSWindowListener>();

        private Window wxWindow;
        private EventListener resizedListener;

        public WxOSWindow(Window wxWindow)
        {
            this.wxWindow = wxWindow;

            resizedListener = new EventListener(onResize);
            wxWindow.EVT_SIZE(resizedListener);
            wxWindow.EVT_CLOSE(onClosed);
        }

        private void onResize(object sender, Event e)
        {
            e.Skip();
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
        }

        private void onClosed(object sender, Event e)
        {
            e.Skip();
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }

            foreach (OSWindowListener listener in listeners)
            {
                listener.closed(this);
            }
        }

        public Window WxWindow
        {
            get
            {
                return wxWindow;
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
            get { return MedicalConfig.EngineConfig.Fullscreen ? wxWindow.Height : wxWindow.ClientSize.Height; }
        }

        public int WindowWidth
        {
            get { return MedicalConfig.EngineConfig.Fullscreen ? wxWindow.Width : wxWindow.ClientSize.Width; }
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
