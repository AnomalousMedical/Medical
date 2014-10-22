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
            parentJointTypeMap.Add(JointType.SpineBase, JointType.SpineBase);
            parentJointTypeMap.Add(JointType.SpineMid, JointType.SpineBase);
            parentJointTypeMap.Add(JointType.SpineShoulder, JointType.SpineMid);
            parentJointTypeMap.Add(JointType.Neck, JointType.SpineShoulder);
            parentJointTypeMap.Add(JointType.Head, JointType.Neck);
            parentJointTypeMap.Add(JointType.ShoulderLeft, JointType.SpineShoulder);
            parentJointTypeMap.Add(JointType.ElbowLeft, JointType.ShoulderLeft);
            parentJointTypeMap.Add(JointType.WristLeft, JointType.ElbowLeft);
            parentJointTypeMap.Add(JointType.ThumbLeft, JointType.WristLeft);
            parentJointTypeMap.Add(JointType.HandLeft, JointType.WristLeft);
            parentJointTypeMap.Add(JointType.HandTipLeft, JointType.HandLeft);
            parentJointTypeMap.Add(JointType.ShoulderRight, JointType.SpineShoulder);
            parentJointTypeMap.Add(JointType.ElbowRight, JointType.ShoulderRight);
            parentJointTypeMap.Add(JointType.WristRight, JointType.ElbowRight);
            parentJointTypeMap.Add(JointType.ThumbRight, JointType.WristRight);
            parentJointTypeMap.Add(JointType.HandRight, JointType.WristRight);
            parentJointTypeMap.Add(JointType.HandTipRight, JointType.HandRight);
            parentJointTypeMap.Add(JointType.HipLeft, JointType.SpineBase);
            parentJointTypeMap.Add(JointType.KneeLeft, JointType.HipLeft);
            parentJointTypeMap.Add(JointType.AnkleLeft, JointType.KneeLeft);
            parentJointTypeMap.Add(JointType.FootLeft, JointType.AnkleLeft);
            parentJointTypeMap.Add(JointType.HipRight, JointType.SpineBase);
            parentJointTypeMap.Add(JointType.KneeRight, JointType.HipRight);
            parentJointTypeMap.Add(JointType.AnkleRight, JointType.KneeRight);
            parentJointTypeMap.Add(JointType.FootRight, JointType.AnkleRight);
        }

        public static Quaternion toEngineQuat(this Microsoft.Kinect.Vector4 src)
        {
            return new Quaternion(src.X, src.Y, src.Z, src.W);
        }

        public static Vector3 toSceneCoords(this CameraSpacePoint position)
        {
            return new Vector3(position.X * 1000f * SimulationConfig.MMToUnits, (position.Y - 0.2f) * 1000f * SimulationConfig.MMToUnits - 85f, -((position.Z - 1.5f) * 1000f * SimulationConfig.MMToUnits));
        }

        public static Vector3 toSceneCoords(this Vector3 position)
        {
            return new Vector3(position.x * 1000f * SimulationConfig.MMToUnits, (position.y - 0.2f) * 1000f * SimulationConfig.MMToUnits - 85f, -((position.z - 1.5f) * 1000f * SimulationConfig.MMToUnits));
        }

        public static JointType GetParentJoint(JointType child)
        {
            return parentJointTypeMap[child];
        }
    }
}
