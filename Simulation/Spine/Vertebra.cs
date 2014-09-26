using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Engine.Attributes;
using Engine.Behaviors.Animation;

namespace Medical.Spine
{
    class Vertebra : BehaviorInterface, SpineSegment
    {
        [Editable]
        String childControlBoneSimObjectName;

        [Editable]
        String parentControlBoneSimObjectName;

        [Editable]
        private String jointSimObjectName;

        [Editable]
        private String parentSpineSegmentSimObjectName;

        [Editable]
        private String parentSpineSegmentName = "Follower";

        [Editable]
        float flexExtInterpolation;

        [Editable]
        float lateralBendingInterpolation;

        [Editable]
        float axialRotInterpolation;

        [DoNotCopy]
        [DoNotSave]
        SimObject childControlBoneSimObject;

        [DoNotCopy]
        [DoNotSave]
        Quaternion childControlBoneRotationOffset;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentControlBoneSimObject;

        [DoNotCopy]
        [DoNotSave]
        Quaternion parentControlBoneRotationOffset;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSpineSegmentSimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 parentSpineSegmentTranslationOffset;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 centerOfRotationOffset = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        private SpineSegment childSegment;

        protected override void link()
        {
            childControlBoneSimObject = Owner.getOtherSimObject(childControlBoneSimObjectName);
            if (childControlBoneSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", childControlBoneSimObjectName);
            }

            parentControlBoneSimObject = Owner.getOtherSimObject(parentControlBoneSimObjectName);
            if (parentControlBoneSimObject == null)
            {
                blacklist("Cannot find parent SimObject {0}.", parentControlBoneSimObjectName);
            }

            parentSpineSegmentSimObject = Owner.getOtherSimObject(parentSpineSegmentSimObjectName);
            if (parentSpineSegmentSimObject == null)
            {
                blacklist("Cannot find parent SpineSegment SimObject {0}.", parentSpineSegmentSimObjectName);
            }
            var spineSegment = parentSpineSegmentSimObject.getElement(parentSpineSegmentName) as SpineSegment;
            if(spineSegment == null)
            {
                blacklist("Cannot find SpineSegment {0} on parent SpineSegment SimObject {1}.", parentSpineSegmentName, parentSpineSegmentSimObjectName);
            }
            spineSegment.setChildSegment(this);

            Vector3 parentSegmentTrans = parentSpineSegmentSimObject.Translation;
            Quaternion inverseParentSegmentRot = parentSpineSegmentSimObject.Rotation.inverse();

            if (!String.IsNullOrEmpty(jointSimObjectName))
            {
                SimObject jointSimObject = Owner.getOtherSimObject(jointSimObjectName);
                if (jointSimObject == null)
                {
                    blacklist("Cannot find Joint SimObject named '{0}'", jointSimObjectName);
                }
                //Find the center of rotation in this object's local space
                centerOfRotationOffset = jointSimObject.Translation - Owner.Translation;

                Quaternion ourRotation = Owner.Rotation;

                //Figure out the translation in parent space, first, however, we must transform the center of rotation offset by the current rotation.
                //This makes the recorded translation offsets relative to the center of rotation point in world space instead of the center of this SimObject.
                parentSpineSegmentTranslationOffset = Owner.Translation + Quaternion.quatRotate(ref ourRotation, ref centerOfRotationOffset) - parentSegmentTrans;
            }
            else
            {
                parentSpineSegmentTranslationOffset = Owner.Translation - parentSegmentTrans;
                centerOfRotationOffset = Vector3.Zero;
            }
            //Rotate the translation offset into the parent coord system
            parentSpineSegmentTranslationOffset = Quaternion.quatRotate(inverseParentSegmentRot, parentSpineSegmentTranslationOffset);

            childControlBoneRotationOffset = childControlBoneSimObject.Rotation.inverse() * Owner.Rotation;
            parentControlBoneRotationOffset = parentControlBoneSimObject.Rotation.inverse() * Owner.Rotation;
        }

        protected override void destroy()
        {
            base.destroy();
        }

        public void computePosition()
        {
            Quaternion childRotation = childControlBoneSimObject.Rotation;
            childRotation = childRotation * childControlBoneRotationOffset;

            Quaternion parentRotation = parentControlBoneSimObject.Rotation;
            parentRotation = parentRotation * parentControlBoneRotationOffset;

            //Slerp method, only allows 1 interpolation value
            //Quaternion rotation = parentRotation.slerp(ref childRotation, interpolationAmount);

            //Parent Local Euler, seems to avoid jitter
            //Rotate child to parent's space
            childRotation *= parentRotation.inverse();

            //Convert to euler
            Vector3 euler = childRotation.getEuler();

            //Blend euler angles from 0 to the given angle and reconstruct quaternion
            Quaternion rotation = new Quaternion(0f.interpolate(euler.x, axialRotInterpolation),
                                                 0f.interpolate(euler.y, lateralBendingInterpolation),
                                                 0f.interpolate(euler.z, flexExtInterpolation)) * parentRotation;

            Vector3 translation = parentSpineSegmentSimObject.Translation + Quaternion.quatRotate(parentSpineSegmentSimObject.Rotation, parentSpineSegmentTranslationOffset);
            translation -= Quaternion.quatRotate(ref rotation, ref centerOfRotationOffset);

            updatePosition(ref translation, ref rotation);
        }

        public void setChildSegment(SpineSegment segment)
        {
            childSegment = segment;
        }

        public void updatePosition()
        {
            computePosition();
            if (childSegment != null)
            {
                childSegment.updatePosition();
            }
        }
    }
}
