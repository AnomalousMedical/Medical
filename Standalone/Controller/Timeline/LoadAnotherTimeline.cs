using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;

namespace Medical
{
    public class LoadAnotherTimeline : TimelineInstantAction
    {
        private bool showContinuePrompt = false;
        private TimelineController timelineControllerAfterDoAction;

        public LoadAnotherTimeline()
        {

        }

        public LoadAnotherTimeline(String targetTimeline)
        {
            this.TargetTimeline = targetTimeline;
        }

        [EditableFile(BrowserWindowController.TimelineSearchPattern, "Timeline Files")]
        public String TargetTimeline { get; set; }

        [Editable]
        public bool ShowContinuePrompt
        {
            get
            {
                return showContinuePrompt;
            }
            set
            {
                showContinuePrompt = value;
            }
        }

        public override void doAction()
        {
            if (showContinuePrompt)
            {
                timelineControllerAfterDoAction = TimelineController;
                TimelineController.showContinuePrompt("Continue", changeTimelineButton);
            }
            else
            {
                Timeline timeline = TimelineController.openTimeline(TargetTimeline);
                if (timeline != null)
                {
                    TimelineController.queueTimeline(timeline);
                }
                else
                {
                    Log.Error("Cannot load timeline {0}. It cannot be found.", TargetTimeline);
                }
            }
        }

        public override void dumpToLog()
        {
            Log.Debug("LoadAnotherTimeline, Timeline = \"{0}\"", TargetTimeline);
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            if (info.matchesPattern(TargetTimeline))
            {
                info.addMatch(this.GetType(), "", TargetTimeline);
            }
        }

        private void changeTimelineButton()
        {
            timelineControllerAfterDoAction.startPlayback(timelineControllerAfterDoAction.openTimeline(TargetTimeline));
        }

#region Saving

        private const String TARGET_TIMELINE = "TargetTimeline";
        private const String SHOW_CONTINUE_PROMPT = "ShowContinuePrompt";

        protected LoadAnotherTimeline(LoadInfo loadInfo)
            :base(loadInfo)
        {
            TargetTimeline = loadInfo.GetString(TARGET_TIMELINE, null);
            showContinuePrompt = loadInfo.GetBoolean(SHOW_CONTINUE_PROMPT, false);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(TARGET_TIMELINE, TargetTimeline);
            info.AddValue(SHOW_CONTINUE_PROMPT, showContinuePrompt);
        }

#endregion
    }
}
