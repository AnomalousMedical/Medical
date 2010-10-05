using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    class ChangeMedicalStateAction : TimelineAction
    {
        public static readonly String Name = "Change Medical State";

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

        public override String TypeName
        {
            get { return Name; }
        }

        public MedicalState State { get; set; }

        #region Saveable

        private static readonly String MEDICAL_STATE = "MedicalState";

        protected ChangeMedicalStateAction(LoadInfo info)
            :base(info)
        {
            State = info.GetValue<MedicalState>(MEDICAL_STATE);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(MEDICAL_STATE, State);
            base.getInfo(info);
        }

        #endregion
    }
}
