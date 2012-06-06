using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Engine.Reflection;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class PlayTimelineCommand : ActionCommand
    {
        public PlayTimelineCommand()
        {

        }

        public PlayTimelineCommand(String timeline)
        {
            this.Timeline = timeline;
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueTimeline(Timeline);
        }

        [EditableFile(BrowserWindowController.TimelineSearchPattern, "Timeline Files")]
        public String Timeline { get; set; }

        public override string Type
        {
            get
            {
                return "Play Timeline";
            }
        }

        protected PlayTimelineCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
