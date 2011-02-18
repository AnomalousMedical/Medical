using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class HtmlHelpController : IDisposable
    {
        private IntPtr nativePtr;

        public HtmlHelpController(NativeOSWindow parent)
        {
            IntPtr windowPtr = IntPtr.Zero;
            if (parent != null)
            {
                windowPtr = parent._NativePtr;
            }
            nativePtr = HtmlHelpController_new(windowPtr);
        }

        public void Dispose()
        {
            HtmlHelpController_delete(nativePtr);
            nativePtr = IntPtr.Zero;
        }

        public void AddBook(string path)
        {
            HtmlHelpController_AddBook(nativePtr, path);
        }

        public void Display(int index)
        {
            HtmlHelpController_Display(nativePtr, index);
        }

#region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr HtmlHelpController_new(IntPtr parentWindow);

        [DllImport("OSHelper")]
        private static extern void HtmlHelpController_delete(IntPtr controller);

        [DllImport("OSHelper")]
        private static extern void HtmlHelpController_AddBook(IntPtr controller, String path);

        [DllImport("OSHelper")]
        private static extern void HtmlHelpController_Display(IntPtr controller, int index);

#endregion
    }
}
