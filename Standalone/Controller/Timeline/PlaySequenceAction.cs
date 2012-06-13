using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.Muscles;
using Engine.Saving;
using Logging;
using Engine.Editing;

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
            PauseOnStop = true;
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
            if (PauseOnStop)
            {
                TimelineController.MovementSequenceController.pausePlayback();
            }
            else
            {
                TimelineController.MovementSequenceController.stopPlayback();
            }
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

        /// <summary>
        /// If this is true the MovementSequenceController will get a pause command, if false it will get a stop command.
        /// </summary>
        [Editable]
        public bool PauseOnStop { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Capture", (callback, caller) =>
            {
                capture();
            }));
        }

        #region Saveable

        private static readonly String MOVEMENT_SEQUENCE = "MovementSequence";
        private static readonly String PAUSE_ON_STOP = "PauseOnStop";

        protected PlaySequenceAction(LoadInfo info)
            : base(info)
        {
            MovementSequence = info.GetValue<MovementSequence>(MOVEMENT_SEQUENCE);
            PauseOnStop = info.GetBoolean(PAUSE_ON_STOP, true);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(MOVEMENT_SEQUENCE, MovementSequence);
            info.AddValue(PAUSE_ON_STOP, PauseOnStop);
            base.getInfo(info);
        }

        #endregion
    }
}
