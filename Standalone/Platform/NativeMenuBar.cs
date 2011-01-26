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

        public NativeMenuBar(IntPtr menuBar)
        {
            this.menuBar = menuBar;
        }

        public NativeMenu append(String text)
        {
            return new NativeMenu(NativeMenuBar_append(menuBar, text));
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenuBar_append(IntPtr menuBar, String text);

        #endregion
    }
}
