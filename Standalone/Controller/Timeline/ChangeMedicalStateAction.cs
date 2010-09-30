using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.Controller.Timeline
{
    class ChangeMedicalStateAction : TimelineAction
    {
        private MedicalState state;

        public ChangeMedicalStateAction()
        {

        }

        public ChangeMedicalStateAction(MedicalState state)
        {
            
        }

        public void started(float timelineTime, Clock clock)
        {
            
        }

        public void stopped(float timelineTime, Clock clock)
        {
            
        }

        public void update(float timelineTime, Clock clock)
        {
            
        }

        public float StartTime
        {
            get;
            set;
        }

        public bool Finished
        {
            get { return true; }
        }
    }
}
