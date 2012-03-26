﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using System.Runtime.InteropServices;

namespace Medical
{
    class WxMouse : Mouse, IDisposable
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

        public WxMouse(NativeOSWindow window)
        {
            this.window = window;

            mouseButtonDownCB = new MouseButtonDownDelegate(OnMouseDown);
            mouseButtonUpCB = new MouseButtonUpDelegate(OnMouseUp);
            mouseMoveCB = new MouseMoveDelegate(OnMouseMotion);
            mouseWheelCB = new MouseWheelDelegate(OnMouseWheel);

            nativeMouse = WxMouse_new(window._NativePtr, mouseButtonDownCB, mouseButtonUpCB, mouseMoveCB, mouseWheelCB);
        }

        public void Dispose()
        {
            WxMouse_delete(nativeMouse);
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

        public override Vector3 getAbsMouse()
        {
            return absMouse;
        }

        public override float getMouseAreaHeight()
        {
            return window.WindowHeight;
        }

        public override float getMouseAreaWidth()
        {
            return window.WindowWidth;
        }

        public override Vector3 getRelMouse()
        {
            return relMouse;
        }

        public override void setSensitivity(float sensitivity)
        {
            
        }

        void OnMouseDown(MouseButtonCode id)
        {
            buttonDownStatus[(int)id] = true;
            fireButtonDown(id);
        }

        void OnMouseUp(MouseButtonCode id)
        {
            //all of a sudden we have to check for this
            //wxWidets is sending mouse up events twice for some reason when double
            //clicking. This fixes it for now. Check ticket number 616 for more info.
            //need to move away from wxWidgets anyway
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

            fireMoved(MouseButtonCode.NUM_BUTTONS);
        }

        void OnMouseWheel(int z)
        {
            absMouse.z += z;

            fireMoved(MouseButtonCode.NUM_BUTTONS);
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr WxMouse_new(IntPtr osWindow, MouseButtonDownDelegate mouseButtonDownCB, MouseButtonUpDelegate mouseButtonUpCB, MouseMoveDelegate mouseMoveCB, MouseWheelDelegate mouseWheelCB);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WxMouse_delete(IntPtr mouse);

        #endregion
    }
}
