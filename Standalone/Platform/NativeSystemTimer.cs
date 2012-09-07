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

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr PerformanceCounter_Create();

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void PerformanceCounter_Delete(IntPtr counter);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool PerformanceCounter_initialize(IntPtr counter);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern Int64 PerformanceCounter_getCurrentTime(IntPtr counter);
    }
}
