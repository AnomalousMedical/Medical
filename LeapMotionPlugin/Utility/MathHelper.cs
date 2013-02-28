using Engine;
using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapMotionPlugin
{
    static class MathHelper
    {
        public static Vector3 GetVector3(Vector vector)
        {
            Vector3 ret = new Vector3(vector.x, vector.y, vector.z);
            vector.Dispose();
            return ret;
        }
    }
}
