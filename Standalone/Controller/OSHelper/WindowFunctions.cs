using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using Logging;
using Engine;

namespace Medical.Controller
{
    public class WindowFunctions
    {
        private WindowFunctions()
        {

        }

        public static void pumpMessages()
        {
            WindowFunctions_pumpMessages();
        }

#region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WindowFunctions_pumpMessages();

#endregion
    }
}
