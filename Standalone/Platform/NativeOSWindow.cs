using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Medical
{
    public class NativeOSWindow : OSWindow, IDisposable
    {
        delegate void DeleteDelegate();
	    delegate void SizedDelegate();
	    delegate void ClosedDelegate();

        DeleteDelegate deleteCB;
        SizedDelegate sizedCB;
        ClosedDelegate closedCB;
        String title;

        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        IntPtr nativeWindow;

        public event EventHandler Closed;

        public NativeOSWindow(String title, Point position, Size size)
            :this(null, title, position, size)
        {
            
        }

        public NativeOSWindow(NativeOSWindow parent, String title, Point position, Size size)
        {
            this.title = title;

            deleteCB = new DeleteDelegate(delete);
            sizedCB = new SizedDelegate(resize);
            closedCB = new ClosedDelegate(closed);

            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }

            nativeWindow = NativeOSWindow_create(parentPtr, title, position.X, position.Y, size.Width, size.Height, deleteCB, sizedCB, closedCB);
        }

        public void Dispose()
        {
            if (nativeWindow != IntPtr.Zero)
            {
                NativeOSWindow_destroy(nativeWindow);
            }
        }

        /// <summary>
        /// Callback from the native class when it is deleted.
        /// </summary>
        private void delete()
        {
            disposed();
            nativeWindow = IntPtr.Zero;
        }

        protected virtual void disposed()
        {

        }

        public void showFullScreen()
        {
            NativeOSWindow_showFullScreen(nativeWindow);
        }

        public void setSize(int width, int height)
        {
            NativeOSWindow_setSize(nativeWindow, width, height);
        }

        public void show()
        {
            NativeOSWindow_show(nativeWindow);
        }

        public void close()
        {
            NativeOSWindow_close(nativeWindow);
        }

        public void setCursor(CursorType cursor)
        {
            NativeOSWindow_setCursor(nativeWindow, cursor);
        }

        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                NativeOSWindow_setTitle(nativeWindow, title);
            }
        }

        public bool Maximized
        {
            get
            {
                return NativeOSWindow_getMaximized(nativeWindow);
            }
            set
            {
                NativeOSWindow_setMaximized(nativeWindow, value);
            }
        }

        public bool Active
        {
            get
            {
                return nativeWindow != IntPtr.Zero;
            }
        }

        #region OSWindow Members

        public bool Focused
        {
            get { return true; }
        }

        public string WindowHandle
        {
            get { return NativeOSWindow_getHandle(nativeWindow).ToString(); }
        }

        public int WindowHeight
        {
            get { return NativeOSWindow_getHeight(nativeWindow); }
        }

        public int WindowWidth
        {
            get { return NativeOSWindow_getWidth(nativeWindow); }
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

        internal IntPtr _NativePtr
        {
            get
            {
                return nativeWindow;
            }
        }

        private void resize()
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.resized(this);
            }
        }

        private void closed()
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }

            foreach (OSWindowListener listener in listeners)
            {
                listener.closed(this);
            }

            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr NativeOSWindow_create(IntPtr parent, String caption, int x, int y, int width, int height, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_destroy(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_setTitle(IntPtr nativeWindow, String title);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_showFullScreen(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_setSize(IntPtr nativeWindow, int width, int height);

        [DllImport("OSHelper")]
        private static extern int NativeOSWindow_getWidth(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern int NativeOSWindow_getHeight(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern IntPtr NativeOSWindow_getHandle(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_show(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_close(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_setMaximized(IntPtr nativeWindow, bool maximize);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeOSWindow_getMaximized(IntPtr nativeWindow);

        [DllImport("OSHelper")]
        private static extern void NativeOSWindow_setCursor(IntPtr nativeWindow, CursorType cursor);

        #endregion
    }
}
