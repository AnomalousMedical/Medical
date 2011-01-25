using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

namespace Medical
{
    public class WxOSWindow : OSWindow
    {
        private delegate void NativeCallback();

        private List<OSWindowListener> listeners = new List<OSWindowListener>();

        private IntPtr nativeWindow;

        private NativeCallback closedCallback;
        private NativeCallback sizedCallback;

        internal WxOSWindow(IntPtr nativeWindow)
        {
            this.nativeWindow = nativeWindow;
            closedCallback = new NativeCallback(onClosed);
            sizedCallback = new NativeCallback(onResize);
            WxOSWindow_registerCallbacks(nativeWindow, closedCallback, sizedCallback);
        }

        /// <summary>
        /// <para>
        /// Returns a pointer to the native OSWindow.
        /// </para>
        /// <para>
        /// This should only be called by other platform classes.
        /// </para>
        /// </summary>
        internal IntPtr _NativeOSWindow
        {
            get
            {
                return nativeWindow;
            }
        }

        private void onResize()
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
        }

        private void onClosed()
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }

            foreach (OSWindowListener listener in listeners)
            {
                listener.closed(this);
            }
        }

        #region OSWindow Members

        public bool Focused
        {
            get { return true; }
        }

        public string WindowHandle
        {
            get { return WxOSWindow_getHandle(nativeWindow).ToString(); }
        }

        public int WindowHeight
        {
            get { return WxOSWindow_getHeight(nativeWindow); }
        }

        public int WindowWidth
        {
            get { return WxOSWindow_getWidth(nativeWindow); }
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

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr WxOSWindow_getHandle(IntPtr window);

        [DllImport("OSHelper")]
        private static extern int WxOSWindow_getWidth(IntPtr window);

        [DllImport("OSHelper")]
        private static extern int WxOSWindow_getHeight(IntPtr window);

        [DllImport("OSHelper")]
        private static extern void WxOSWindow_registerCallbacks(IntPtr window, NativeCallback closedCallback, NativeCallback sizedCallback);

        #endregion
    }
}
