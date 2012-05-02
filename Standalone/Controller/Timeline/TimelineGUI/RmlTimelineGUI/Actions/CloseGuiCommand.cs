using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.RmlTimeline.Actions
{
    class CloseGuiCommand : RmlGuiActionCommand
    {
        public CloseGuiCommand()
        {

        }

        public override void execute(RmlTimelineGUI gui)
        {
            gui.queueClose();
        }

        public override string Type
        {
            get
            {
                return "Close GUI";
            }
        }

        protected CloseGuiCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
