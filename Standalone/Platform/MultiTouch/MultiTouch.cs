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
    public struct TouchInfo
    {
        [FieldOffset(0)]
	    public float normalizedX;
        [FieldOffset(4)]
        public float normalizedY;
        [FieldOffset(8)]
        public int id;
    };

    public delegate void TouchEvent(TouchInfo info);
    public delegate void TouchCanceledEvent();

    public class MultiTouch : IDisposable
    {
        private IntPtr nativeMultiTouch;

        public event TouchEvent TouchStarted;
        public event TouchEvent TouchEnded;
        public event TouchEvent TouchMoved;
        public event TouchCanceledEvent AllTouchesCanceled;

        public MultiTouch(OSWindow windowHandle) 
        {
            touchStartedCB = new TouchEventDelegate(touchStarted);
            touchEndedCB = new TouchEventDelegate(touchEnded);
            touchMovedCB = new TouchEventDelegate(touchMoved);
            touchCanceledCB = new TouchEventCanceledDelegate(allTouchesCanceled);

            IntPtr windowPtr = new IntPtr(long.Parse(windowHandle.WindowHandle));
            Log.Info("Activating MultiTouch on window {0}", windowPtr.ToString());
            nativeMultiTouch = MultiTouch_new(windowPtr, touchStartedCB, touchEndedCB, touchMovedCB, touchCanceledCB);
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
            if (TouchStarted != null)
            {
                TouchStarted.Invoke(touchInfo);
            }
        }

        private void touchEnded(TouchInfo touchInfo)
        {
            if (TouchEnded != null)
            {
                TouchEnded.Invoke(touchInfo);
            }
        }

        private void touchMoved(TouchInfo touchInfo)
        {
            if (TouchMoved != null)
            {
                TouchMoved.Invoke(touchInfo);
            }
        }

        private void allTouchesCanceled()
        {
            if (AllTouchesCanceled != null)
            {
                AllTouchesCanceled.Invoke();
            }
        }

        delegate void TouchEventDelegate(TouchInfo touchInfo);
        TouchEventDelegate touchStartedCB;
        TouchEventDelegate touchEndedCB;
        TouchEventDelegate touchMovedCB;
        delegate void TouchEventCanceledDelegate();
        TouchEventCanceledDelegate touchCanceledCB;

#region PInvoke
        [DllImport("OSHelper")]
        private static extern IntPtr MultiTouch_new(IntPtr hwnd, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB, TouchEventCanceledDelegate touchCanceledCB);

        [DllImport("OSHelper")]
        private static extern void MultiTouch_delete(IntPtr multiTouch);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MultiTouch_isMultitouchAvailable();

#endregion
    }
}
