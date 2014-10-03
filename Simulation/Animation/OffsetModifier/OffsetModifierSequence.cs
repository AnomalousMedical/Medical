using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class OffsetModifierSequence : Saveable
    {
        private List<OffsetModifierKeyframe> keyframes = new List<OffsetModifierKeyframe>();
        private int currentKeyframeIndex = 0;

        public OffsetModifierSequence()
        {

        }

        public void addKeyframe(OffsetModifierKeyframe frame)
        {
            keyframes.Add(frame);
        }

        public void removeFrame(OffsetModifierKeyframe frame)
        {
            keyframes.Remove(frame);
        }

        public void sort()
        {
            keyframes.Sort((x, y) =>
            {
                if(x.BlendAmount < y.BlendAmount)
                {
                    return -1;
                }
                else if(x.BlendAmount > y.BlendAmount)
                {
                    return 1;
                }
                return 0;
            });
        }

        public void blend(float amount, SimObjectFollowerWithRotation follower)
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

        protected OffsetModifierSequence(LoadInfo info)
        {
            info.RebuildList("Keyframe", keyframes);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList("Keyframe", keyframes);
        }
    }
}
