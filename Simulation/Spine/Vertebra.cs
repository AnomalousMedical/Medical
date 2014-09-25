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
    class Vertebra : Interface, SpineSegment
    {
        [Editable]
        String targetSimObjectName;

        [Editable]
        float interpolationAmount = 0.5f;

        [Editable]
        String parentSimObjectName;

        [Editable]
        private String jointSimObjectName;

        [Editable]
        private String parentSpineSegmentSimObjectName;

        [Editable]
        private String parentSpineSegmentName = "Follower";

        [DoNotCopy]
        [DoNotSave]
        SimObject targetSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSpineSegmentSimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion parentRotationOffset;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 centerOfRotationOffset = Vector3.Zero;

        [DoNotCopy]
        [DoNotSave]
        private SpineSegment childSegment;

        protected override void link()
        {
            targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", targetSimObjectName);
            }

            parentSimObject = Owner.getOtherSimObject(parentSimObjectName);
            if (parentSimObject == null)
            {
                blacklist("Cannot find parent SimObject {0}.", parentSimObjectName);
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
                Quaternion inverseParentRot = parentSpineSegmentSimObject.Rotation.inverse();
                Vector3 parentTrans = parentSpineSegmentSimObject.Translation;

                //Figure out the translation in parent space, first, however, we must transform the center of rotation offset by the current rotation.
                //This makes the recorded translation offsets relative to the center of rotation point in world space instead of the center of this SimObject.
                translationOffset = Owner.Translation + Quaternion.quatRotate(ref ourRotation, ref centerOfRotationOffset) - parentTrans;
                translationOffset = Quaternion.quatRotate(inverseParentRot, translationOffset);
            }
            else
            {
                translationOffset = Vector3.Zero;
            }

            rotationOffset = targetSimObject.Rotation.inverse() * Owner.Rotation;
            parentRotationOffset = parentSimObject.Rotation.inverse() * Owner.Rotation;
        }

        protected override void destroy()
        {
            base.destroy();
        }

        public void computePosition()
        {
            Quaternion targetRotation = targetSimObject.Rotation;
            targetRotation = targetRotation * rotationOffset;

            Quaternion parentRotation = parentSimObject.Rotation;
            parentRotation = parentRotation * parentRotationOffset;

            Quaternion rotation = parentRotation.slerp(ref targetRotation, interpolationAmount);
            Vector3 newTrans = parentSpineSegmentSimObject.Translation + Quaternion.quatRotate(parentSpineSegmentSimObject.Rotation, translationOffset);
            newTrans -= Quaternion.quatRotate(ref rotation, ref centerOfRotationOffset);

            updatePosition(ref newTrans, ref rotation);
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

        [DoNotCopy]
        public Vector3 TranslationOffset
        {
            get
            {
                return translationOffset;
            }
            set
            {
                translationOffset = value;
            }
        }

        [DoNotCopy]
        public Quaternion RotationOffset
        {
            get
            {
                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
            }
        }

        protected override void customLoad(Engine.Saving.LoadInfo info)
        {
            parentSpineSegmentSimObjectName = info.GetString("fkParentSimObjectName", parentSpineSegmentSimObjectName);
            base.customLoad(info);
        }
    }
}
