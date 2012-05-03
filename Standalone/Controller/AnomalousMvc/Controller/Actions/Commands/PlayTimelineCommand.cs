using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Engine.Reflection;

namespace Medical.Controller.AnomalousMvc
{
    public partial class PlayTimelineCommand : ActionCommand
    {
        public PlayTimelineCommand()
        {

        }

        public override void execute(RmlTimelineGUI gui)
        {
            gui.queueTimeline(Timeline);
        }

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

    public partial class PlayTimelineCommand
    {
        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new BrowseableEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("Timeline")), this, BrowserWindowController.TimelineSearchPattern));
        }
    }
}
