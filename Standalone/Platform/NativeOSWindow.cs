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
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DeleteDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void SizedDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void ClosedDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ActivateDelegate(bool active);

        DeleteDelegate deleteCB;
        SizedDelegate sizedCB;
        ClosedDelegate closedCB;
        ActivateDelegate activateCB;
        String title;
        NativeMenuBar menuBar = null;

        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        IntPtr nativeWindow;

        public event EventHandler Closed;
        public event EventHandler Activated;
        public event EventHandler Deactivated;
        private bool activated = true;

        public NativeOSWindow(String title, Point position, Size size)
            :this(null, title, position, size, false)
        {
            
        }

        public NativeOSWindow(NativeOSWindow parent, String title, Point position, Size size)
            :this(parent, title, position, size, false)
        {

        }

        public NativeOSWindow(NativeOSWindow parent, String title, Point position, Size size, bool floatOnParent)
        {
            this.title = title;

            deleteCB = new DeleteDelegate(delete);
            sizedCB = new SizedDelegate(resize);
            closedCB = new ClosedDelegate(closed);
            activateCB = new ActivateDelegate(activate);

            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }

            nativeWindow = NativeOSWindow_create(parentPtr, title, position.X, position.Y, size.Width, size.Height, floatOnParent, deleteCB, sizedCB, closedCB, activateCB);
        }

        public void Dispose()
        {
            if (nativeWindow != IntPtr.Zero)
            {
                NativeOSWindow_destroy(nativeWindow);
            }
        }

        /// <summary>
        /// Callback from the native class when it is deleted. This WILL be
        /// called if Dispose deletes the class.
        /// </summary>
        private void delete()
        {
            disposed();
            if (menuBar != null)
            {
                menuBar.Dispose(true);
            }
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

        public NativeMenuBar MenuBar
        {
            get
            {
                if (menuBar == null)
                {
                    menuBar = new NativeMenuBar(this, NativeOSWindow_createMenu(nativeWindow));
                }
                return menuBar;
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

        private void activate(bool active)
        {
            Logging.Log.Debug("Window activated {0}", active);
            if (active != this.activated)
            {
                this.activated = active;
                if (activated)
                {
                    if (Activated != null)
                    {
                        Activated.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (Deactivated != null)
                    {
                        Deactivated.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_create(IntPtr parent, String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosedDelegate closedCB, ActivateDelegate activeCB);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_destroy(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setTitle(IntPtr nativeWindow, String title);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_showFullScreen(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setSize(IntPtr nativeWindow, int width, int height);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern int NativeOSWindow_getWidth(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern int NativeOSWindow_getHeight(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_getHandle(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_show(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_close(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setMaximized(IntPtr nativeWindow, bool maximize);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeOSWindow_getMaximized(IntPtr nativeWindow);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setCursor(IntPtr nativeWindow, CursorType cursor);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_createMenu(IntPtr nativeWindow);

        #endregion
    }
}
