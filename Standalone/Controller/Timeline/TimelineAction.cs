using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    public abstract class TimelineAction : ActionSequencerAction, Saveable
    {
        private Timeline timeline;

        protected TimelineAction()
        {
            discoverProperties();
        }

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public void _sortAction()
        {
            timeline._actionStartChanged(this);
        }

        /// <summary>
        /// This function will capture the state of the scene to the action. What this does is action specific and may do nothing.
        /// </summary>
        public virtual void capture()
        {

        }

        public abstract void started(float timelineTime, Clock clock);

        public abstract void stopped(float timelineTime, Clock clock);

        public abstract void update(float timelineTime, Clock clock);

        public abstract void editing();

        public virtual void editingCompleted()
        {
            
        }

        public virtual float StartTime { get; set; }

        public virtual float Duration { get; set; }

        public float EndTime
        {
            get
            {
                return StartTime + Duration;
            }
        }

        public abstract bool Finished { get; }

        public String TypeName { get; private set; }

        public TimelineController TimelineController
        {
            get
            {
                return timeline.TimelineController;
            }
        }

        private void discoverProperties()
        {
            try
            {
                TimelineActionProperties properties = (TimelineActionProperties)(GetType().GetCustomAttributes(typeof(TimelineActionProperties), false)[0]);
                TypeName = properties.TypeName;
            }
            catch (Exception)
            {
                throw new Exception("All TimelineActions added to the factory must have a TimelineActionProperties attribute.");
            }
        }

        #region Saveable Members

        private static readonly String START_TIME = "StartTime";
        private static readonly String DURATION = "Duration";

        protected TimelineAction(LoadInfo info)
        {
            discoverProperties();
            StartTime = info.GetSingle(START_TIME, 0.0f);
            Duration = info.GetSingle(DURATION, 0.0f);
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue(START_TIME, StartTime);
            info.AddValue(DURATION, Duration);
        }

        #endregion
    }
}
