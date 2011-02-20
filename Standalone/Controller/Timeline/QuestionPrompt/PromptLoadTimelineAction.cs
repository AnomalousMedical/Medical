using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;

namespace Medical
{
    public class PromptLoadTimelineAction : PromptAnswerAction
    {
        public PromptLoadTimelineAction()
        {

        }

        public PromptLoadTimelineAction(String timeline)
        {
            TargetTimeline = timeline;
        }

        public void execute(TimelineController timelineController)
        {
            if (TargetTimeline != null)
            {
                timelineController.startPlayback(timelineController.openTimeline(TargetTimeline));
            }
            else
            {
                Log.Warning("Target timeline was null");
            }
        }

        public void dumpToLog()
        {
            Log.Debug("|--- PromptLoadTimelineAction");
            Log.Debug("|------- Timeline {0}", TargetTimeline);
        }

        public void findFileReference(TimelineStaticInfo info, String answerText)
        {
            if (info.matchesPattern(TargetTimeline))
            {
                info.addMatch(this.GetType(), answerText);
            }
        }

        public String TargetTimeline { get; set; }

        #region Saveable Members

        private const String TARGET_TIMELINE = "TargetTimeline";

        protected PromptLoadTimelineAction(LoadInfo info)
        {
            TargetTimeline = info.GetString(TARGET_TIMELINE, null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(TARGET_TIMELINE, TargetTimeline);
        }

        #endregion
    }
}
