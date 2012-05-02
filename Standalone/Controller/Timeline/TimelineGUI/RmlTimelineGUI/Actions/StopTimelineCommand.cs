using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.RmlTimeline.Actions
{
    class StopTimelineCommand : RmlGuiActionCommand
    {
        public StopTimelineCommand()
        {

        }

        public override void execute(RmlTimelineGUI gui)
        {
            gui.stopPlayingExample();
        }

        public override string Type
        {
            get
            {
                return "Stop Timeline Playback";
            }
        }

        protected StopTimelineCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
