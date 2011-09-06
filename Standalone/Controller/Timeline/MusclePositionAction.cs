using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Muscle Position")]
    public class MusclePositionAction : TimelineAction
    {
        private MusclePosition targetState;

        private MusclePosition startState = new MusclePosition();
        private float lastTime = 0.0f;

        public MusclePositionAction()
        {
            targetState = new MusclePosition();
            Duration = 1.0f;
        }

        public override void capture()
        {
            targetState.captureState();
        }

        public override void started(float timelineTime, Clock clock)
        {
            start();
        }

        public override void skipTo(float timelineTime)
        {
            start();
            startState.blend(targetState, (timelineTime - StartTime) / Duration);
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
            startState.blend(targetState, (timelineTime - StartTime) / Duration);
        }

        public override void editing()
        {
            start();
            targetState.preview();
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override void reverseSides()
        {
            targetState.reverseSides();
        }

        public override bool Finished
        {
            get 
            {
                return lastTime > StartTime + Duration;
            }
        }

        private void start()
        {
            startState.captureState();
            lastTime = 0.0f;
        }

        protected MusclePositionAction(LoadInfo info)
            :base(info)
        {
            targetState = info.GetValue<MusclePosition>("TargetState");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TargetState", targetState);
        }
    }
}
