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
    public abstract class OffsetModifierSequence : Saveable
    {
        private List<OffsetModifierKeyframe> keyframes = new List<OffsetModifierKeyframe>();
        private int currentKeyframeIndex = 0;

        public OffsetModifierSequence()
        {

        }

        public abstract OffsetModifierKeyframe createKeyframe();

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

        public IEnumerable<OffsetModifierKeyframe> Keyframes
        {
            get
            {
                return keyframes;
            }
        }

        /// <summary>
        /// This is a hint for the SimObject used to edit this sequence. It does not mean that is the only
        /// object that this sequence can apply to, but it provides a hint to the editor for an object to
        /// use as a default.
        /// </summary>
        public String SimObjectHint { get; set; }

        /// <summary>
        /// This is a hint for the Player on a SimObject used to edit this sequence. It does not mean that is the only
        /// object that this sequence can apply to, but it provides a hint to the editor for an object to
        /// use as a default.
        /// </summary>
        public String PlayerNameHint { get; set; }

        protected OffsetModifierSequence(LoadInfo info)
        {
            SimObjectHint = info.GetString("SimObjectHint", null);
            PlayerNameHint = info.GetString("FollowerNameHint", null);
            info.RebuildList("Keyframe", keyframes);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("SimObjectHint", SimObjectHint);
            info.AddValue("FollowerNameHint", PlayerNameHint);
            info.ExtractList("Keyframe", keyframes);
        }
    }
}
