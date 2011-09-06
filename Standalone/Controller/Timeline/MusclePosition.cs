using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;

namespace Medical
{
    class MusclePosition : Saveable
    {
        private float leftCPPosition;
        private float rightCPPosition;
        private Vector3 movingTargetPosition;
        private float muscleForce;

        public MusclePosition()
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
            blend(this, 1.0f);
        }

        public void blend(MusclePosition targetState, float blendFactor)
        {
            MuscleController.changeForce("MovingMuscleDynamic", targetState.muscleForce);
            MuscleController.MovingTarget.Offset = targetState.movingTargetPosition;

            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            float delta = targetState.leftCPPosition - leftCPPosition;
            leftCP.setLocation(leftCPPosition + delta * blendFactor);

            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            delta = targetState.rightCPPosition - rightCPPosition;
            rightCP.setLocation(rightCPPosition + delta * blendFactor);
        }

        #region Saveable Members

        private const String LEFT_CP_POSITION = "leftCPPosition";
        private const String RIGHT_CP_POSITION = "rightCPPosition";
        private const String MOVING_TARGET_POSITION = "movingTargetPosition";
        private const String MUSCLE_FORCE = "muscleForce";

        protected MusclePosition(LoadInfo info)
        {
            leftCPPosition = info.GetFloat(LEFT_CP_POSITION);
            rightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
            movingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
            muscleForce = info.GetFloat(MUSCLE_FORCE);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
        }

        #endregion
    }
}
