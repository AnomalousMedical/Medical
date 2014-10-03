using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Muscles;

namespace Medical.GUI
{
    class OffsetKeyframeData : TimelineData
    {
        private OffsetModifierKeyframe keyframe;
        private OffsetModifierSequence sequence;

        public OffsetKeyframeData(OffsetModifierKeyframe keyframe, OffsetModifierSequence sequence)
        {
            this.keyframe = keyframe;
            this.sequence = sequence;
        }

        public override void editingStarted()
        {
            //Call this with some kind of follower
            //keyframe.preview();
        }

        public override string Track
        {
            get { return "Offset Position"; }
        }

        public override float _Duration
        {
            get
            {
                return 0.0f;
            }
            set
            {
                
            }
        }

        public override float _StartTime
        {
            get
            {
                return keyframe.BlendAmount;
            }
            set
            {
                keyframe.BlendAmount = value;
                sequence.sort();
            }
        }

        public OffsetModifierKeyframe KeyFrame
        {
            get
            {
                return keyframe;
            }
        }
    }
}
