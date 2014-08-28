using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using System.Runtime.InteropServices;

namespace Medical
{
    class NativeMouse : MouseHardware, IDisposable
    {
        private NativeOSWindow window;
        private Vector3 absMouse = Vector3.Zero;
        private Vector3 relMouse = Vector3.Zero;
        private Vector3 lastMouse = Vector3.Zero;
        private bool[] buttonDownStatus = new bool[(int)MouseButtonCode.NUM_BUTTONS];

        IntPtr nativeMouse;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void MouseButtonDownDelegate(MouseButtonCode id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseButtonUpDelegate(MouseButtonCode id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseMoveDelegate(int absX, int absY);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	    delegate void MouseWheelDelegate(int relZ);

        MouseButtonDownDelegate mouseButtonDownCB;
        MouseButtonUpDelegate mouseButtonUpCB;
        MouseMoveDelegate mouseMoveCB;
        MouseWheelDelegate mouseWheelCB;

        public NativeMouse(NativeOSWindow window, EventManager eventManager)
            :base(eventManager)
        {
            this.window = window;

            mouseButtonDownCB = new MouseButtonDownDelegate(OnMouseDown);
            mouseButtonUpCB = new MouseButtonUpDelegate(OnMouseUp);
            mouseMoveCB = new MouseMoveDelegate(OnMouseMotion);
            mouseWheelCB = new MouseWheelDelegate(OnMouseWheel);

            nativeMouse = NativeMouse_new(window._NativePtr, mouseButtonDownCB, mouseButtonUpCB, mouseMoveCB, mouseWheelCB);
        }

        public void Dispose()
        {
            NativeMouse_delete(nativeMouse);
            nativeMouse = IntPtr.Zero;
        }

        public override bool buttonDown(MouseButtonCode button)
        {
            return buttonDownStatus[(int)button];
        }

        public override void capture()
        {
            relMouse.x = absMouse.x - lastMouse.x;
            relMouse.y = absMouse.y - lastMouse.y;
            relMouse.z = absMouse.z - lastMouse.z;

            lastMouse = absMouse;
        }

        public override Vector3 AbsolutePosition
        {
            get
            {
                return absMouse;
            }
        }

        public override Vector3 RelativePosition
        {
            get
            {
                return relMouse;
            }
        }

        public override float AreaWidth
        {
            get
            {
                return window.WindowWidth;
            }
        }

        public override float AreaHeight
        {
            get
            {
                return window.WindowHeight;
            }
        }

        void OnMouseDown(MouseButtonCode id)
        {
            buttonDownStatus[(int)id] = true;
            fireButtonDown(id);
        }

        void OnMouseUp(MouseButtonCode id)
        {
            //Make sure the button is down
            if(buttonDownStatus[(int)id])
            {
                buttonDownStatus[(int)id] = false;
                fireButtonUp(id);
            }
        }

        void OnMouseMotion(int x, int y)
        {
            absMouse.x = x;
            absMouse.y = y;

            fireMoved();
        }

        void OnMouseWheel(int z)
        {
            absMouse.z += z;

            fireMoved();
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeMouse_new(IntPtr osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeMouse_delete(IntPtr mouse);

        #endregion
    }
}
