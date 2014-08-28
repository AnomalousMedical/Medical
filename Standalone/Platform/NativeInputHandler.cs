using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    public class NativeInputHandler : InputHandler, IDisposable
    {
        NativeOSWindow window;

        private NativeKeyboard createdKeyboard;
        private NativeMouse createdMouse;

        public NativeInputHandler(NativeOSWindow window)
        {
            this.window = window;
        }

        public void Dispose()
        {
            if (createdKeyboard != null)
            {
                destroyKeyboard(createdKeyboard);
            }
            if (createdMouse != null)
            {
                destroyMouse(createdMouse);
            }
        }

        public override KeyboardHardware createKeyboard(bool buffered, EventManager eventManager)
        {
            if (createdKeyboard == null)
            {
                createdKeyboard = new NativeKeyboard(window, eventManager);
            }
            return createdKeyboard;
        }

        public override MouseHardware createMouse(bool buffered, EventManager eventManager)
        {
            if (createdMouse == null)
            {
                createdMouse = new NativeMouse(window, eventManager);
            }
            return createdMouse;
        }

        public override void destroyKeyboard(KeyboardHardware keyboard)
        {
            ((NativeKeyboard)keyboard).Dispose();
            if (keyboard == createdKeyboard)
            {
                createdKeyboard = null;
            }
        }

        public override void destroyMouse(MouseHardware mouse)
        {
            ((NativeMouse)mouse).Dispose();
            if (mouse == createdMouse)
            {
                createdMouse = null;
            }
        }
    }
}
