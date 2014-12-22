using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;
using System.Runtime.InteropServices;
using Engine;

namespace Medical
{
    public class NativeOSWindow : OSWindow, IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DeleteDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void SizedDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ClosingDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void ClosedDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ActivateDelegate(bool active);

        DeleteDelegate deleteCB;
        SizedDelegate sizedCB;
        ClosingDelegate closingCB;
        ClosedDelegate closedCB;
        ActivateDelegate activateCB;
        String title;

        IntPtr nativeWindow;

        public event OSWindowEvent Activated;
        public event OSWindowEvent Deactivated;
        private bool activated = true;

        public NativeOSWindow(String title, IntVector2 position, IntSize2 size)
            :this(null, title, position, size, false)
        {
            
        }

        public NativeOSWindow(NativeOSWindow parent, String title, IntVector2 position, IntSize2 size)
            :this(parent, title, position, size, false)
        {

        }

        public NativeOSWindow(NativeOSWindow parent, String title, IntVector2 position, IntSize2 size, bool floatOnParent)
        {
            this.title = title;

            deleteCB = new DeleteDelegate(delete);
            sizedCB = new SizedDelegate(resize);
            closingCB = new ClosingDelegate(closing);
            closedCB = new ClosedDelegate(closed);
            activateCB = new ActivateDelegate(activate);

            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }

            nativeWindow = NativeOSWindow_create(parentPtr, title, position.x, position.y, size.Width, size.Height, floatOnParent, deleteCB, sizedCB, closingCB, closedCB, activateCB);
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
            nativeWindow = IntPtr.Zero;
        }

        protected virtual void disposed()
        {

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

        public void toggleFullscreen()
        {
            NativeOSWindow_toggleFullscreen(nativeWindow);
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

        public bool ExclusiveFullscreen
        {
            get
            {
                return NativeOSWindow_getExclusiveFullscreen(nativeWindow);
            }
            set
            {
                NativeOSWindow_setExclusiveFullscreen(nativeWindow, value);
            }
        }

        public bool Active
        {
            get
            {
                return nativeWindow != IntPtr.Zero;
            }
        }

        public float WindowScaling
        {
            get
            {
                return NativeOSWindow_getWindowScaling(nativeWindow);
            }
        }

        #region OSWindow Members

        public bool Focused
        {
            get { return true; }
        }

        public IntPtr WindowHandle
        {
            get { return NativeOSWindow_getHandle(nativeWindow); }
        }

        public int WindowHeight
        {
            get { return NativeOSWindow_getHeight(nativeWindow); }
        }

        public int WindowWidth
        {
            get { return NativeOSWindow_getWidth(nativeWindow); }
        }

        public event OSWindowEvent Moved;

        public event OSWindowEvent Resized;

        public event OSWindowEvent Closing;

        public event OSWindowEvent Closed;

        public event OSWindowEvent FocusChanged;

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
            if (Resized != null)
            {
                Resized.Invoke(this);
            }
        }

        private void closing()
        {
            if(Closing != null)
            {
                Closing.Invoke(this);
            }
        }

        private void closed()
        {
            if (Closed != null)
            {
                Closed.Invoke(this);
            }
        }

        private void activate(bool active)
        {
            if (active != this.activated)
            {
                this.activated = active;
                if (activated)
                {
                    if (Activated != null)
                    {
                        Activated.Invoke(this);
                    }
                }
                else
                {
                    if (Deactivated != null)
                    {
                        Deactivated.Invoke(this);
                    }
                }
                if (FocusChanged != null)
                {
                    FocusChanged.Invoke(this);
                }
            }
        }

        #region PInvoke

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_create(IntPtr parent, [MarshalAs(UnmanagedType.LPWStr)] String caption, int x, int y, int width, int height, bool floatOnParent, DeleteDelegate deleteCB, SizedDelegate sizedCB, ClosingDelegate closingCB, ClosedDelegate closedCB, ActivateDelegate activeCB);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_destroy(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setTitle(IntPtr nativeWindow, [MarshalAs(UnmanagedType.LPWStr)] String title);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setSize(IntPtr nativeWindow, int width, int height);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern int NativeOSWindow_getWidth(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern int NativeOSWindow_getHeight(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_getHandle(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_show(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_close(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setMaximized(IntPtr nativeWindow, bool maximize);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeOSWindow_getMaximized(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setCursor(IntPtr nativeWindow, CursorType cursor);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeOSWindow_createMenu(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float NativeOSWindow_getWindowScaling(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_setExclusiveFullscreen(IntPtr nativeWindow, bool exclusiveFullscreen);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeOSWindow_getExclusiveFullscreen(IntPtr nativeWindow);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeOSWindow_toggleFullscreen(IntPtr nativeWindow);

        #endregion
    }
}
