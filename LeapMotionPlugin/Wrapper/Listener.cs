using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LeapMotionPlugin
{
    abstract class Listener : LeapNativeClass, IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LeapManagedCallback(IntPtr controller);

        LeapManagedCallback onInitDelegate;
        LeapManagedCallback onConnectDelegate;
        LeapManagedCallback onDisconnectDelegate;
        LeapManagedCallback onFrameDelegate;

        public Listener()
        {
            onInitDelegate = onInitCb;
            onConnectDelegate = onConnectCb;
            onDisconnectDelegate = onDisconnectCb;
            onFrameDelegate = onFrameCb;

            Ptr = ManagedListener_Create(onInitDelegate, onConnectDelegate, onDisconnectDelegate, onFrameDelegate);
        }

        public void Dispose()
        {
            ManagedListener_Delete(Ptr);
        }

        public abstract void onInit(Controller controller);

        public abstract void onConnect(Controller controller);

        public abstract void onDisconnect(Controller controller);

        public abstract void onFrame(Controller controller);

        private void onInitCb(IntPtr controller)
        {
            onInit(Controller.resolveController(controller));
        }

        private void onConnectCb(IntPtr controller)
        {
            onConnect(Controller.resolveController(controller));
        }

        private void onDisconnectCb(IntPtr controller)
        {
            onDisconnect(Controller.resolveController(controller));
        }

        private void onFrameCb(IntPtr controller)
        {
            onFrame(Controller.resolveController(controller));
        }

        #region PInvoke

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ManagedListener_Create(LeapManagedCallback onInitDelegate, LeapManagedCallback onConnectDelegate, LeapManagedCallback onDisconnectDelegate, LeapManagedCallback onFrameDelegate);

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ManagedListener_Delete(IntPtr managedListener);

        #endregion
    }
}
