using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Logging;

namespace Medical
{
    public class SimObjectFollower : Behavior
    {
        [Editable]
        String targetSimObjectName;

        SimObject targetSimObject;
        Vector3 translationOffset;

        protected override void link()
        {
            targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", targetSimObjectName);
            }
            translationOffset = Owner.Translation - targetSimObject.Translation;
            translationOffset = Quaternion.quatRotate(targetSimObject.Rotation.inverse(), translationOffset);
            Log.Debug("{0} {1}", targetSimObjectName, translationOffset);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Log.Debug(Quaternion.quatRotate(targetSimObject.Rotation, translationOffset).ToString());
            updateTranslation(targetSimObject.Translation + Quaternion.quatRotate(targetSimObject.Rotation, translationOffset));
        }
    }
}
