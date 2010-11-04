using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class LoadAnotherTimeline : TimelineInstantAction
    {
        public LoadAnotherTimeline()
        {

        }

        public LoadAnotherTimeline(String targetTimeline)
        {
            this.TargetTimeline = targetTimeline;
        }

        public String TargetTimeline { get; set; }

        public override void doAction()
        {
            TimelineController.queueTimeline(TimelineController.openTimeline(TargetTimeline));
        }

#region Saving

        private const String TARGET_TIMELINE = "TargetTimeline";

        protected LoadAnotherTimeline(LoadInfo loadInfo)
            :base(loadInfo)
        {
            TargetTimeline = loadInfo.GetString(TARGET_TIMELINE, null);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(TARGET_TIMELINE, TargetTimeline);
        }

#endregion
    }
}
