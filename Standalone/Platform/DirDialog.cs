using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    class DirDialog : IDisposable
    {
        private IntPtr dirDialog = IntPtr.Zero;

        public DirDialog(String message, String startPath)
        {
            dirDialog = DirDialog_new(message, startPath);
        }

        public void Dispose()
        {
            if (dirDialog != IntPtr.Zero)
            {
                DirDialog_delete(dirDialog);
                dirDialog = IntPtr.Zero;
            }
        }

        public NativeDialogResult showModal()
        {
            return DirDialog_showModal(dirDialog);
        }

        public String Path
        {
            get
            {
                using (NativeString nativeString = new NativeString(DirDialog_getPath(dirDialog)))
                {
                    return nativeString.ToString();
                }
            }
            set
            {
                DirDialog_setPath(dirDialog, value);
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr DirDialog_new(String message, String startPath);

        [DllImport("OSHelper")]
        private static extern void DirDialog_delete(IntPtr dirDialog);

        [DllImport("OSHelper")]
        private static extern NativeDialogResult DirDialog_showModal(IntPtr dirDialog);

        [DllImport("OSHelper")]
        private static extern void DirDialog_setPath(IntPtr dirDialog, String path);

        [DllImport("OSHelper")]
        private static extern IntPtr DirDialog_getPath(IntPtr dirDialog);

        #endregion
    }
}
