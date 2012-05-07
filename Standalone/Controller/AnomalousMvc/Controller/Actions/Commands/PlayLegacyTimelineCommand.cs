using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc.Legacy
{
    public class PlayLegacyTimelineCommand : ActionCommand
    {
        public PlayLegacyTimelineCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.setLegacyTimelineMode();
            context.queueTimeline(Timeline);
        }

        [EditableFile(BrowserWindowController.TimelineSearchPattern)]
        public String Timeline { get; set; }

        public override string Type
        {
            get
            {
                return "Play Legacy Timeline";
            }
        }

        protected PlayLegacyTimelineCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
