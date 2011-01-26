using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical
{
    class SystemInfo
    {
        private SystemInfo() { }

        public static uint DisplayCount
        {
            get
            {
                return 1;
            }
        }

        public static Point getDisplayLocation(int displayIndex)
        {
            return new Point(-1, -1);
        }
    }
}
