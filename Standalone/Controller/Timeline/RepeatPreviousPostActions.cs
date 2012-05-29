using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;

namespace Medical
{
    public class RepeatPreviousPostActions : TimelineInstantAction
    {
        public RepeatPreviousPostActions()
        {

        }

        public override void doAction()
        {
            Timeline previousTimeline = TimelineController.PreviousTimeline;
            if (previousTimeline != null)
            {
                foreach (TimelineInstantAction action in previousTimeline.duplicatePostActions())
                {
                    Timeline.addPostAction(action);
                }
                Timeline.removePostAction(this);
            }
        }

        public override void dumpToLog()
        {
            Log.Debug("RepeatPreviousPostActions");
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        #region Saving

        protected RepeatPreviousPostActions(LoadInfo info)
            : base(info)
        {

        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        #endregion
    }
}
