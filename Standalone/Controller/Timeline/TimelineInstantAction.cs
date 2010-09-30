using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    abstract class TimelineInstantAction
    {

        private Timeline timeline;

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public abstract void doAction();

        public TimelineController TimelineController
        {
            get
            {
                return timeline.TimelineController;
            }
        }
    }
}
