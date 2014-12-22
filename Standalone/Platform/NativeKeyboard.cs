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

        public NativeKeyboard(NativeOSWindow window, Keyboard keyboard)
            :base(keyboard)
        {
            this.window = window;
            window.Deactivated += window_Deactivated;

            keyDownCB = new KeyDownDelegate(fireKeyPressed);
            keyUpCB = new KeyUpDelegate(fireKeyReleased);

            nativeKeyboard = NativeKeyboard_new(window._NativePtr, keyDownCB, keyUpCB);
        }

        public void Dispose()
        {
            window.Deactivated -= window_Deactivated;
            NativeKeyboard_delete(nativeKeyboard);
            nativeKeyboard = IntPtr.Zero;
        }

        void window_Deactivated(OSWindow window)
        {
            fireReleaseAllKeys();
        }

        #region PInvoke

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NativeKeyboard_new(IntPtr osWindow, KeyDownDelegate keyDownCB, KeyUpDelegate keyUpCB);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeKeyboard_delete(IntPtr keyboard);

        #endregion
    }
}
