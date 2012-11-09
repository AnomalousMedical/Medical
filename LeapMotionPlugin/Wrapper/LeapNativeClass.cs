using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapMotionPlugin
{
    class LeapNativeClass
    {
        public LeapNativeClass()
        {

        }

        public LeapNativeClass(IntPtr ptr)
        {
            this.Ptr = ptr;
        }

        public IntPtr Ptr { get; protected set; }
    }
}
