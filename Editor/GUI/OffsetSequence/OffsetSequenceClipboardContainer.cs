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
    class OffsetSequenceClipboardContainer : SaveableClipboardContainer
    {
        [DoNotSave]
        private List<OffsetModifierKeyframe> copiedActions = new List<OffsetModifierKeyframe>();

        private float startTimeZeroOffset;

        public OffsetSequenceClipboardContainer()
        {

        }

        public void addKeyFrames(IEnumerable<TimelineData> timelineData)
        {
            copiedActions.Clear();
            foreach (OffsetKeyframeData actionData in timelineData)
            {
                copiedActions.Add(actionData.KeyFrame);
            }
            copiedActions.Sort((x, y) =>
            {
                if (x.BlendAmount > y.BlendAmount)
                {
                    return 1;
                }
                if (x.BlendAmount < y.BlendAmount)
                {
                    return -1;
                }
                return 0;
            });
            if (copiedActions.Count > 0)
            {
                startTimeZeroOffset = copiedActions[0].BlendAmount;
            }
        }

        public void addKeyFramesToSequence(OffsetModifierSequence sequence, OffsetSequenceEditor editor, float markerTime, float totalDuration)
        {
            foreach (var copiedAction in copiedActions)
            {
                copiedAction.BlendAmount = copiedAction.BlendAmount - startTimeZeroOffset + markerTime;
                if (copiedAction.BlendAmount > totalDuration)
                {
                    copiedAction.BlendAmount = totalDuration;
                }
                sequence.addKeyframe(copiedAction);
                editor.addToTimeline(copiedAction);
            }
            sequence.sort();
        }

        protected OffsetSequenceClipboardContainer(LoadInfo info)
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
