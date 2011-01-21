using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Engine.Platform;
using Logging;

namespace Medical
{
    class MultiTouch
    {
        private MultiTouch() { }

        public static void registerMultiTouchEventHandler(OSWindow windowHandle)
        {
            IntPtr windowPtr = new IntPtr(long.Parse(windowHandle.WindowHandle));
            Log.Info("Activating MultiTouch on window {0}", windowPtr.ToString());
            WindowFunctions_registerMultiTouchEventHandler(windowPtr);
        }

        public static bool IsAvaliable
        {
            get
            {
                return isMultitouchAvaliable();
            }
        }

#region PInvoke
        [DllImport("OSHelper")]
        private static extern void WindowFunctions_registerMultiTouchEventHandler(IntPtr hwnd);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool isMultitouchAvaliable();

#endregion
    }
}
