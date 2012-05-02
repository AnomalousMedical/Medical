﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;
using Engine.Editing;
using Engine.Reflection;

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

        //[Editable]
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
                timeline.AutoFireMultiTimelineStopped = Timeline.AutoFireMultiTimelineStopped;
                TimelineController.queueTimeline(timeline);
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

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addEditableProperty(new BrowseableEditableProperty("Timeline", new PropertyMemberWrapper(this.GetType().GetProperty("TargetTimeline")), this, BrowserWindowController.TimelineSearchPattern));
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
