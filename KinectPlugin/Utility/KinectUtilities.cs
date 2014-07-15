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
    static class KinectUtilities
    {
        private static Dictionary<JointType, JointType> parentJointTypeMap = new Dictionary<JointType, JointType>();

        static KinectUtilities()
        {
            parentJointTypeMap.Add(JointType.HipCenter, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.Spine, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.ShoulderCenter, JointType.Spine);
            parentJointTypeMap.Add(JointType.Head, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ShoulderLeft, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ElbowLeft, JointType.ShoulderLeft);
            parentJointTypeMap.Add(JointType.WristLeft, JointType.ElbowLeft);
            parentJointTypeMap.Add(JointType.HandLeft, JointType.WristLeft);
            parentJointTypeMap.Add(JointType.ShoulderRight, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ElbowRight, JointType.ShoulderRight);
            parentJointTypeMap.Add(JointType.WristRight, JointType.ElbowRight);
            parentJointTypeMap.Add(JointType.HandRight, JointType.WristRight);
            parentJointTypeMap.Add(JointType.HipLeft, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.KneeLeft, JointType.HipLeft);
            parentJointTypeMap.Add(JointType.AnkleLeft, JointType.KneeLeft);
            parentJointTypeMap.Add(JointType.FootLeft, JointType.AnkleLeft);
            parentJointTypeMap.Add(JointType.HipRight, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.KneeRight, JointType.HipRight);
            parentJointTypeMap.Add(JointType.AnkleRight, JointType.KneeRight);
            parentJointTypeMap.Add(JointType.FootRight, JointType.AnkleRight);
        }

        public static Quaternion toEngineQuat(this Microsoft.Kinect.Vector4 src)
        {
            return new Quaternion(src.X, src.Y, src.Z, src.W);
        }

        public static Vector3 toEngineCoords(this SkeletonPoint position)
        {
            return new Vector3(position.X * 1000f * SimulationConfig.MMToUnits, position.Y * 1000f * SimulationConfig.MMToUnits - 85f, -((position.Z - 1.5f) * 1000f * SimulationConfig.MMToUnits));
            //return new Vector3(position.X * 1000f * SimulationConfig.MMToUnits, position.Y * 1000f * SimulationConfig.MMToUnits - 90f, (position.Z - 2) * 1000f * SimulationConfig.MMToUnits);
        }

        public static JointType GetParentJoint(JointType child)
        {
            return parentJointTypeMap[child];
        }
    }
}
