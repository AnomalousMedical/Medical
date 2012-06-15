using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    public class HighlightTeethAction : TimelineAction
    {
        private bool finished;

        public HighlightTeethAction()
            :this(false, 0.0f)
        {

        }

        public HighlightTeethAction(bool enable, float startTime)
        {
            this.StartTime = startTime;
        }

        public override void capture()
        {
            
        }

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            TeethController.HighlightContacts = true;
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                started(timelineTime, null);
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            TeethController.HighlightContacts = false;
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void editing()
        {
            TeethController.HighlightContacts = true;
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override void editingCompleted()
        {
            TeethController.HighlightContacts = false;
            base.editingCompleted();
        }

        public override bool Finished
        {
            get { return finished; }
        }

        public override string TypeName
        {
            get
            {
                return "Highlight Teeth";
            }
        }

        #region Saveable

        protected HighlightTeethAction(LoadInfo info)
            :base(info)
        {
            
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        #endregion
    }
}
