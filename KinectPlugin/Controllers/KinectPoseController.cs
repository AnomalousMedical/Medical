using Engine.ObjectManagement;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    interface KinectPoseController
    {
        event Action<KinectPoseController> AllowMovementChanged;

        void createIkControls(SimScene scene);

        void destroyIkControls(SimScene scene);

        void updateControls(Body skel);

        bool DebugVisible { get; set; }

        bool AllowMovement { get; set; }

        bool SkeletonTracking { get; set; }

        bool JawTracking { get; set; }

        void updateFace(FaceAlignment face);
    }
}
