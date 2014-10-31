using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Microsoft.Kinect.Face;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIKFace
    {
        private KinectIKBone parentBone;
        private float distanceToParent;
        private SimObjectBase faceTargetSimObject;
        private Quaternion currentOrientation;

        public KinectIKFace(KinectIKBone parentBone, float distanceToParent, SimObjectBase faceTargetSimObject)
        {
            this.parentBone = parentBone;
            this.distanceToParent = distanceToParent;
            this.faceTargetSimObject = faceTargetSimObject;
            currentOrientation = Quaternion.Identity;
        }

        public void skeletonUpdated()
        {
            Vector3 newPos = parentBone.Translation + Quaternion.quatRotate(currentOrientation, Vector3.Forward) * distanceToParent;
            faceTargetSimObject.updateTranslation(ref newPos, null);
        }

        public void update(FaceAlignment face)
        {
            currentOrientation = face.FaceOrientation.toEngineQuat().inverse();
        }

        public void render(DebugDrawingSurface debugDraw)
        {
            debugDraw.drawLine(parentBone.Translation, faceTargetSimObject.Translation);
        }
    }
}
