using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using Logging;
using Engine;

namespace Medical.Controller
{
    public enum WindowIcons
    {
        ICON_SKULL,
        ICON_DOPPLER,
        ICON_TMJOVERVIEW,
    };

    public class WindowFunctions
    {
        private WindowFunctions()
        {

        }

        /// <summary>
        /// Set the icon of a window.
        /// </summary>
        /// <param name="windowHandle">The handle to the window.</param>
        /// <param name="iconHandle">The icon to set.</param>
        public static void setWindowIcon(OSWindow windowHandle, WindowIcons icon)
        {
            WindowFunctions_changeWindowIcon(new IntPtr(NumberParser.ParseLong(windowHandle.WindowHandle)), icon);
        }

        public static void maximizeWindow(OSWindow windowHandle)
        {
            WindowFunctions_maximizeWindow(new IntPtr(NumberParser.ParseLong(windowHandle.WindowHandle)));
        }

        public static void pumpMessages()
        {
            WindowFunctions_pumpMessages();
        }

#region PInvoke
        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern int WindowFunctions_changeWindowIcon(IntPtr hwnd, WindowIcons icon);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern int WindowFunctions_maximizeWindow(IntPtr hwnd);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WindowFunctions_pumpMessages();

#endregion
    }
}
