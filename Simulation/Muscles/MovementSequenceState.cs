using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Logging;

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

        public void reverseSides()
        {
            float temp = leftCPPosition;
            leftCPPosition = rightCPPosition;
            rightCPPosition = temp;
        }

        public void preview()
        {
            blend(this, 0.0f, 1.0f);
        }

        public void blend(MovementSequenceState targetState, float currentTime, float duration)
        {
            float endTime;
            float blendFactor;
            if (targetState.startTime > startTime)
            {
                endTime = targetState.startTime - startTime;
                blendFactor = (currentTime - startTime) / endTime;
            }
            else
            {
                endTime = targetState.startTime + duration - startTime;
                if (currentTime < startTime)
                {
                    currentTime += duration;
                }
                blendFactor = (currentTime - startTime) / endTime;
            }
            MuscleController.changeForce("MovingMuscleDynamic", targetState.muscleForce);
            MuscleController.MovingTarget.Offset = targetState.movingTargetPosition;
            //MuscleController.MovingTarget.Offset = movingTargetPosition.lerp(ref targetState.movingTargetPosition, ref blendFactor);

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
            //Can keep this around, but get rid of the simulation version checks
            //Be sure to set the version to 1 in the getInfo when you commit to this scene.
            if(info.Version == 0 && SimulationVersionManager.LoadedVersion > SimulationVersionManager.OriginalVersion)
            {
                leftCPPosition = UpgradeCpPosition(leftCPPosition);
                rightCPPosition = UpgradeCpPosition(rightCPPosition);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
            info.AddValue(START_TIME, startTime);
            //When you remove the conversion you need to set the version by uncommenting the line below.
            //info.Version = 1;
        }

        /// <summary>
        /// Conversion function to update old simulation cp positions to the new ones.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static float UpgradeCpPosition(float oldValue)
        {
            if(oldValue < 0.515f)
            {
                return oldValue - 0.01f;
            }
            else
            {
                return (oldValue - .515f) * 1.1590909090909090909090909090909f + .505f;
            }
        }

        #endregion
    }
}
