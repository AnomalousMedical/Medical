using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class ChangeMedicalStateAction : TimelineAction
    {
        public ChangeMedicalStateAction()
        {

        }

        public ChangeMedicalStateAction(MedicalState state, float startTime)
        {
            State = state;
            this.StartTime = StartTime;
        }

        public void started(float timelineTime, Clock clock)
        {
            TimelineController.Instance.MedicalStateController.directBlend(State, 1.0f);
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

        public MedicalState State { get; set; }
    }
}
