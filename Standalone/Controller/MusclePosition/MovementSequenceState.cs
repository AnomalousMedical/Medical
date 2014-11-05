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
        private MusclePosition musclePosition;
        private float startTime;

        public MovementSequenceState()
        {
            musclePosition = new MusclePosition();
            musclePosition.Easing = EasingFunction.None;
        }

        public void captureState()
        {
            musclePosition.captureState();
        }

        public void reverseSides()
        {
            musclePosition.reverseSides();
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
            musclePosition.blend(targetState.musclePosition, blendFactor);
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
            startTime = info.GetFloat(START_TIME);

            if (info.Version < 2)
            {
                musclePosition = new MusclePosition();
                musclePosition.Easing = EasingFunction.None;

                musclePosition.MovingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
                musclePosition.MuscleForce = info.GetFloat(MUSCLE_FORCE);
                musclePosition.PelvisChainState = info.GetValue<FKChainState>(PELIVS_CHAIN_STATE, null);
                if (info.Version == 0)
                {
                    musclePosition.LeftCPPosition = MusclePosition.UpgradeCpPosition(info.GetFloat(LEFT_CP_POSITION));
                    musclePosition.RightCPPosition = MusclePosition.UpgradeCpPosition(info.GetFloat(RIGHT_CP_POSITION));
                }
                else
                {
                    musclePosition.LeftCPPosition = info.GetFloat(LEFT_CP_POSITION);
                    musclePosition.RightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
                }
            }
            else
            {
                musclePosition = info.GetValue<MusclePosition>("musclePosition");
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(START_TIME, startTime);

            //For backward compatibility until we ship keep saving in verision 1 format.
            info.Version = 1;
            info.AddValue(LEFT_CP_POSITION, musclePosition.LeftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, musclePosition.RightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, musclePosition.MovingTargetPosition);
            info.AddValue(MUSCLE_FORCE, musclePosition.MuscleForce);
            info.AddValue(PELIVS_CHAIN_STATE, musclePosition.PelvisChainState);

            //Uncomment when upgrade is finished to save into version 2 format
            /*
            info.Version = 2;
            info.AddValue("musclePosition", musclePosition);
            */
        }

        #endregion
    }
}
