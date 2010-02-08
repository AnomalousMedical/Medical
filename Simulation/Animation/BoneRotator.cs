using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using OgreWrapper;
using Engine;
using OgrePlugin;
using Engine.ObjectManagement;
using Engine.Attributes;
using Logging;

namespace Medical
{
    class BoneRotator : BoneManipulator
    {
        private const float DEG_TO_RAD = 0.0174532925f;

        [Editable]
        Vector3 startRotation = Vector3.Zero;

        [Editable]
        Vector3 endRotation = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        Quaternion startRotQuat;

        [DoNotCopy]
        [DoNotSave]
        Quaternion endRotQuat;

        [DoNotCopy]
        [DoNotSave]
        Quaternion newRotQuat = Quaternion.Identity;

        [DoNotCopy]
        [DoNotSave]
        private Quaternion lengthRange;

        [DoNotCopy]
        [DoNotSave]
        private int nonZeroAxis = 0;

        protected override void constructed()
        {
            base.constructed();
            Vector3 startRotationRad = startRotation * DEG_TO_RAD;
            Vector3 endRotationRad = endRotation * DEG_TO_RAD;
            startRotQuat = new Quaternion(startRotationRad.x, startRotationRad.y, startRotationRad.z);
            endRotQuat = new Quaternion(endRotationRad.x, endRotationRad.y, endRotationRad.z);

            lengthRange = endRotQuat - startRotQuat;
            if (lengthRange.x != 0.0f)
            {
                nonZeroAxis = 0;
            }
            else if (lengthRange.y != 0.0f)
            {
                nonZeroAxis = 1;
            }
            else if (lengthRange.z != 0.0f)
            {
                nonZeroAxis = 2;
            }
        }

        protected override void positionUpdated(float position)
        {
            if (bone != null)
            {
                Quaternion newRotQuat = startRotQuat.slerp(ref endRotQuat, position);
                bone.setOrientation(newRotQuat);
                bone.needUpdate(true);
            }
        }

        public override AnimationManipulatorStateEntry createStateEntry()
        {
            return new BoneRotatorStateEntry(UIName, Rotation);
        }

        public override float DefaultPosition
        {
            get
            {
                return 0.0f;
            }
        }

        [DoNotCopy]
        public Quaternion Rotation
        {
            get
            {
                return bone.getOrientation();
            }
            set
            {
                bone.setOrientation(value);
                bone.needUpdate(true);
                switch (nonZeroAxis)
                {
                    case 0:
                        currentPosition = (value.x - startRotQuat.x) / (lengthRange.x);
                        break;
                    case 1:
                        currentPosition = (value.y - startRotQuat.y) / (lengthRange.y);
                        break;
                    case 2:
                        currentPosition = (value.z - startRotQuat.z) / (lengthRange.z);
                        break;
                }
                if (float.IsNaN(currentPosition))
                {
                    currentPosition = 0.0f;
                }
            }
        }
    }
}
