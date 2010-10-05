using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Muscles;
using Engine.Saving;

namespace Medical
{
    class PlaySequenceAction : TimelineAction
    {
        public static readonly String Name = "Play Sequence";

        private bool finished = false;

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
            EndTime = timelineTime + Duration;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            if (timelineTime > EndTime)
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

        public override String TypeName
        {
            get { return Name; }
        }

        public float EndTime { get; set; }

        public MovementSequence MovementSequence { get; set; }

        #region Saveable

        private static readonly String END_TIME = "EndTime";
        private static readonly String MOVEMENT_SEQUENCE = "MovementSequence";

        protected PlaySequenceAction(LoadInfo info)
            : base(info)
        {
            EndTime = info.GetFloat(END_TIME);
            MovementSequence = info.GetValue<MovementSequence>(MOVEMENT_SEQUENCE);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(END_TIME, EndTime);
            info.AddValue(MOVEMENT_SEQUENCE, MovementSequence);
            base.getInfo(info);
        }

        #endregion
    }
}
