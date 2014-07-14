using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    static class KinectVector4Extensions
    {
        public static Quaternion toEngineQuat(this Microsoft.Kinect.Vector4 src)
        {
            return new Quaternion(src.X, src.Y, src.Z, src.W);
        }
    }
}
