using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;

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

        public String Timeline { get; set; }

        public override string Type
        {
            get
            {
                return "Play Legacy Timeline";
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new BrowseableEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("Timeline")), this, BrowserWindowController.TimelineSearchPattern));
        }

        protected PlayLegacyTimelineCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
