using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Engine.Attributes;

namespace Medical
{
    class SimObjectFollowerWithRotation : Interface
    {
        [Editable]
        String targetSimObjectName;

        [Editable]
        String targetPositionBroadcasterName = "PositionBroadcaster";

        SimObject targetSimObject;
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        PositionBroadcaster broadcaster;

        protected override void link()
        {
            targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", targetSimObjectName);
            }

            broadcaster = targetSimObject.getElement(targetPositionBroadcasterName) as PositionBroadcaster;
            if (broadcaster == null)
            {
                blacklist("Cannot find target PositionBroadcaster '{0}' on SimObject '{1}'", targetPositionBroadcasterName, targetSimObjectName);
            }
            broadcaster.PositionChanged += broadcaster_PositionChanged;

            translationOffset = Owner.Translation - targetSimObject.Translation;
            translationOffset = Quaternion.quatRotate(targetSimObject.Rotation.inverse(), translationOffset);
        }

        protected override void destroy()
        {
            broadcaster.PositionChanged -= broadcaster_PositionChanged;
            base.destroy();
        }

        void broadcaster_PositionChanged(SimObject obj)
        {
            Vector3 trans = targetSimObject.Translation + Quaternion.quatRotate(targetSimObject.Rotation, translationOffset);
            Quaternion rotation = targetSimObject.Rotation;
            updatePosition(ref trans, ref rotation);
        }
    }
}
