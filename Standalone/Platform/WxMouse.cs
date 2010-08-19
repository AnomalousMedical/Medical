using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using wx;

namespace Medical
{
    class WxMouse : Mouse, IDisposable
    {
        private WxOSWindow window;
        private Vector3 absMouse = Vector3.Zero;
        private Vector3 relMouse = Vector3.Zero;
        private Vector3 lastMouse = Vector3.Zero;
        private bool[] buttonDownStatus = new bool[(int)MouseButtonCode.NUM_BUTTONS];

        public WxMouse(WxOSWindow window)
        {
            this.window = window;

            window.WxWindow.AddEventListener(Event.wxEVT_LEFT_DOWN, OnMouseLeftDown);
            window.WxWindow.AddEventListener(Event.wxEVT_LEFT_UP, OnMouseLeftUp);

            window.WxWindow.AddEventListener(Event.wxEVT_RIGHT_DOWN, OnMouseRightDown);
            window.WxWindow.AddEventListener(Event.wxEVT_RIGHT_UP, OnMouseRightUp);

            window.WxWindow.AddEventListener(Event.wxEVT_MIDDLE_DOWN, OnMouseMiddleDown);
            window.WxWindow.AddEventListener(Event.wxEVT_MIDDLE_UP, OnMouseMiddleUp);

            window.WxWindow.AddEventListener(Event.wxEVT_MOTION, OnMouseMotion);

            window.WxWindow.AddEventListener(Event.wxEVT_MOUSEWHEEL, OnMouseWheel);
        }

        public void Dispose()
        {
            window.WxWindow.RemoveListener(OnMouseLeftDown);
            window.WxWindow.RemoveListener(OnMouseLeftUp);

            window.WxWindow.RemoveListener(OnMouseRightDown);
            window.WxWindow.RemoveListener(OnMouseRightUp);

            window.WxWindow.RemoveListener(OnMouseMiddleDown);
            window.WxWindow.RemoveListener(OnMouseMiddleUp);

            window.WxWindow.RemoveListener(OnMouseMotion);

            window.WxWindow.RemoveListener(OnMouseWheel);
        }

        public override bool buttonDown(MouseButtonCode button)
        {
            return buttonDownStatus[(int)button];
        }

        public override void capture()
        {
            
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

        void OnMouseLeftDown(object sender, Event evt)
        {
            buttonDownStatus[0] = true;
            fireButtonDown(MouseButtonCode.MB_BUTTON0);
        }

        void OnMouseLeftUp(object sender, Event evt)
        {
            buttonDownStatus[0] = false;
            fireButtonUp(MouseButtonCode.MB_BUTTON0);
        }

        void OnMouseRightDown(object sender, Event evt)
        {
            buttonDownStatus[1] = true;
            fireButtonDown(MouseButtonCode.MB_BUTTON1);
        }

        void OnMouseRightUp(object sender, Event evt)
        {
            buttonDownStatus[1] = false;
            fireButtonUp(MouseButtonCode.MB_BUTTON1);
        }

        void OnMouseMiddleDown(object sender, Event evt)
        {
            buttonDownStatus[2] = true;
            fireButtonDown(MouseButtonCode.MB_BUTTON2);
        }

        void OnMouseMiddleUp(object sender, Event evt)
        {
            buttonDownStatus[2] = false;
            fireButtonUp(MouseButtonCode.MB_BUTTON2);
        }

        void OnMouseMotion(object sender, Event evt)
        {
            MouseEvent mevt = ((MouseEvent)evt);
            absMouse.x = mevt.X;
            absMouse.y = mevt.Y;

            relMouse.x = mevt.X - lastMouse.x;
            relMouse.y = mevt.Y - lastMouse.y;

            lastMouse = absMouse;

            fireMoved(MouseButtonCode.NUM_BUTTONS);
        }

        void OnMouseWheel(object sender, Event evt)
        {
            MouseEvent mevt = ((MouseEvent)evt);
            absMouse.z += mevt.WheelRotation;
            relMouse.z = mevt.WheelRotation;

            fireMoved(MouseButtonCode.NUM_BUTTONS);
        }
    }
}
