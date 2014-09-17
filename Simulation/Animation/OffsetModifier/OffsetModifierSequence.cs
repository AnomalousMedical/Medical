using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class OffsetModifierSequence : Interface
    {
        [Editable]
        String targetSimObjectName = "this";

        [Editable]
        String targetFollowerName = "SimObjectFollower";

        [DoNotCopy]
        [DoNotSave]
        SimObjectFollowerWithRotation follower;

        [DoNotCopy]
        [DoNotSave]
        private OffsetModifierKeyframe originalPos;

        [DoNotCopy]
        [DoNotSave]
        private OffsetModifierKeyframe testPos;

        protected override void constructed()
        {
            base.constructed();
        }

        protected override void link()
        {
            base.link();

            SimObject targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if(targetSimObject == null)
            {
                blacklist("The target SimObject {0} could not be found.", targetSimObjectName);
            }

            follower = targetSimObject.getElement(targetFollowerName) as SimObjectFollowerWithRotation;
            if(follower == null)
            {
                blacklist("The target SimObject {0} does not have a SimObjectFollowerWithRotation named {1}.", targetSimObjectName, targetFollowerName);
            }

            originalPos = new OffsetModifierKeyframe()
            {
                TranslationOffset = follower.TranslationOffset,
                RotationOffset = follower.RotationOffset,
            };

            testPos = new OffsetModifierKeyframe()
            {
                TranslationOffset = Vector3.Zero,
                RotationOffset = Quaternion.Identity
            };
        }

        public void blend(float amount)
        {
            testPos.blendFrom(originalPos, amount, follower);
        }
    }
}
