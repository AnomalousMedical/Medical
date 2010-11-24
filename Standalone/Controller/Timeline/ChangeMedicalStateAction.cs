using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Change Medical State", 128 / 255f, 0 / 255f, 255 / 255f, GUIType=typeof(Medical.GUI.ChangeMedicalStateProperties))]
    class ChangeMedicalStateAction : TimelineAction
    {
        private bool finished = false;

        public ChangeMedicalStateAction()
            :this(null, 0.0f)
        {
            
        }

        public ChangeMedicalStateAction(MedicalState state, float startTime)
        {
            State = state;
            this.StartTime = StartTime;
            Duration = 1.0f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            TimelineController.MedicalStateController.directBlend(State, Duration);
            finished = false;
        }

        public override void stopped(float timelineTime, Clock clock)
        {

        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void capture()
        {
            State = TimelineController.MedicalStateController.createState("");
        }

        public override void editing()
        {
            TimelineController.MedicalStateController.directBlend(State, 1.0f);
        }

        public override bool Finished
        {
            get { return finished; }
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
