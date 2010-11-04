using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class LoadAnotherTimeline : TimelineInstantAction
    {
        private bool showContinuePrompt = false;

        public LoadAnotherTimeline()
        {

        }

        public LoadAnotherTimeline(String targetTimeline)
        {
            this.TargetTimeline = targetTimeline;
        }

        public String TargetTimeline { get; set; }

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
                TimelineController.showContinuePrompt(changeTimelineButton);
            }
            else
            {
                TimelineController.queueTimeline(TimelineController.openTimeline(TargetTimeline));
            }
        }

        private void changeTimelineButton()
        {
            TimelineController.startPlayback(TimelineController.openTimeline(TargetTimeline));
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
