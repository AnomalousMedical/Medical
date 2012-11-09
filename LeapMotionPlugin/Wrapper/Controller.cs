using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LeapMotionPlugin
{
    class Controller : LeapNativeClass, IDisposable
    {
        public Controller(Listener listener)
        {
            Ptr = Controller_Create(listener.Ptr);
        }

        public void Dispose()
        {
            Controller_Delete(Ptr);
        }

        #region PInvoke

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Controller_Create(IntPtr listener);

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Controller_Delete(IntPtr controller);

        #endregion
    }
}
