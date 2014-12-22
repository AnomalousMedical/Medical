using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Medical.Controller;

namespace Medical
{
    public class FileOpenDialog
    {
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

        /// <summary>
        /// May or may not block the main thread depending on os. Assume it does
        /// not block and handle all results in the callback.
        /// </summary>
        /// <param name="callback">Called when the dialog is done showing with the results.</param>
        /// <returns></returns>
        public void showModal(ResultCallback callback)
        {
            FileOpenDialogResults results = new FileOpenDialogResults(callback);
            results.showNativeDialogModal(Parent, Message, DefaultDir, DefaultFile, Wildcard, SelectMultiple);
        }

        public NativeOSWindow Parent { get; set; }

        public String Message { get; set; }

        public String DefaultDir { get; set; }

        public String DefaultFile { get; set; }

        public String Wildcard { get; set; }

        public bool SelectMultiple { get; set; }

        class FileOpenDialogResults : IDisposable
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void FileOpenDialogSetPathString(IntPtr path);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void FileOpenDialogResultCallback(NativeDialogResult result);

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
                    paths.Add(Marshal.PtrToStringUni(pathPtr));
                };

                resultCb = (result) =>
                {
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            this.showModalCallback(result, paths);
                        }
                        finally
                        {
                            this.Dispose();
                        }
                    });
                };
            }

            public void Dispose()
            {
                handle.Free();
            }

            public void showNativeDialogModal(NativeOSWindow parent, String message, String defaultDir, String defaultFile, String wildcard, bool selectMultiple)
            {
                handle = GCHandle.Alloc(this, GCHandleType.Normal);
                IntPtr parentPtr = parent != null ? parent._NativePtr : IntPtr.Zero;
                FileOpenDialog_showModal(parentPtr, message, defaultDir, defaultFile, wildcard, selectMultiple, setPathStringCb, resultCb);
            }

            #region PInvoke

            [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
            private static extern void FileOpenDialog_showModal(IntPtr parent, [MarshalAs(UnmanagedType.LPWStr)] String message, [MarshalAs(UnmanagedType.LPWStr)] String defaultDir, [MarshalAs(UnmanagedType.LPWStr)] String defaultFile, [MarshalAs(UnmanagedType.LPWStr)] String wildcard, bool selectMultiple, FileOpenDialogSetPathString setPathString, FileOpenDialogResultCallback resultCallback);

            #endregion
        }
    }
}
