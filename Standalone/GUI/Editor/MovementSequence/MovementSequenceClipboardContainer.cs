using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using MyGUIPlugin;
using Engine.Attributes;
using Engine.Saving;

namespace Medical.GUI
{
    class MovementSequenceClipboardContainer : SaveableClipboardContainer
    {
        [DoNotSave]
        private List<MovementSequenceState> copiedActions = new List<MovementSequenceState>();

        private float startTimeZeroOffset;

        public MovementSequenceClipboardContainer()
        {

        }

        public void addKeyFrames(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (MovementKeyframeData actionData in timelineData)
            {
                copiedActions.Add(actionData.KeyFrame);
            }
            copiedActions.Sort(delegate(MovementSequenceState x, MovementSequenceState y)
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

        public void addKeyFramesToSequence(MovementSequence movementSequence, MovementSequenceEditor editor, float markerTime, float totalDuration)
        {
            foreach (MovementSequenceState copiedAction in copiedActions)
            {
                copiedAction.StartTime = copiedAction.StartTime - startTimeZeroOffset + markerTime;
                if (copiedAction.StartTime > totalDuration)
                {
                    copiedAction.StartTime = totalDuration;
                }
                movementSequence.addState(copiedAction);
                editor.addStateToTimeline(copiedAction);
            }
        }

        protected MovementSequenceClipboardContainer(LoadInfo info)
            :base(info)
        {
            info.RebuildList("KeyFrame", copiedActions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList("KeyFrame", copiedActions);
        }
    }
}
