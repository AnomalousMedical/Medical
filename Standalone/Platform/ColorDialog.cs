using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Runtime.InteropServices;

namespace Medical
{
    class ColorDialog : IDisposable
    {
        private IntPtr colorDialog;

        public ColorDialog()
        {
            colorDialog = ColorDialog_new();
        }

        public void Dispose()
        {
            if (colorDialog != IntPtr.Zero)
            {
                ColorDialog_delete(colorDialog);
                colorDialog = IntPtr.Zero;
            }
        }

        public NativeDialogResult showModal()
        {
            return ColorDialog_showModal(colorDialog);
        }

        public Color Color
        {
            get
            {
                return ColorDialog_getColor(colorDialog);
            }
            set
            {
                ColorDialog_setColor(colorDialog, value);
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr ColorDialog_new();

        [DllImport("OSHelper")]
        private static extern void ColorDialog_delete(IntPtr colorDialog);

        [DllImport("OSHelper")]
        private static extern NativeDialogResult ColorDialog_showModal(IntPtr colorDialog);

        [DllImport("OSHelper")]
        private static extern Color ColorDialog_getColor(IntPtr colorDialog);

        [DllImport("OSHelper")]
        private static extern void ColorDialog_setColor(IntPtr colorDialog, Color color);

        #endregion
    }
}
