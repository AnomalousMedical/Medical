using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class NativeMenuBar
    {
        IntPtr menuBar;
        NativeOSWindow parentWindow;
        private List<NativeMenu> menus = new List<NativeMenu>();

        public NativeMenuBar(NativeOSWindow parentWindow, IntPtr menuBar)
        {
            this.menuBar = menuBar;
            this.parentWindow = parentWindow;
        }

        internal void Dispose(bool windowDeleted)
        {
            foreach (NativeMenu menu in menus)
            {
                menu.Dispose(windowDeleted);
            }
        }

        public NativeMenu createMenu(String text)
        {
            return new NativeMenu(parentWindow, NativeMenuBar_createMenu(menuBar, text), text);
        }

        public void append(NativeMenu menu)
        {
            menus.Add(menu);
            NativeMenuBar_append(menuBar, menu._NativePtr, menu.Text);
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeMenuBar_createMenu(IntPtr menuBar, String text);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeMenuBar_append(IntPtr menuBar, IntPtr nativeMenu, String text);

        #endregion
    }
}
