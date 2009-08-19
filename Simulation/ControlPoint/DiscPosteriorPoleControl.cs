using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Attributes;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;

namespace Medical
{
    class DiscPosteriorPoleControl : BehaviorObject
    {
        private const float DEG_TO_RAD = 0.0174532925f;

        [Editable]
        String boneName;

        [Editable]
        private Vector3 nineOClockRotation = new Vector3(0.0f, 0.0f, 110.0f);

        [Editable]
        private float nineOClockPosition = .7368f;

        [Editable]
        private float oneOClockPosition = .5132f;

        [DoNotCopy]
        [DoNotSave]
        Bone posteriorPoleRotator;

        [DoNotCopy]
        [DoNotSave]
        Quaternion nineOClockRotationQuat;

        [DoNotCopy]
        [DoNotSave]
        Quaternion startingRot;

        [DoNotCopy]
        [DoNotSave]
        float rotationRange;

        [DoNotCopy]
        [DoNotSave]
        ControlPointBehavior controlPoint;

        [DoNotCopy]
        [DoNotSave]
        Disc disc;

        public DiscPosteriorPoleControl()
        {

        }

        protected DiscPosteriorPoleControl(LoadInfo info)
            :base(info)
        {

        }

        public void initialize(Skeleton skeleton, SimObject owner, ControlPointBehavior controlPoint, Disc disc)
        {
            this.controlPoint = controlPoint;
            this.disc = disc;
            Vector3 rotRad = nineOClockRotation * DEG_TO_RAD;
            nineOClockRotationQuat.setEuler(rotRad.x, rotRad.y, rotRad.z);
            startingRot = owner.Rotation;
            rotationRange = nineOClockPosition - oneOClockPosition;

            posteriorPoleRotator = skeleton.getBone(boneName);
            posteriorPoleRotator.setManuallyControlled(true);
        }

        public void update(float location)
        {
            Quaternion posteriorPopRotation = Quaternion.Identity;
            if (location < nineOClockPosition && location > oneOClockPosition)
            {
                float rotBlend = (location - oneOClockPosition) / rotationRange;
                posteriorPopRotation = startingRot.slerp(ref nineOClockRotationQuat, rotBlend);
            }
            else
            {
                if (location >= nineOClockPosition)
                {
                    posteriorPopRotation = nineOClockRotationQuat;
                }
                if (location <= oneOClockPosition)
                {
                    posteriorPopRotation = startingRot;
                }
            }
            if (controlPoint.CurrentLocation < disc.DiscPopLocation - disc.DiscBackOffset)
            {
                posteriorPoleRotator.setOrientation(posteriorPopRotation);
                posteriorPoleRotator.needUpdate(true);
            }
            else
            {
                float rotBlend = (controlPoint.CurrentLocation - disc.DiscPopLocation + disc.DiscBackOffset) / disc.DiscBackOffset;
                Quaternion posteriorSlipRotation = posteriorPopRotation.slerp(ref startingRot, rotBlend);
                posteriorPoleRotator.setOrientation(posteriorSlipRotation);
                posteriorPoleRotator.needUpdate(true);
            }
        }
    }
}
