using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class FileOpenDialog : IDisposable
    {
        private IntPtr fileDialog;

        public FileOpenDialog(NativeOSWindow parent)
            : this(parent, "", "", "", "")
        {

        }

        public FileOpenDialog(NativeOSWindow parent, String message)
            :this(parent, message, "", "", "")
        {
            
        }

        public FileOpenDialog(NativeOSWindow parent, String message, String defaultDir, String defaultFile, String wildcard)
        {
            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }
            fileDialog = FileOpenDialog_new(parentPtr, message, defaultDir, defaultFile, wildcard);
        }

        public void Dispose()
        {
            FileOpenDialog_delete(fileDialog);
        }

        public NativeDialogResult showModal()
        {
            return FileOpenDialog_showModal(fileDialog);
        }

        public String Wildcard
        {
            get
            {
                using (NativeString ns = new NativeString(FileOpenDialog_getWildcard(fileDialog)))
                {
                    return ns.ToString();
                }
            }
            set
            {
                FileOpenDialog_setWildcard(fileDialog, value);
            }
        }

        public String Path
        {
            get
            {
                using (NativeString ns = new NativeString(FileOpenDialog_getPath(fileDialog)))
                {
                    return ns.ToString();
                }
            }
            set
            {
                FileOpenDialog_setPath(fileDialog, value);
            }
        }

        public String[] Paths
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr FileOpenDialog_new(IntPtr parent, String message, String defaultDir, String defaultFile, String wildcard);

        [DllImport("OSHelper")]
        private static extern void FileOpenDialog_delete(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern NativeDialogResult FileOpenDialog_showModal(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern void FileOpenDialog_setWildcard(IntPtr fileDialog, String value);

        [DllImport("OSHelper")]
        private static extern IntPtr FileOpenDialog_getWildcard(IntPtr fileDialog);

        [DllImport("OSHelper")]
        private static extern void FileOpenDialog_setPath(IntPtr fileDialog, String value);

        [DllImport("OSHelper")]
        private static extern IntPtr FileOpenDialog_getPath(IntPtr fileDialog);

        #endregion
    }
}
