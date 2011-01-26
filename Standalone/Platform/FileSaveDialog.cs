using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class FileSaveDialog : IDisposable
    {
        private IntPtr fileDialog;

        public FileSaveDialog(NativeOSWindow parent, String message)
            :this(parent, message, "", "", "")
        {
            
        }

        public FileSaveDialog(NativeOSWindow parent, String message, String defaultDir, String defaultFile, String wildcard)
        {
            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }
            fileDialog = FileSaveDialog_new(parentPtr, message, defaultDir, defaultFile, wildcard);
        }

        public void Dispose()
        {
            FileSaveDialog_delete(fileDialog);
        }

        public NativeDialogResult showModal()
        {
            return FileSaveDialog_showModal(fileDialog);
        }

        public String Wildcard
        {
            get
            {
                using (NativeString ns = new NativeString(FileSaveDialog_getWildcard(fileDialog)))
                {
                    return ns.ToString();
                }
            }
            set
            {
                FileSaveDialog_setWildcard(fileDialog, value);
            }
        }

        public String Path
        {
            get
            {
                using (NativeString ns = new NativeString(FileSaveDialog_getPath(fileDialog)))
                {
                    return ns.ToString();
                }
            }
            set
            {
                FileSaveDialog_setPath(fileDialog, value);
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr FileSaveDialog_new(IntPtr parent, String message, String defaultDir, String defaultFile, String wildcard);

        [DllImport("OSHelper")]
        private static extern void FileSaveDialog_delete(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern NativeDialogResult FileSaveDialog_showModal(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern void FileSaveDialog_setWildcard(IntPtr fileDialog, String value);

        [DllImport("OSHelper")]
        private static extern IntPtr FileSaveDialog_getWildcard(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern void FileSaveDialog_setPath(IntPtr fileDialog, String value);

        [DllImport("OSHelper")]
        private static extern IntPtr FileSaveDialog_getPath(IntPtr fileDialog);

        #endregion
    }
}
