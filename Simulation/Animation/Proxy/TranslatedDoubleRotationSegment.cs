using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    public class TranslatedDoubleRotationSegment : ProxyChainSegmentBehavior
    {
        [Editable]
        private String proxySimObjectName;

        [Editable]
        private String secondaryRotationParentSimObjectName;

        [Editable]
        private float rotationInterpolationAmount = 0.5f;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxySimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        [DoNotCopy]
        [DoNotSave]
        SimObject secondaryRotationParentSimObject;

        [DoNotCopy]
        [DoNotSave]
        Quaternion secondaryRotationOffset;

        protected override void link()
        {
            base.link();

            proxySimObject = Owner.getOtherSimObject(proxySimObjectName);
            if (proxySimObject == null)
            {
                blacklist("Cannot find proxy SimObject '{0}'.", proxySimObjectName);
            }

            secondaryRotationParentSimObject = Owner.getOtherSimObject(secondaryRotationParentSimObjectName);
            if (secondaryRotationParentSimObject == null)
            {
                blacklist("Cannot find secondary rotation parent SimObject '{0}'.", secondaryRotationParentSimObjectName);
            }

            Quaternion inverseTargetRot = parentSegmentSimObject.Rotation.inverse();

            translationOffset = Owner.Translation - parentSegmentSimObject.Translation;
            translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

            rotationOffset = inverseTargetRot * Owner.Rotation;

            secondaryRotationOffset = secondaryRotationParentSimObject.Rotation.inverse() * Owner.Rotation;
        }

        protected override void computePosition()
        {
            Vector3 trans = parentSegmentSimObject.Translation + Quaternion.quatRotate(parentSegmentSimObject.Rotation, translationOffset);
            Quaternion primaryRotation = parentSegmentSimObject.Rotation * rotationOffset;
            Quaternion secondaryRotation = secondaryRotationParentSimObject.Rotation * secondaryRotationOffset;
            Quaternion rotation = primaryRotation.nlerp(ref secondaryRotation, ref rotationInterpolationAmount);

            updatePosition(ref trans, ref rotation);
        }
    }
}
