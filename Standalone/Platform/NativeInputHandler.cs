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

        public override Keyboard createKeyboard(bool buffered)
        {
            if (createdKeyboard == null)
            {
                createdKeyboard = new NativeKeyboard(window);
            }
            return createdKeyboard;
        }

        public override Mouse createMouse(bool buffered)
        {
            if (createdMouse == null)
            {
                createdMouse = new NativeMouse(window);
            }
            return createdMouse;
        }

        public override void destroyKeyboard(Keyboard keyboard)
        {
            ((NativeKeyboard)keyboard).Dispose();
            if (keyboard == createdKeyboard)
            {
                createdKeyboard = null;
            }
        }

        public override void destroyMouse(Mouse mouse)
        {
            ((NativeMouse)mouse).Dispose();
            if (mouse == createdMouse)
            {
                createdMouse = null;
            }
        }
    }
}
