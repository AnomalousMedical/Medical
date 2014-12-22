using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Medical.Controller;

namespace Medical
{
    public class FileSaveDialog
    {
        public delegate void ResultCallback(NativeDialogResult result, String path);

        public FileSaveDialog(NativeOSWindow parent = null, String message = "", String defaultDir = "", String defaultFile = "", String wildcard = "")
        {
            Parent = parent;
            Message = message;
            DefaultDir = defaultDir;
            DefaultFile = defaultFile;
            Wildcard = wildcard;
        }

        /// <summary>
        /// May or may not block the main thread depending on os. Assume it does
        /// not block and handle all results in the callback.
        /// </summary>
        /// <param name="callback">Called when the dialog is done showing with the results.</param>
        /// <returns></returns>
        public void showModal(ResultCallback callback)
        {
            FileSaveDialogResults results = new FileSaveDialogResults(callback);
            results.showNativeDialogModal(Parent, Message, DefaultDir, DefaultFile, Wildcard);
        }

        public NativeOSWindow Parent { get; set; }

        public String Message { get; set; }

        public String DefaultDir { get; set; }

        public String DefaultFile { get; set; }

        public String Wildcard { get; set; }

        class FileSaveDialogResults : IDisposable
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void FileSaveDialogResultCallback(NativeDialogResult result, IntPtr file);

            FileSaveDialogResultCallback resultCb;
            ResultCallback showModalCallback;
            GCHandle handle;

            public FileSaveDialogResults(ResultCallback callback)
            {
                this.showModalCallback = callback;

                resultCb = (result, filePtr) =>
                {
                    String managedFileString = Marshal.PtrToStringUni(filePtr);
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            this.showModalCallback(result, managedFileString);
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

            public void showNativeDialogModal(NativeOSWindow parent, String message, String defaultDir, String defaultFile, String wildcard)
            {
                handle = GCHandle.Alloc(this, GCHandleType.Normal);
                IntPtr parentPtr = parent != null ? parent._NativePtr : IntPtr.Zero;
                FileSaveDialog_showModal(parentPtr, message, defaultDir, defaultFile, wildcard, resultCb);
            }

            #region PInvoke

            [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
            private static extern void FileSaveDialog_showModal(IntPtr parent, [MarshalAs(UnmanagedType.LPWStr)] String message, [MarshalAs(UnmanagedType.LPWStr)] String defaultDir, [MarshalAs(UnmanagedType.LPWStr)] String defaultFile, [MarshalAs(UnmanagedType.LPWStr)] String wildcard, FileSaveDialogResultCallback resultCallback);

            #endregion
        }
    }
}
