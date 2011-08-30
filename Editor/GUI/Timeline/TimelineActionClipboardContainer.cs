using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Attributes;
using Engine.Saving;

namespace Medical.GUI
{
    class TimelineActionClipboardContainer : SaveableClipboardContainer
    {
        [DoNotSave]
        private List<TimelineAction> copiedActions = new List<TimelineAction>();

        private float startTimeZeroOffset;

        public TimelineActionClipboardContainer()
        {

        }

        public void addActions(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (TimelineActionData actionData in timelineData)
            {
                copiedActions.Add(actionData.Action);
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

        public void addActionsToTimeline(Timeline currentTimeline, float markerTime)
        {
            foreach (TimelineAction copiedAction in copiedActions)
            {
                copiedAction.StartTime = copiedAction.StartTime - startTimeZeroOffset + markerTime;
                currentTimeline.addAction(copiedAction);
            }
        }

        protected TimelineActionClipboardContainer(LoadInfo info)
            :base(info)
        {
            info.RebuildList("Action", copiedActions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList("Action", copiedActions);
        }
    }
}
