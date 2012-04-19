using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class StopTimelinePlaybackDoAction : DoActionsDataFieldCommand
    {
        public StopTimelinePlaybackDoAction()
        {

        }

        public override void doAction(DataDrivenTimelineGUI gui)
        {
            gui.stopPlayingExample();
        }

        public override string Type
        {
            get
            {
                return "Stop Example Timeline Playback";
            }
        }

        protected StopTimelinePlaybackDoAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
