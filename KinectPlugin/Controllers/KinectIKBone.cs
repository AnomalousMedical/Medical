using Engine.ObjectManagement;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIKBone
    {
        private List<KinectIKBone> children = new List<KinectIKBone>();

        public KinectIKBone(JointType jointType, float DistanceToParent)
        {

        }

        public void update()
        {

        }

        public JointType JointType { get; private set; }

        public float DistanceToParent { get; private set; }

        public SimObjectBase SimObject { get; private set; }
    }
}
