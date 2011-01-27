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

        public NativeMenu append(String text)
        {
            NativeMenu menu = new NativeMenu(parentWindow, NativeMenuBar_append(menuBar, text));
            menus.Add(menu);
            return menu;
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenuBar_append(IntPtr menuBar, String text);

        #endregion
    }
}
