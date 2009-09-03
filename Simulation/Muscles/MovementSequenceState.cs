using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Muscles
{
    public class MovementSequenceState
    {
        private float leftCPPosition;
        private float rightCPPosition;
        private Vector3 movingTargetPosition;
        private float muscleForce;
        private float duration;
        private String name;

        public MovementSequenceState(String name)
        {
            this.name = name;
            this.duration = 1.0f;
        }

        public void captureState()
        {
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            muscleForce = movingMuscle.getForce();
            movingTargetPosition = MuscleController.MovingTarget.Offset;
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            leftCPPosition = leftCP.CurrentLocation;
            rightCPPosition = rightCP.CurrentLocation;
        }

        public void blend(MovementSequenceState targetState, float currentTime)
        {
            float blendFactor = currentTime / duration;
            MuscleController.changeForce("MovingMuscleDynamic", muscleForce);
            MuscleController.MovingTarget.Offset = movingTargetPosition.lerp(ref targetState.movingTargetPosition, ref blendFactor);
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            leftCPPosition = leftCP.CurrentLocation;
            rightCPPosition = rightCP.CurrentLocation;
        }

        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }
    }
}
