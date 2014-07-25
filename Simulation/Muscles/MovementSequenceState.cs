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
        private FKChainState pelvisChainState;
        private FKChainState interpolatedPelvisChainState = new FKChainState(); //Pooled pelvis chain state used for interpolation, prevents garbage generation. Part of instance state, not saved.

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

            //Setup the pelvis fk chain if available
            FKRoot pelvis;
            if (PoseableObjectsManager.tryGetFkChainRoot("Pelvis", out pelvis))
            {
                pelvisChainState = new FKChainState();
                pelvis.addToChainState(pelvisChainState);
            }
            else
            {
                pelvisChainState = null;
            }
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

            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            float delta = targetState.leftCPPosition - leftCPPosition;
            leftCP.setLocation(leftCPPosition + delta * blendFactor);

            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            delta = targetState.rightCPPosition - rightCPPosition;
            rightCP.setLocation(rightCPPosition + delta * blendFactor);

            FKRoot pelvis;
            if (pelvisChainState != null && targetState.pelvisChainState != null && PoseableObjectsManager.tryGetFkChainRoot("Pelvis", out pelvis))
            {
                interpolatedPelvisChainState.interpolateFrom(pelvisChainState, targetState.pelvisChainState, blendFactor);
                pelvis.applyChainState(interpolatedPelvisChainState);
            }
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
        private const String PELIVS_CHAIN_STATE = "pelvisChainState";

        protected MovementSequenceState(LoadInfo info)
        {
            leftCPPosition = info.GetFloat(LEFT_CP_POSITION);
            rightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
            movingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
            muscleForce = info.GetFloat(MUSCLE_FORCE);
            startTime = info.GetFloat(START_TIME);
            pelvisChainState = info.GetValue<FKChainState>(PELIVS_CHAIN_STATE, null);
            if(info.Version == 0)
            {
                leftCPPosition = UpgradeCpPosition(leftCPPosition);
                rightCPPosition = UpgradeCpPosition(rightCPPosition);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.Version = 1;

            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
            info.AddValue(START_TIME, startTime);
            info.AddValue(PELIVS_CHAIN_STATE, pelvisChainState);
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
