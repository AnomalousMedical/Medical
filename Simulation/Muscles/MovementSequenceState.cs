using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;

namespace Medical.Muscles
{
    public class MovementSequenceState : Saveable
    {
        private float leftCPPosition;
        private float rightCPPosition;
        private Vector3 movingTargetPosition;
        private float muscleForce;
        private float startTime;

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

        #region Saveable Members

        private const String LEFT_CP_POSITION = "leftCPPosition";
        private const String RIGHT_CP_POSITION = "rightCPPosition";
        private const String MOVING_TARGET_POSITION = "movingTargetPosition";
        private const String MUSCLE_FORCE = "muscleForce";
        private const String START_TIME = "startTime";

        protected MovementSequenceState(LoadInfo info)
        {
            leftCPPosition = info.GetFloat(LEFT_CP_POSITION);
            rightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
            movingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
            muscleForce = info.GetFloat(MUSCLE_FORCE);
            startTime = info.GetFloat(START_TIME);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
            info.AddValue(START_TIME, startTime);
        }

        #endregion
    }
}
