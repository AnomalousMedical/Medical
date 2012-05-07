using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Engine.Reflection;
using Medical.Editor;

namespace Medical
{
    public partial class PlayExampleTimelineDoAction : DoActionsDataFieldCommand
    {
        public PlayExampleTimelineDoAction()
        {

        }

        public override void doAction(DataDrivenTimelineGUI gui)
        {
            gui.playExampleTimeline(Timeline);
        }

        [EditableFile(BrowserWindowController.TimelineSearchPattern)]
        public String Timeline { get; set; }

        public override string Type
        {
            get
            {
                return "Play Example Timeline";
            }
        }

        protected PlayExampleTimelineDoAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
