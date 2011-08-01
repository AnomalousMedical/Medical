using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using MyGUIPlugin;
using Medical.Muscles;

namespace Medical.GUI
{
    class MovementSequenceClipboard
    {
        private CopySaver copySaver = new CopySaver();
        private List<MovementSequenceState> copiedActions = new List<MovementSequenceState>();
        private float startTimeZeroOffset;

        public void copy(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (MovementKeyframeData actionData in timelineData)
            {
                copiedActions.Add(copySaver.copy<MovementSequenceState>(actionData.KeyFrame));
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

        public void paste(MovementSequence movementSequence, MovementSequenceEditor editor, float markerTime, float totalDuration)
        {
            foreach (MovementSequenceState action in copiedActions)
            {
                MovementSequenceState copiedAction = copySaver.copy<MovementSequenceState>(action);
                copiedAction.StartTime = copiedAction.StartTime - startTimeZeroOffset + markerTime;
                if (copiedAction.StartTime > totalDuration)
                {
                    copiedAction.StartTime = totalDuration;
                }
                movementSequence.addState(copiedAction);
                editor.addStateToTimeline(copiedAction);
            }
        }
    }
}
