using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class StopTimelineCommand : ActionCommand
    {
        public StopTimelineCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.stopPlayingTimeline();
        }

        public override string Type
        {
            get
            {
                return "Stop Timeline Playback";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/TimelineStopIcon";
            }
        }

        protected StopTimelineCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
