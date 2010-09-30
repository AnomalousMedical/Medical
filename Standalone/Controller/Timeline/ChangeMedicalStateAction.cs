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

        public override void started(float timelineTime, Clock clock)
        {
            TimelineController.MedicalStateController.directBlend(State, 1.0f);
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override bool Finished
        {
            get { return true; }
        }

        public MedicalState State { get; set; }
    }
}
