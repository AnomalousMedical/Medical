using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    abstract class TimelineAction : Saveable
    {
        private Timeline timeline;

        protected TimelineAction()
        {

        }

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public abstract void started(float timelineTime, Clock clock);

        public abstract void stopped(float timelineTime, Clock clock);

        public abstract void update(float timelineTime, Clock clock);

        public virtual void reset()
        {

        }

        public virtual float StartTime { get; set; }

        public virtual float Duration { get; set; }

        public abstract bool Finished { get; }

        public TimelineController TimelineController
        {
            get
            {
                return timeline.TimelineController;
            }
        }

        #region Saveable Members

        private static readonly String START_TIME = "StartTime";
        private static readonly String DURATION = "Duration";

        protected TimelineAction(LoadInfo info)
        {
            StartTime = info.GetSingle(START_TIME, 0.0f);
            Duration = info.GetSingle(DURATION, 0.0f);
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue(START_TIME, StartTime);
            info.AddValue(DURATION, Duration);
        }

        #endregion
    }
}
