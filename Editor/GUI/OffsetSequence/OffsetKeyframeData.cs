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
        private OffsetSequenceEditor editor;

        public OffsetKeyframeData(OffsetModifierKeyframe keyframe, OffsetModifierSequence sequence, OffsetSequenceEditor editor)
        {
            this.keyframe = keyframe;
            this.sequence = sequence;
            this.editor = editor;
        }

        public override void editingStarted()
        {
            if (editor.Player != null)
            {
                editor.Player.blend(keyframe.BlendAmount);
            }
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
                return keyframe.BlendAmount * OffsetSequenceEditor.Duration;
            }
            set
            {
                keyframe.BlendAmount = value / OffsetSequenceEditor.Duration;
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
