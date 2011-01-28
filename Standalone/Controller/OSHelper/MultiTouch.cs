using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Engine.Platform;
using Logging;

namespace Medical
{
    [StructLayout(LayoutKind.Explicit, Size=12)]
    struct TouchInfo
    {
        [FieldOffset(0)]
	    public float normalizedX;
        [FieldOffset(4)]
        public float normalizedY;
        [FieldOffset(8)]
        public int id;
    };

    class MultiTouch : IDisposable
    {
        private IntPtr nativeMultiTouch;

        public MultiTouch(OSWindow windowHandle) 
        {
            touchStartedCB = new TouchEventDelegate(touchStarted);
            touchEndedCB = new TouchEventDelegate(touchEnded);
            touchMovedCB = new TouchEventDelegate(touchMoved);

            IntPtr windowPtr = new IntPtr(long.Parse(windowHandle.WindowHandle));
            Log.Info("Activating MultiTouch on window {0}", windowPtr.ToString());
            nativeMultiTouch = MultiTouch_new(windowPtr, touchStartedCB, touchEndedCB, touchMovedCB);
        }

        public void Dispose()
        {
            MultiTouch_delete(nativeMultiTouch);
        }

        public static bool IsAvailable
        {
            get
            {
                return MultiTouch_isMultitouchAvailable();
            }
        }

        private void touchStarted(TouchInfo touchInfo)
        {
            Log.Debug("Touch started {0} {1} {2}", touchInfo.id, touchInfo.normalizedX, touchInfo.normalizedY);
        }

        private void touchEnded(TouchInfo touchInfo)
        {
            Log.Debug("Touch ended {0} {1} {2}", touchInfo.id, touchInfo.normalizedX, touchInfo.normalizedY);
        }

        private void touchMoved(TouchInfo touchInfo)
        {
            Log.Debug("Touch moved {0} {1} {2}", touchInfo.id, touchInfo.normalizedX, touchInfo.normalizedY);
        }

        delegate void TouchEventDelegate(TouchInfo touchInfo);
        TouchEventDelegate touchStartedCB;
        TouchEventDelegate touchEndedCB;
        TouchEventDelegate touchMovedCB;

#region PInvoke
        [DllImport("OSHelper")]
        private static extern IntPtr MultiTouch_new(IntPtr hwnd, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB);

        [DllImport("OSHelper")]
        private static extern void MultiTouch_delete(IntPtr multiTouch);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MultiTouch_isMultitouchAvailable();

#endregion
    }
}
