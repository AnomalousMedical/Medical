using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LeapMotionPlugin.Wrapper
{
    class Frame : LeapNativeClass
    {
        private static Frame frameInstance = new Frame(IntPtr.Zero);

        public static Frame getFrameWrapper(IntPtr framePtr)
        {
            frameInstance.Ptr = framePtr;
            return frameInstance;
        }

        private Frame(IntPtr framePtr)
            :base(framePtr)
        {
            
        }

        public Int64 Id
        {
            get
            {
                return Frame_id(Ptr);
            }
        }

        public Int64 Timestamp
        {
            get
            {
                return Frame_timestamp(Ptr);
            }
        }

        #region PInvoke

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int64 Frame_id(IntPtr frame);

        [DllImport("LeapMotionWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int64 Frame_timestamp(IntPtr frame);

        #endregion
    }
}
