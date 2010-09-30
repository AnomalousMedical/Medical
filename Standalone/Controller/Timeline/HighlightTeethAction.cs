using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class HighlightTeethAction : TimelineAction
    {
        public HighlightTeethAction()
        {

        }

        public HighlightTeethAction(bool enable, float startTime)
        {
            this.EnableHighlight = enable;
            this.StartTime = startTime;
        }

        public override void started(float timelineTime, Clock clock)
        {
            TeethController.HighlightContacts = EnableHighlight;
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override bool Finished
        {
            get { return true; }
        }

        public bool EnableHighlight { get; set; }
    }
}
