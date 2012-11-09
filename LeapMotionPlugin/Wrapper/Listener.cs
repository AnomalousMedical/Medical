using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LeapMotionPlugin
{
    class Listener : LeapNativeClass, IDisposable
    {
        public Listener()
        {
            Ptr = ManagedListener_Create();
        }

        public void Dispose()
        {
            ManagedListener_Delete(Ptr);
        }

        #region PInvoke

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ManagedListener_Create();

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ManagedListener_Delete(IntPtr managedListener);

        #endregion
    }
}
