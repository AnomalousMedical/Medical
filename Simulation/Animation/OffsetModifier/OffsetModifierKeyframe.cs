using Engine;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class OffsetModifierKeyframe : Saveable
    {
        private Vector3 translationOffset;
        private Quaternion rotationOffset;

        public OffsetModifierKeyframe()
        {

        }

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

        public float BlendAmount { get; set; }

        public void blendFrom(OffsetModifierKeyframe previousFrame, float percentage, SimObjectFollowerWithRotation follower)
        {
            follower.TranslationOffset = previousFrame.translationOffset.lerp(ref translationOffset, ref percentage);
            follower.RotationOffset = previousFrame.rotationOffset.nlerp(ref rotationOffset, ref percentage);
            follower.computePosition();
        }

        protected OffsetModifierKeyframe(LoadInfo info)
        {

        }

        public void getInfo(SaveInfo info)
        {
            
        }
    }
}
