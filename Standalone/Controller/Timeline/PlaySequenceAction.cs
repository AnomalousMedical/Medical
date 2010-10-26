using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Muscles;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Play Sequence", 31 / 255f, 73 / 255f, 125 / 255f, GUIType=typeof(Medical.GUI.PlaySequenceProperties))]
    class PlaySequenceAction : TimelineAction
    {
        private float lastTime;

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

        public override void stopped(float timelineTime, Clock clock)
        {
            TimelineController.MovementSequenceController.stopPlayback();
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
        }

        internal void preview()
        {
            TimelineController.MovementSequenceController.CurrentSequence = MovementSequence;
            TimelineController.MovementSequenceController.playCurrentSequence();
        }

        public override void capture()
        {
            MovementSequence sequence = TimelineController.MovementSequenceController.CurrentSequence;
            if (sequence != null)
            {
                MovementSequence = sequence;
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
