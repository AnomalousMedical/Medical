using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LeapMotionPlugin
{
    class Controller : LeapNativeClass, IDisposable
    {
        private static Dictionary<IntPtr, Controller> controllerPtrMap = new Dictionary<IntPtr, Controller>();

        private static void addToMap(Controller controller)
        {
            controllerPtrMap.Add(controller.Ptr, controller);
        }

        private static void removeFromMap(Controller controller)
        {
            controllerPtrMap.Remove(controller.Ptr);
        }

        public static Controller resolveController(IntPtr controllerPtr)
        {
            Controller value;
            controllerPtrMap.TryGetValue(controllerPtr, out value);
            return value;
        }

        public Controller(Listener listener)
        {
            Ptr = Controller_Create(listener.Ptr);
            addToMap(this);
        }

        public void Dispose()
        {
            removeFromMap(this);
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
