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
        private bool enableMultitouch;

        public NativeInputHandler(NativeOSWindow window, bool enableMultitouch)
        {
            this.window = window;
            this.enableMultitouch = enableMultitouch;
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

        public override KeyboardHardware createKeyboard(Keyboard keyboard)
        {
            if (createdKeyboard == null)
            {
                createdKeyboard = new NativeKeyboard(window, keyboard);
            }
            return createdKeyboard;
        }

        public override MouseHardware createMouse(Mouse mouse)
        {
            if (createdMouse == null)
            {
                createdMouse = new NativeMouse(window, mouse);
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

        public override TouchHardware createTouchHardware(Touches touches)
        {
            if(enableMultitouch && MultiTouch.IsAvailable)
            {
                return new MultiTouch(window, touches);
            }
            return null;
        }

        public override void destroyTouchHardware(TouchHardware touchHardware)
        {
            if(touchHardware != null)
            {
                ((MultiTouch)touchHardware).Dispose();
            }
        }
    }
}
