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

namespace Medical
{
    class SimObjectPartialFollowerWithRotation : Interface
    {
        [Editable]
        String targetSimObjectName;

        [Editable]
        String targetPositionBroadcasterName = "PositionBroadcaster";

        [Editable]
        float interpolationAmount = 0.5f;

        [Editable]
        String parentSimObjectName;

        [Editable]
        private String jointSimObjectName;

        [DoNotCopy]
        [DoNotSave]
        SimObject targetSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        [DoNotCopy]
        [DoNotSave]
        Vector3 parentTranslationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion parentRotationOffset;

        [DoNotCopy]
        [DoNotSave]
        PositionBroadcaster broadcaster;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 centerOfRotationOffset = Vector3.Zero;

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

            broadcaster = targetSimObject.getElement(targetPositionBroadcasterName) as PositionBroadcaster;
            if (broadcaster == null)
            {
                blacklist("Cannot find target PositionBroadcaster '{0}' on SimObject '{1}'", targetPositionBroadcasterName, targetSimObjectName);
            }
            broadcaster.PositionChanged += broadcaster_PositionChanged;

            if (!String.IsNullOrEmpty(jointSimObjectName))
            {
                SimObject jointSimObject = Owner.getOtherSimObject(jointSimObjectName);
                if (jointSimObject == null)
                {
                    blacklist("Cannot find Joint SimObject named '{0}'", jointSimObjectName);
                }
                //Find the center of rotation in this object's local space
                centerOfRotationOffset = jointSimObject.Translation - Owner.Translation;
            }

            Quaternion ourRotation = Owner.Rotation;
            Vector3 ourTranslation = Owner.Translation;// +Quaternion.quatRotate(ref ourRotation, ref centerOfRotationOffset);

            Quaternion inverseTargetRot = targetSimObject.Rotation.inverse();

            translationOffset = ourTranslation - targetSimObject.Translation;
            translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

            rotationOffset = inverseTargetRot * Owner.Rotation;

            Quaternion inverseParentRot = parentSimObject.Rotation.inverse();

            parentTranslationOffset = ourTranslation - parentSimObject.Translation;
            parentTranslationOffset = Quaternion.quatRotate(inverseParentRot, parentTranslationOffset);

            parentRotationOffset = inverseParentRot * Owner.Rotation;
        }

        protected override void destroy()
        {
            broadcaster.PositionChanged -= broadcaster_PositionChanged;
            base.destroy();
        }

        public void computePosition()
        {
            Quaternion targetRotation = targetSimObject.Rotation;
            Vector3 targetTrans = targetSimObject.Translation + Quaternion.quatRotate(targetRotation, translationOffset);
            targetRotation = targetRotation * rotationOffset;
            //targetTrans -= Quaternion.quatRotate(ref targetRotation, ref centerOfRotationOffset);

            Quaternion parentRotation = parentSimObject.Rotation;
            Vector3 parentTrans = parentSimObject.Translation + Quaternion.quatRotate(parentRotation, parentTranslationOffset);
            parentRotation = parentRotation * parentRotationOffset;
            //parentTrans -= Quaternion.quatRotate(ref parentRotation, ref centerOfRotationOffset);

            Vector3 trans = parentTrans.lerp(ref targetTrans, ref interpolationAmount);
            Quaternion rotation = parentRotation.slerp(ref targetRotation, interpolationAmount);
            updatePosition(ref trans, ref rotation);
        }

        /* rotation only
         Quaternion targetRotation = targetSimObject.Rotation;
            Quaternion parentRotation = parentSimObject.Rotation;
            //targetRotation = Quaternion.Identity.slerp(ref targetRotation, interpolationAmount, true) * Quaternion.Identity.slerp(ref parentRotation, 1.0f - interpolationAmount, true);
            targetRotation = parentRotation.slerp(ref targetRotation, interpolationAmount, true);

            Vector3 trans = targetSimObject.Translation + Quaternion.quatRotate(targetRotation, translationOffset);
            Quaternion rotation = targetRotation * rotationOffset;
            updatePosition(ref trans, ref rotation);
         */

        void broadcaster_PositionChanged(SimObject obj)
        {
            computePosition();
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
    }
}
