using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using Logging;

namespace Medical.Controller
{
    public enum WindowIcons
    {
        ICON_SKULL,
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
            WindowFunctions_changeWindowIcon(new IntPtr(long.Parse(windowHandle.WindowHandle)), icon);
        }

#region PInvoke
        [DllImport("OSHelper")]
        private static extern int WindowFunctions_changeWindowIcon(IntPtr hwnd, WindowIcons icon);

#endregion
    }
}
