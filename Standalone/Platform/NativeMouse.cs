using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using System.Runtime.InteropServices;

namespace Medical
{
    class NativeMouse : MouseHardware, IDisposable
    {
        private NativeOSWindow window;

        IntPtr nativeMouse;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void MouseButtonDownDelegate(MouseButtonCode id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseButtonUpDelegate(MouseButtonCode id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseMoveDelegate(int absX, int absY);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseWheelDelegate(int relZ);

        MouseButtonDownDelegate mouseButtonDownCB;
        MouseButtonUpDelegate mouseButtonUpCB;
        MouseMoveDelegate mouseMoveCB;
        MouseWheelDelegate mouseWheelCB;

        public NativeMouse(NativeOSWindow window, Mouse mouse)
            : base(mouse)
        {
            this.window = window;

            mouseButtonDownCB = new MouseButtonDownDelegate(fireButtonDown);
            mouseButtonUpCB = new MouseButtonUpDelegate(fireButtonUp);
            mouseMoveCB = new MouseMoveDelegate(fireMoved);
            mouseWheelCB = new MouseWheelDelegate(fireWheel);

            nativeMouse = NativeMouse_new(window._NativePtr, mouseButtonDownCB, mouseButtonUpCB, mouseMoveCB, mouseWheelCB);

            fireSizeChanged(window.WindowWidth, window.WindowHeight);
            window.Resized += window_Resized;
        }

        public void Dispose()
        {
            window.Resized -= window_Resized;
            NativeMouse_delete(nativeMouse);
            nativeMouse = IntPtr.Zero;
        }

        void window_Resized(OSWindow window)
        {
            fireSizeChanged(window.WindowWidth, window.WindowHeight);
        }

        #region PInvoke

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeMouse_new(IntPtr osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeMouse_delete(IntPtr mouse);

        #endregion
    }
}
