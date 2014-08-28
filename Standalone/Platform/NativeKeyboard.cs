using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

namespace Medical
{
    class NativeKeyboard : Engine.Platform.KeyboardHardware, IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void KeyDownDelegate(KeyboardButtonCode keyCode, uint character);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void KeyUpDelegate(KeyboardButtonCode keyCode);

        private KeyDownDelegate keyDownCB;
        private KeyUpDelegate keyUpCB;

        private IntPtr nativeKeyboard;

        private NativeOSWindow window;
        private bool[] keysDown = new bool[256];
        bool altDown = false;
        bool ctrlDown = false;
        bool shiftDown = false;

        public NativeKeyboard(NativeOSWindow window, EventManager eventManager)
            :base(eventManager)
        {
            this.window = window;
            window.Deactivated += window_Deactivated;

            keyDownCB = new KeyDownDelegate(OnKeyDown);
            keyUpCB = new KeyUpDelegate(OnKeyUp);

            nativeKeyboard = NativeKeyboard_new(window._NativePtr, keyDownCB, keyUpCB);
        }

        public void Dispose()
        {
            NativeKeyboard_delete(nativeKeyboard);
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
                fireKeyPressed(keyCode, character);
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
                fireKeyReleased(keyCode, 0);
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

        void window_Deactivated(OSWindow window)
        {
            for (int i = 0; i < keysDown.Length; ++i)
            {
                OnKeyUp((KeyboardButtonCode)i);
            }
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NativeKeyboard_new(IntPtr osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeKeyboard_delete(IntPtr keyboard);

        #endregion
    }
}
