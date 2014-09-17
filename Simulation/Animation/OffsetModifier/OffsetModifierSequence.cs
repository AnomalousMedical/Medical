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
        private List<OffsetModifierKeyframe> keyframes = new List<OffsetModifierKeyframe>();

        [DoNotCopy]
        [DoNotSave]
        private int currentKeyframeIndex = 0;

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

            keyframes.Add(new OffsetModifierKeyframe()
            {
                TranslationOffset = follower.TranslationOffset,
                RotationOffset = follower.RotationOffset,
                BlendAmount = 0,
            });

            keyframes.Add(new OffsetModifierKeyframe()
            {
                TranslationOffset = Vector3.UnitZ * 20,
                RotationOffset = follower.RotationOffset,
                BlendAmount = 0.5f,
            });

            keyframes.Add(new OffsetModifierKeyframe()
            {
                TranslationOffset = Vector3.Zero,
                RotationOffset = Quaternion.Identity,
                BlendAmount = 1,
            });
        }

        public void blend(float amount)
        {
            if (keyframes.Count > 1) //At least 2 key frames
            {
                var start = keyframes[currentKeyframeIndex];
                var end = keyframes[currentKeyframeIndex + 1];
                if (!(start.BlendAmount < amount && end.BlendAmount > amount))
                {
                    //Find the keyframes we should be using if the current ones don't work
                    for (currentKeyframeIndex = 0; currentKeyframeIndex < keyframes.Count && keyframes[currentKeyframeIndex].BlendAmount < amount; ++currentKeyframeIndex)
                    {

                    }
                    if (currentKeyframeIndex > 0)
                    {
                        currentKeyframeIndex--;
                    }
                    start = keyframes[currentKeyframeIndex];
                    end = keyframes[currentKeyframeIndex + 1];
                }
                float length = end.BlendAmount - start.BlendAmount;
                end.blendFrom(start, (amount - start.BlendAmount) / length, follower);
            }
        }
    }
}
