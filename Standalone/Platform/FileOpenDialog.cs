using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class FileOpenDialog
    {
        class FileOpenDialogResults : IDisposable
        {
            List<String> paths = new List<string>();
            FileOpenDialogSetPathString setPathStringCb;
            FileOpenDialogResultCallback resultCb;
            ResultCallback showModalCallback;
            GCHandle handle;

            public FileOpenDialogResults(ResultCallback callback)
            {
                this.showModalCallback = callback;

                setPathStringCb = (pathPtr) =>
                {
                    paths.Add(Marshal.PtrToStringAnsi(pathPtr));
                };

                resultCb = (result) =>
                {
                    try
                    {
                        this.showModalCallback(result, paths);
                    }
                    finally
                    {
                        this.Dispose();
                    }
                };
                handle = GCHandle.Alloc(this, GCHandleType.Normal);
            }

            public void Dispose()
            {
                handle.Free();
            }
            
            public FileOpenDialogSetPathString SetPathStringCb
            {
                get
                {
                    return setPathStringCb;
                }
            }

            public FileOpenDialogResultCallback ResultCb
            {
                get
                {
                    return resultCb;
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FileOpenDialogSetPathString(IntPtr path);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FileOpenDialogResultCallback(NativeDialogResult result);

        public delegate void ResultCallback(NativeDialogResult result, IEnumerable<String> files);

        public FileOpenDialog(NativeOSWindow parent = null, String message = "", String defaultDir = "", String defaultFile = "", String wildcard = "", bool selectMultiple = false)
        {
            Parent = parent;
            Message = message;
            DefaultDir = defaultDir;
            DefaultFile = defaultFile;
            Wildcard = wildcard;
            SelectMultiple = selectMultiple;
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// May or may not block the main thread depending on os. Assume it does
        /// not block and handle all results in the callback.
        /// </summary>
        /// <param name="callback">Called when the dialog is done showing with the results.</param>
        /// <returns></returns>
        public void showModal(ResultCallback callback)
        {
            FileOpenDialogResults results = new FileOpenDialogResults(callback);
            IntPtr parentPtr = Parent != null ? Parent._NativePtr : IntPtr.Zero;
            FileOpenDialog_showModal(parentPtr, Message, DefaultDir, DefaultFile, Wildcard, SelectMultiple, results.SetPathStringCb, results.ResultCb);
        }

        public NativeOSWindow Parent { get; set; }

        public String Message { get; set; }

        public String DefaultDir { get; set; }

        public String DefaultFile { get; set; }

        public String Wildcard { get; set; }

        public bool SelectMultiple { get; set; }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void FileOpenDialog_showModal(IntPtr parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback);

        #endregion
    }
}
