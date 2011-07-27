using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using MyGUIPlugin;
using Medical.GUI;

namespace Medical
{
    class TimelineActionClipboard
    {
        private CopySaver copySaver = new CopySaver();
        private List<TimelineAction> copiedActions = new List<TimelineAction>();
        private float startTimeZeroOffset;

        public void copy(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (TimelineActionData actionData in timelineData)
            {
                copiedActions.Add(copySaver.copy<TimelineAction>(actionData.Action));
            }
            copiedActions.Sort(delegate(TimelineAction x, TimelineAction y)
            {
                if (x.StartTime > y.StartTime)
                {
                    return 1;
                }
                if (x.StartTime < y.StartTime)
                {
                    return -1;
                }
                return 0;
            });
            if (copiedActions.Count > 0)
            {
                startTimeZeroOffset = copiedActions[0].StartTime;
            }
        }

        public void paste(Timeline currentTimeline, float markerTime)
        {
            foreach (TimelineAction action in copiedActions)
            {
                TimelineAction copiedAction = copySaver.copy<TimelineAction>(action);
                copiedAction.StartTime = copiedAction.StartTime - startTimeZeroOffset + markerTime;
                currentTimeline.addAction(copiedAction);
            }
        }
    }
}