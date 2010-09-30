using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Muscles;

namespace Medical
{
    class PlaySequenceAction : TimelineAction
    {
        private float endTime;
        private bool finished =false;

        public PlaySequenceAction()
        {

        }

        public PlaySequenceAction(MovementSequence movementSequence, float startTime, float duration)
        {
            this.MovementSequence = movementSequence;
            this.StartTime = startTime;
            this.Duration = duration;
        }

        public override void started(float timelineTime, Clock clock)
        {
            TimelineController.MovementSequenceController.CurrentSequence = MovementSequence;
            TimelineController.MovementSequenceController.playCurrentSequence();
            endTime = timelineTime + Duration;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            if (timelineTime > endTime)
            {
                finished = true;
                TimelineController.MovementSequenceController.stopPlayback();
            }
        }

        public override void reset()
        {
            finished = false;
            base.reset();
        }

        public override bool Finished
        {
            get
            {
                return finished;
            }
        }

        public MovementSequence MovementSequence { get; set; }

        public float Duration { get; set; }
    }
}
