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
            MessageDialog_showErrorDialog(message, caption);
        }

        public static NativeDialogResult showQuestionDialog(String message, String caption)
        {
            return MessageDialog_showQuestionDialog(message, caption);
        }

        [DllImport("OSHelper")]
        private static extern void MessageDialog_showErrorDialog(String msg, String cap);

        [DllImport("OSHelper")]
        private static extern NativeDialogResult MessageDialog_showQuestionDialog(String msg, String cap);
    }
}
