using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropTimelineClipboard
    {
        private CopySaver copySaver = new CopySaver();
        private List<ShowPropSubAction> copiedActions = new List<ShowPropSubAction>();
        private float startTimeZeroOffset;

        public void copy(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (PropTimelineData actionData in timelineData)
            {
                copiedActions.Add(copySaver.copy<ShowPropSubAction>(actionData.Action));
            }
            copiedActions.Sort(delegate(ShowPropSubAction x, ShowPropSubAction y)
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

        public void paste(ShowPropAction propData, PropTimeline propTimeline, float markerTime, float totalDuration)
        {
            foreach (ShowPropSubAction action in copiedActions)
            {
                ShowPropSubAction copiedAction = copySaver.copy<ShowPropSubAction>(action);
                copiedAction.StartTime = copiedAction.StartTime - startTimeZeroOffset + markerTime;
                if (copiedAction.EndTime > totalDuration)
                {
                    copiedAction.StartTime -= (copiedAction.EndTime - totalDuration);
                    if (copiedAction.StartTime < 0)
                    {
                        copiedAction.StartTime = 0.0f;
                        copiedAction.Duration = totalDuration;
                    }
                }
                propData.addSubAction(copiedAction);
                propTimeline.addSubActionData(copiedAction);
            }
        }
    }
}
