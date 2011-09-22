using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

namespace Medical
{
    class WxKeyboard : Engine.Platform.Keyboard, IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void KeyDownDelegate(KeyboardButtonCode keyCode, uint character);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void KeyUpDelegate(KeyboardButtonCode keyCode);

        private KeyDownDelegate keyDownCB;
        private KeyUpDelegate keyUpCB;

        private IntPtr nativeKeyboard;

        public override event KeyEvent KeyPressed;
        public override event KeyEvent KeyReleased;

        private NativeOSWindow window;
        private bool[] keysDown = new bool[256];
        bool altDown = false;
        bool ctrlDown = false;
        bool shiftDown = false;

        public WxKeyboard(NativeOSWindow window)
        {
            this.window = window;

            keyDownCB = new KeyDownDelegate(OnKeyDown);
            keyUpCB = new KeyUpDelegate(OnKeyUp);

            nativeKeyboard = WxKeyboard_new(window._NativePtr, keyDownCB, keyUpCB);
        }

        public void Dispose()
        {
            WxKeyboard_delete(nativeKeyboard);
            nativeKeyboard = IntPtr.Zero;
        }

        public override void capture()
        {
            
        }

        void OnKeyDown(KeyboardButtonCode keyCode, uint character)
        {
            if (!keysDown[(int)keyCode])
            {
                keysDown[(int)keyCode] = true;
                switch (keyCode)
                {
                    case KeyboardButtonCode.KC_LSHIFT:
                        shiftDown = true;
                        break;

                    case KeyboardButtonCode.KC_LMENU:
                        altDown = true;
                        break;

                    case KeyboardButtonCode.KC_LCONTROL:
                        ctrlDown = true;
                        break;
                }
                if (KeyPressed != null)
                {
                    KeyPressed.Invoke(keyCode, character);
                }
            }
        }

        void OnKeyUp(KeyboardButtonCode keyCode)
        {
            if (keysDown[(int)keyCode])
            {
                keysDown[(int)keyCode] = false;
                switch (keyCode)
                {
                    case KeyboardButtonCode.KC_LSHIFT:
                        shiftDown = false;
                        break;

                    case KeyboardButtonCode.KC_LMENU:
                        altDown = false;
                        break;

                    case KeyboardButtonCode.KC_LCONTROL:
                        ctrlDown = false;
                        break;
                }
                if (KeyReleased != null)
                {
                    KeyReleased.Invoke(keyCode, 0);
                }
            }
        }

        public override string getAsString(KeyboardButtonCode code)
        {
            return "";
        }

        public override bool isKeyDown(KeyboardButtonCode keyCode)
        {
            return keysDown[(int)keyCode];
        }

        public override bool isModifierDown(Modifier keyCode)
        {
            switch (keyCode)
            {
                case Modifier.Shift:
                    return shiftDown;
                case Modifier.Alt:
                    return altDown;
                case Modifier.Ctrl:
                    return ctrlDown;
                default:
                    return false;
            }
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr WxKeyboard_new(IntPtr osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WxKeyboard_delete(IntPtr keyboard);

        #endregion
    }
}
