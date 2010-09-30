using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    abstract class TimelineAction
    {
        private Timeline timeline;

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
    }
}
