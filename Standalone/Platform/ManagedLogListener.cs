using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using System.Runtime.InteropServices;

namespace Medical.Platform
{
    class ManagedLogListener : IDisposable
    {
        private IntPtr managedLogListener;
        private LogMessageDelegate logCB;

        public ManagedLogListener()
        {
            logCB = logMessage;
            managedLogListener = ManagedLogListener_create(logCB);
            NativeLog_addLogListener(managedLogListener);
        }

        public void Dispose()
        {
            ManagedLogListener_destroy(managedLogListener);
        }

        private void logMessage(IntPtr message, LogLevel logLevel, IntPtr subsystem)
        {
            Log.Default.sendMessage(Marshal.PtrToStringAnsi(message), logLevel, Marshal.PtrToStringAnsi(subsystem));
        }

        #region PInvoke

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LogMessageDelegate(IntPtr message, LogLevel logLevel, IntPtr subsystem);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ManagedLogListener_create(LogMessageDelegate logDelegate);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ManagedLogListener_destroy(IntPtr managedLogListener);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeLog_addLogListener(IntPtr logListener);

        #endregion
    }
}
