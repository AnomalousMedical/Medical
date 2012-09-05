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

        public String Wildcard
        {
            get
            {
                return Marshal.PtrToStringAnsi(FileOpenDialog_getWildcard(fileDialog));
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
                return Marshal.PtrToStringAnsi(FileOpenDialog_getPath(fileDialog));
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
        public IEnumerable<String> Paths
        {
            get
            {
                int numPaths = FileOpenDialog_getNumPaths(fileDialog);
                for (int i = 0; i < numPaths; ++i)
                {
                    yield return Marshal.PtrToStringAnsi(FileOpenDialog_getPathIndex(fileDialog, i));
                }
            }
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr FileOpenDialog_new(IntPtr parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void FileOpenDialog_delete(IntPtr fileDialog);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern NativeDialogResult FileOpenDialog_showModal(IntPtr fileDialog);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void FileOpenDialog_setWildcard(IntPtr fileDialog, String value);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr FileOpenDialog_getWildcard(IntPtr fileDialog);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void FileOpenDialog_setPath(IntPtr fileDialog, String value);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr FileOpenDialog_getPath(IntPtr fileDialog);

        [DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int FileOpenDialog_getNumPaths(IntPtr fileDialog);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr FileOpenDialog_getPathIndex(IntPtr fileDialog, int index);

        #endregion
    }
}
