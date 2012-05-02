using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Engine.Reflection;

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

    public partial class PlayExampleTimelineDoAction
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new BrowseableEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("Timeline")), this, BrowserWindowController.TimelineSearchPattern));
        }
    }
}
