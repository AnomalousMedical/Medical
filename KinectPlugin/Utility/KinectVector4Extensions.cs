using Engine;
using Medical;
using Microsoft.Kinect;
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

        public static Vector3 toEngineCoords(this SkeletonPoint Position)
        {
            return new Vector3(Position.X * 1000f * SimulationConfig.MMToUnits, Position.Y * 1000f * SimulationConfig.MMToUnits - 90f, (Position.Z - 2) * 1000f * SimulationConfig.MMToUnits);
        }
    }
}
