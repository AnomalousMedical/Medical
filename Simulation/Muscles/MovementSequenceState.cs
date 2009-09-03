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
        private float startTime;
        private int index;

        public MovementSequenceState()
        {
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

        internal void blend(MovementSequenceState targetState, float currentTime, float duration)
        {
            float endTime = targetState.StartTime > startTime ? targetState.startTime - startTime : duration - startTime;
            float blendFactor = (currentTime - startTime) / endTime;
            MuscleController.changeForce("MovingMuscleDynamic", muscleForce);
            MuscleController.MovingTarget.Offset = movingTargetPosition.lerp(ref targetState.movingTargetPosition, ref blendFactor);

            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            float delta = targetState.leftCPPosition - leftCPPosition;
            leftCP.setLocation(leftCPPosition + delta * blendFactor);

            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            delta = targetState.rightCPPosition - rightCPPosition;
            rightCP.setLocation(rightCPPosition + delta * blendFactor);
        }

        /// <summary>
        /// Internal tracking of where this sequence goes in the list. This is
        /// the start time for the state.
        /// </summary>
        public float StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }
    }
}
