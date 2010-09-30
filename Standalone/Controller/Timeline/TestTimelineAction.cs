using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;

namespace Medical
{
    class TestTimelineAction : TimelineAction
    {
        private float stopTime;
        private bool finished = false;
        private String text;

        public TestTimelineAction(String text, float startTime, float stopTime)
        {
            this.text = text;
            this.StartTime = startTime;
            this.stopTime = stopTime;
        }

        #region TimelineAction Members

        public override void started(float timelineTime, Clock clock)
        {
            Log.Debug("{0} Started at {1}", text, timelineTime);
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            Log.Debug("{0} Stopped at {1}", text, timelineTime);
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = stopTime < timelineTime;
        }

        public override bool Finished
        {
            get { return finished; }
        }

        #endregion
    }
}
