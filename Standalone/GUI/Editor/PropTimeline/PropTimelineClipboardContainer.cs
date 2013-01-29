using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using MyGUIPlugin;
using Engine.Attributes;

namespace Medical.GUI
{
    /// <summary>
    /// A clipboard container for the prop timeline. This will collect up any
    /// actions added to it and can put them back into a timeline. It is
    /// designed to be put into the clipboard directly and copied on the way
    /// back out for best results as it will modify the actions contained within
    /// it.
    /// 
    /// In short copy this object with the clipboard before putting anything on
    /// the timeline or you will have the same objects in multiple places.
    /// </summary>
    class PropTimelineClipboardContainer : SaveableClipboardContainer
    {
        public PropTimelineClipboardContainer()
        {

        }

        [DoNotSave]
        private List<ShowPropSubAction> copiedActions = new List<ShowPropSubAction>();

        private float startTimeZeroOffset;

        /// <summary>
        /// Add actions to the container. This will NOT create any copies.
        /// </summary>
        /// <param name="timelineData"></param>
        public void addActions(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (PropTimelineData actionData in timelineData)
            {
                copiedActions.Add(actionData.Action);
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

        /// <summary>
        /// Add the actions from the container to the timeline. This will NOT
        /// create copies of the actions and relies on the clipboard to do this.
        /// </summary>
        /// <param name="propData"></param>
        /// <param name="propTimeline"></param>
        /// <param name="markerTime"></param>
        /// <param name="totalDuration"></param>
        public void addActionsToTimeline(ShowPropAction propData, PropTimeline propTimeline, float markerTime, float totalDuration)
        {
            foreach (ShowPropSubAction copiedAction in copiedActions)
            {
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
            }
        }

        protected PropTimelineClipboardContainer(LoadInfo info)
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
