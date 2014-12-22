using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Runtime.InteropServices;
using Medical.Controller;

namespace Medical
{
    public class ColorDialog
    {
        public delegate void ResultCallback(NativeDialogResult result, Color color);

        public ColorDialog(NativeOSWindow parent = null)
        {
            Parent = parent;
        }

        /// <summary>
        /// May or may not block the main thread depending on os. Assume it does
        /// not block and handle all results in the callback.
        /// </summary>
        /// <param name="callback">Called when the dialog is done showing with the results.</param>
        /// <returns></returns>
        public void showModal(ResultCallback callback)
        {
            ColorDialogResults results = new ColorDialogResults(callback);
            results.showNativeDialogModal(Parent, Color);
        }

        public NativeOSWindow Parent { get; set; }

        public Color Color { get; set; }

        class ColorDialogResults : IDisposable
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void ColorDialogResultCallback(NativeDialogResult result, Color color);

            ColorDialogResultCallback resultCb;
            ResultCallback showModalCallback;
            GCHandle handle;

            public ColorDialogResults(ResultCallback callback)
            {
                this.showModalCallback = callback;

                resultCb = (result, color) =>
                {
                    ThreadManager.invoke(() =>
                    {
                        try
                        {
                            this.showModalCallback(result, color);
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

            public void showNativeDialogModal(NativeOSWindow parent, Color color)
            {
                handle = GCHandle.Alloc(this, GCHandleType.Normal);
                IntPtr parentPtr = parent != null ? parent._NativePtr : IntPtr.Zero;
                ColorDialog_showModal(parentPtr, color, resultCb);
            }

            #region PInvoke

            [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
            private static extern void ColorDialog_showModal(IntPtr parent, Color color, ColorDialogResultCallback resultCallback);

            #endregion
        }
    }
}
