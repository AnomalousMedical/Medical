using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Medical
{
    public class SystemInfo
    {
        private SystemInfo() { }

        public static uint DisplayCount
        {
            get
            {
                return SystemInfo_getDisplayCount();
            }
        }

        public static Point getDisplayLocation(int displayIndex)
        {
            int x, y;
            SystemInfo_getDisplayLocation(displayIndex, out x, out y);
            return new Point(x, y);
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern uint SystemInfo_getDisplayCount();

        [DllImport("OSHelper")]
        private static extern void SystemInfo_getDisplayLocation(int displayIndex, out int x, out int y);

        #endregion
    }
}
