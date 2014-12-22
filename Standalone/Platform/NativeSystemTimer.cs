using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

namespace Medical
{
    class NativeSystemTimer : SystemTimer, IDisposable
    {
        IntPtr performanceCounter;

        public NativeSystemTimer()
        {
            performanceCounter = PerformanceCounter_Create();
        }

        public void Dispose()
        {
            PerformanceCounter_Delete(performanceCounter);
        }

        public bool initialize()
        {
            return PerformanceCounter_initialize(performanceCounter);
        }

        public long getCurrentTime()
        {
            return PerformanceCounter_getCurrentTime(performanceCounter);
        }

        public bool Accurate
        {
            get
            {
                return PerformanceCounter_isAccurate(performanceCounter);
            }
            set
            {
                PerformanceCounter_setAccurate(performanceCounter, value);
            }
        }

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr PerformanceCounter_Create();

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void PerformanceCounter_Delete(IntPtr counter);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool PerformanceCounter_initialize(IntPtr counter);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern Int64 PerformanceCounter_getCurrentTime(IntPtr counter);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PerformanceCounter_setAccurate(IntPtr counter, bool accurate);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool PerformanceCounter_isAccurate(IntPtr counter);
    }
}
