using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Muscles;

namespace Medical.GUI
{
    class MovementKeyframeData : TimelineData
    {
        private MovementSequenceState keyframe;
        private MovementSequence sequence;

        public MovementKeyframeData(MovementSequenceState keyframe, MovementSequence sequence)
        {
            this.keyframe = keyframe;
            this.sequence = sequence;
        }

        public override string Track
        {
            get { return "Muscle Position"; }
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
                return keyframe.StartTime;
            }
            set
            {
                keyframe.StartTime = value;
                sequence.sortStates();
            }
        }
    }
}
