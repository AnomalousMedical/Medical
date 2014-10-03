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
    public class OffsetModifierPlayer : BehaviorInterface
    {
        [Editable]
        String targetSimObjectName = "this";

        [Editable]
        String targetFollowerName = "SimObjectFollower";

        [Editable]
        String blendDriverSimObjectName = "this";

        [Editable]
        String blendDriverElementName = "BlendDriver";

        [DoNotCopy]
        [DoNotSave]
        BlendDriver blendDriver;

        [DoNotCopy]
        [DoNotSave]
        SimObjectFollowerWithRotation follower;

        [DoNotCopy]
        [DoNotSave]
        OffsetModifierSequence currentSequence;

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

            SimObject blendDriverSimObject = Owner.getOtherSimObject(blendDriverSimObjectName);
            if (blendDriverSimObject == null)
            {
                blacklist("The blend driver SimObject {0} could not be found.", blendDriverSimObjectName);
            }

            blendDriver = targetSimObject.getElement(blendDriverElementName) as BlendDriver;
            if (blendDriver == null)
            {
                blacklist("The blend driver SimObject {0} does not have a BlendDriver named {1}.", blendDriverSimObjectName, blendDriverElementName);
            }

            blendDriver.BlendAmountChanged += blendDriver_BlendAmountChanged;

            currentSequence = new OffsetModifierSequence();

            currentSequence.addKeyframe(new OffsetModifierKeyframe()
            {
                TranslationOffset = follower.TranslationOffset,
                RotationOffset = follower.RotationOffset,
                BlendAmount = 0,
            });

            currentSequence.addKeyframe(new OffsetModifierKeyframe()
            {
                TranslationOffset = Vector3.UnitZ * 20,
                RotationOffset = follower.RotationOffset,
                BlendAmount = 0.5f,
            });

            currentSequence.addKeyframe(new OffsetModifierKeyframe()
            {
                TranslationOffset = Vector3.Zero,
                RotationOffset = Quaternion.Identity,
                BlendAmount = 1,
            });
        }

        protected override void destroy()
        {
            blendDriver.BlendAmountChanged -= blendDriver_BlendAmountChanged;
            base.destroy();
        }

        void blendDriver_BlendAmountChanged(BlendDriver obj)
        {
            blend(obj.BlendAmount);
        }

        void blend(float amount)
        {
            currentSequence.blend(amount, follower);
        }
    }
}
