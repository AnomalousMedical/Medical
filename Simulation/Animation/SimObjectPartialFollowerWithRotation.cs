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

        [Editable]
        private String fkParentSimObjectName;

        [DoNotCopy]
        [DoNotSave]
        SimObject targetSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject fkParentSimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        //[DoNotCopy]
        //[DoNotSave]
        //Vector3 parentTranslationOffset;

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

            fkParentSimObject = Owner.getOtherSimObject(fkParentSimObjectName);
            if (fkParentSimObject == null)
            {
                blacklist("Cannot find parent SimObject {0}.", fkParentSimObjectName);
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

                Quaternion ourRotation = Owner.Rotation;
                Quaternion inverseParentRot = fkParentSimObject.Rotation.inverse();
                Vector3 parentTrans = fkParentSimObject.Translation;

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
            broadcaster.PositionChanged -= broadcaster_PositionChanged;
            base.destroy();
        }

        public void computePosition()
        {
            Quaternion targetRotation = targetSimObject.Rotation;
            targetRotation = targetRotation * rotationOffset;

            Quaternion parentRotation = parentSimObject.Rotation;
            parentRotation = parentRotation * parentRotationOffset;

            Quaternion rotation = parentRotation.slerp(ref targetRotation, interpolationAmount);
            Vector3 newTrans = fkParentSimObject.Translation + Quaternion.quatRotate(fkParentSimObject.Rotation, translationOffset);
            newTrans -= Quaternion.quatRotate(ref rotation, ref centerOfRotationOffset);

            updatePosition(ref newTrans, ref rotation);
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
