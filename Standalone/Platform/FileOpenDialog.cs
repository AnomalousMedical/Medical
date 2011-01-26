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

        public FileOpenDialog(NativeOSWindow parent, String message)
            :this(parent, message, "", "", "", false)
        {
            
        }

        public FileOpenDialog(NativeOSWindow parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
        {
            IntPtr parentPtr = IntPtr.Zero;
            if (parent != null)
            {
                parentPtr = parent._NativePtr;
            }
            fileDialog = FileOpenDialog_new(parentPtr, message, defaultDir, defaultFile, wildcard, selectMultiple);
        }

        public void Dispose()
        {
            FileOpenDialog_delete(fileDialog);
        }

        public NativeDialogResult showModal()
        {
            return FileOpenDialog_showModal(fileDialog);
        }

        public bool SelectMultiple { get; set; }

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

        /// <summary>
        /// Return an Enumerator over all paths. Call this carefully as it will
        /// allocate native objects. Either let foreach call dispose or call it
        /// yourself.
        /// </summary>
        public NativeStringEnumerator Paths
        {
            get
            {
                NativeStringEnumerator nse = new NativeStringEnumerator();
                FileOpenDialog_getPaths(fileDialog, nse._NativePtr);
                return nse;
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr FileOpenDialog_new(IntPtr parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple);

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

        [DllImport("OSHelper")]
        private static extern void FileOpenDialog_getPaths(IntPtr fileDialog, IntPtr nativeStringEnumerator);

        #endregion
    }
}
