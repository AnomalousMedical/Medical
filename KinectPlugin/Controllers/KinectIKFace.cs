using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Medical;
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
        private const float fullyOpenProtrusion = .653f;
        private const float fullyOpenHinge = -17.997f;
        private const float additionalOneSideProtrusion = .4f;

        private float openMaxValue = 0.55f;
        private float protrusionMaxValue = 0.25f;

        private KinectIKBone parentBone;
        private float distanceToParent;
        private SimObjectBase faceTargetSimObject;
        private Quaternion currentOrientation;

        //Jaw
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private MuscleBehavior movingMuscle;
        private MovingMuscleTarget movingMuscleTarget;
        private float neutralProtrusion;

        public KinectIKFace(KinectIKBone parentBone, float distanceToParent, SimObjectBase faceTargetSimObject)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            movingMuscleTarget = MuscleController.MovingTarget;
            neutralProtrusion = leftCP.NeutralLocation;

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

            if (JawTracking)
            {
                float openInterpolate = Math.Min(face.AnimationUnits[FaceShapeAnimations.JawOpen] / openMaxValue, 1.0f);
                float protrusion = neutralProtrusion.interpolate(fullyOpenProtrusion, openInterpolate);

                float leftAdditiontalSlide = 0f;
                float rightAdditiontalSlide = 0f;

                //Uncomment to try left/right sliding
                //float slide = face.AnimationUnits[FaceShapeAnimations.JawSlideRight];
                //if(slide > 0)
                //{
                //    rightAdditiontalSlide = 0f.interpolate(additionalOneSideProtrusion, Math.Min(slide / protrusionMaxValue, 1.0f));
                //}
                //else
                //{
                //    leftAdditiontalSlide = 0f.interpolate(additionalOneSideProtrusion, Math.Min(-slide / protrusionMaxValue, 1.0f));
                //}

                leftCP.setLocation(Math.Min(protrusion + leftAdditiontalSlide, 1.0f));
                rightCP.setLocation(Math.Min(protrusion + rightAdditiontalSlide, 1.0f));
                movingMuscleTarget.Offset = new Vector3(0, 0f.interpolate(fullyOpenHinge, openInterpolate), 0.0f);
                movingMuscle.changeForce(70);

                //Logging.Log.Debug("Jaw pos {0} slide {1}", face.AnimationUnits[FaceShapeAnimations.JawOpen], face.AnimationUnits[FaceShapeAnimations.JawSlideRight]);
            }
        }

        public void render(DebugDrawingSurface debugDraw)
        {
            debugDraw.drawLine(parentBone.Translation, faceTargetSimObject.Translation);
        }

        public bool JawTracking { get; set; }
    }
}
