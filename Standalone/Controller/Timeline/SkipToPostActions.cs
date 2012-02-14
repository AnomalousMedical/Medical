using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class SkipToPostActions : TimelineInstantAction
    {
        public SkipToPostActions()
        {

        }

        public override void doAction()
        {
            TimelineController.showContinuePrompt("Skip", skipToEndButton);
            TimelineController.PlaybackStopped += TimelineController_PlaybackStopped;
        }

        void TimelineController_PlaybackStopped(object sender, EventArgs e)
        {
            TimelineController timelineController = (TimelineController)sender;
            timelineController.PlaybackStopped -= TimelineController_PlaybackStopped;
            timelineController.ContinuePrompt.hidePrompt();
        }

        public override void dumpToLog()
        {
            
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        private void skipToEndButton()
        {
            TimelineController.stopPlayback(true);
        }

        protected SkipToPostActions(LoadInfo info)
            :base(info)
        {

        }
    }
}
