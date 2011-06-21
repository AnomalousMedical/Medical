using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Muscles;
using Engine.Saving;
using Logging;

namespace Medical
{
    [TimelineActionProperties("Play Sequence")]
    public class PlaySequenceAction : TimelineAction
    {
        private float lastTime;
        private static CopySaver copySaver = new CopySaver();

        public PlaySequenceAction()
            :this(null, 0.0f, 1.0f)
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
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                started(timelineTime, null);
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            TimelineController.MovementSequenceController.pausePlayback();
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
        }

        public override void editing()
        {
            if (TimelineController != null)
            {
                TimelineController.MovementSequenceController.CurrentSequence = MovementSequence;
            }
            else
            {
                Log.Warning("TimelineController was null when trying to edit PlaySequenceAction.");
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {

        }

        public override void reverseSides()
        {
            MovementSequence.reverseSides();
        }

        public override void capture()
        {
            MovementSequence sequence = TimelineController.MovementSequenceController.CurrentSequence;
            if (sequence != null)
            {
                MovementSequence = copySaver.copy<MovementSequence>(sequence);
            }
            else
            {
                MovementSequence = new MovementSequence();
                MovementSequence.Duration = Duration;
            }
        }

        public override bool Finished
        {
            get
            {
                return lastTime > StartTime + Duration;
            }
        }

        public MovementSequence MovementSequence { get; set; }

        #region Saveable

        private static readonly String MOVEMENT_SEQUENCE = "MovementSequence";

        protected PlaySequenceAction(LoadInfo info)
            : base(info)
        {
            MovementSequence = info.GetValue<MovementSequence>(MOVEMENT_SEQUENCE);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(MOVEMENT_SEQUENCE, MovementSequence);
            base.getInfo(info);
        }

        #endregion
    }
}
