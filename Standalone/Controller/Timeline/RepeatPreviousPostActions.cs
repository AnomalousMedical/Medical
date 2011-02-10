using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

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
            else
            {
                TimelineController._fireMultiTimelineStopEvent();
            }
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
