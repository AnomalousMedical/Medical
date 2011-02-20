using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public abstract class TimelineInstantAction : Saveable
    {
        private Timeline timeline;

        protected TimelineInstantAction()
        {

        }

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public abstract void doAction();

        public abstract void dumpToLog();

        public abstract void findFileReference(TimelineStaticInfo info);

        public TimelineController TimelineController
        {
            get
            {
                return timeline.TimelineController;
            }
        }

        public Timeline Timeline
        {
            get
            {
                return timeline;
            }
        }

        #region Saveable Members

        protected TimelineInstantAction(LoadInfo info)
        {

        }

        public virtual void getInfo(SaveInfo info)
        {
            
        }

        #endregion
    }
}
