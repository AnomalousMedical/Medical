using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    public class WxInputHandler : InputHandler, IDisposable
    {
        NativeOSWindow window;

        private WxKeyboard createdKeyboard;
        private WxMouse createdMouse;

        public WxInputHandler(NativeOSWindow window)
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
                createdKeyboard = new WxKeyboard(window);
            }
            return createdKeyboard;
        }

        public override Mouse createMouse(bool buffered)
        {
            if (createdMouse == null)
            {
                createdMouse = new WxMouse(window);
            }
            return createdMouse;
        }

        public override void destroyKeyboard(Keyboard keyboard)
        {
            ((WxKeyboard)keyboard).Dispose();
            if (keyboard == createdKeyboard)
            {
                createdKeyboard = null;
            }
        }

        public override void destroyMouse(Mouse mouse)
        {
            ((WxMouse)mouse).Dispose();
            if (mouse == createdMouse)
            {
                createdMouse = null;
            }
        }
    }
}
