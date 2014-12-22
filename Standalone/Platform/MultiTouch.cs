using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Engine.Platform;
using Logging;
using Engine;

namespace Medical
{
    public class MultiTouch : TouchHardware, IDisposable
    {
        private IntPtr nativeMultiTouch;

        public MultiTouch(NativeOSWindow nativeWindow, Touches touches) 
            :base(touches)
        {
            touchStartedCB = new TouchEventDelegate(touchStarted);
            touchEndedCB = new TouchEventDelegate(touchEnded);
            touchMovedCB = new TouchEventDelegate(touchMoved);
            touchCanceledCB = new TouchEventCanceledDelegate(allTouchesCanceled);

            Log.Info("Activating MultiTouch on window {0}", nativeWindow._NativePtr);
            nativeMultiTouch = MultiTouch_new(nativeWindow._NativePtr, touchStartedCB, touchEndedCB, touchMovedCB, touchCanceledCB);
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
            fireTouchStarted(touchInfo);
        }

        private void touchEnded(TouchInfo touchInfo)
        {
            fireTouchEnded(touchInfo);
        }

        private void touchMoved(TouchInfo touchInfo)
        {
            fireTouchMoved(touchInfo);
        }

        private void allTouchesCanceled()
        {
            fireAllTouchesCanceled();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void TouchEventDelegate(TouchInfo touchInfo);
        TouchEventDelegate touchStartedCB;
        TouchEventDelegate touchEndedCB;
        TouchEventDelegate touchMovedCB;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void TouchEventCanceledDelegate();
        TouchEventCanceledDelegate touchCanceledCB;

#region PInvoke
        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr MultiTouch_new(IntPtr nativeWindow, TouchEventDelegate touchStartedCB, TouchEventDelegate touchEndedCB, TouchEventDelegate touchMovedCB, TouchEventCanceledDelegate touchCanceledCB);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void MultiTouch_delete(IntPtr multiTouch);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MultiTouch_isMultitouchAvailable();

#endregion
    }
}
