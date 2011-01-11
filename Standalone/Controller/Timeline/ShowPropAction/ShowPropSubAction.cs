using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Platform;

namespace Medical
{
    public abstract class ShowPropSubAction : Saveable
    {
        //private Timeline timeline;

        protected ShowPropSubAction()
        {
            
        }

        //internal void _setTimeline(Timeline timeline)
        //{
        //    this.timeline = timeline;
        //}

        //public void _sortAction()
        //{
        //    timeline._actionStartChanged(this);
        //}

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

        public abstract bool Finished { get; }

        public abstract String TypeName { get; }

        #region Saveable Members

        private static readonly String START_TIME = "StartTime";
        private static readonly String DURATION = "Duration";

        protected ShowPropSubAction(LoadInfo info)
        {
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
