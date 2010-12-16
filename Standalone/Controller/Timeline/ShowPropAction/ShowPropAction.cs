using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.Controller
{
    class ShowPropAction : TimelineAction
    {
        private bool finished;

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void editing()
        {
            
        }

        public override bool Finished
        {
            get 
            {
                return finished;
            }
        }
    }
}
