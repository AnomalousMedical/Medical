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

        protected override void constructed()
        {
            base.constructed();
            Vector3 startRotationRad = startRotation * DEG_TO_RAD;
            Vector3 endRotationRad = endRotation * DEG_TO_RAD;
            startRotQuat = new Quaternion(startRotationRad.x, startRotationRad.y, startRotationRad.z);
            endRotQuat = new Quaternion(endRotationRad.x, endRotationRad.y, endRotationRad.z);
        }

        public override void positionUpdated(float position)
        {
            if (bone != null)
            {
                Quaternion newRotQuat = startRotQuat.slerp(ref endRotQuat, position);
                bone.setOrientation(newRotQuat);
                bone.needUpdate(true);
            }
        }

        public override BoneManipulatorStateEntry createStateEntry()
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
            }
        }
    }
}
