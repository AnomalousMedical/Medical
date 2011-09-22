using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    [Flags]
    public enum NativeDialogResult
    {
        YES = 1,
        OK = 2,
        NO = 4,
        CANCEL = 8,
    }

    public class MessageDialog
    {
        private MessageDialog() { }

        public static void showErrorDialog(String message, String caption)
        {
            MessageDialog_showErrorDialog(IntPtr.Zero, message, caption);
        }

        public static void showErrorDialog(NativeOSWindow parent, String message, String caption)
        {
            MessageDialog_showErrorDialog(parent._NativePtr, message, caption);
        }

        public static NativeDialogResult showQuestionDialog(String message, String caption)
        {
            return MessageDialog_showQuestionDialog(IntPtr.Zero, message, caption);
        }

        public static NativeDialogResult showQuestionDialog(NativeOSWindow parent, String message, String caption)
        {
            return MessageDialog_showQuestionDialog(parent._NativePtr, message, caption);
        }

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void MessageDialog_showErrorDialog(IntPtr parent, String msg, String cap);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern NativeDialogResult MessageDialog_showQuestionDialog(IntPtr parent, String msg, String cap);
    }
}
