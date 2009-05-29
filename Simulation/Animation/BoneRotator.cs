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
        Vector3 startRotationRad;

        [DoNotCopy]
        [DoNotSave]
        Vector3 endRotationRad;

        [DoNotCopy]
        [DoNotSave]
        Quaternion newRotQuat = Quaternion.Identity;

        protected override void constructed()
        {
            base.constructed();
            startRotationRad = startRotation * DEG_TO_RAD;
            endRotationRad = endRotation * DEG_TO_RAD;
        }

        public override void positionUpdated(float position, Bone bone)
        {
            if (bone != null)
            {
                Vector3 newRot = startRotationRad.lerp(ref endRotationRad, ref position);
                newRotQuat.setEuler(newRot.x, newRot.y, newRot.z);
                bone.setOrientation(newRotQuat);
                bone.needUpdate(true);
            }
        }
    }
}
