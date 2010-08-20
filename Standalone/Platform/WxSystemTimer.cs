using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

namespace Medical
{
    class WxSystemTimer : SystemTimer, IDisposable
    {
        IntPtr performanceCounter;

        public WxSystemTimer()
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

        [DllImport("OSHelper")]
        private static extern IntPtr PerformanceCounter_Create();

        [DllImport("OSHelper")]
        private static extern void PerformanceCounter_Delete(IntPtr counter);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool PerformanceCounter_initialize(IntPtr counter);

        [DllImport("OSHelper")]
        private static extern Int64 PerformanceCounter_getCurrentTime(IntPtr counter);
    }
}
