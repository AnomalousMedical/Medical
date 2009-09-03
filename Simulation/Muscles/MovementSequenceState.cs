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
        private float timeIndex;
        private int index;

        public MovementSequenceState()
        {
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
            float blendFactor = (currentTime - timeIndex) / duration;
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
        /// Determine if time is within this state.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool isTimePart(float time)
        {
            return time >= timeIndex && time < timeIndex + duration;
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

        /// <summary>
        /// Internal tracking of where this sequence goes in the list. This is
        /// the start time for the state.
        /// </summary>
        internal float TimeIndex
        {
            get
            {
                return timeIndex;
            }
            set
            {
                timeIndex = value;
            }
        }

        internal int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
    }
}
