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
            MultiTouch_registerMultiTouchEventHandler(windowPtr);
        }

        public static bool IsAvailable
        {
            get
            {
                return MultiTouch_isMultitouchAvailable();
            }
        }

#region PInvoke
        [DllImport("OSHelper")]
        private static extern void MultiTouch_registerMultiTouchEventHandler(IntPtr hwnd);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MultiTouch_isMultitouchAvailable();

#endregion
    }
}
