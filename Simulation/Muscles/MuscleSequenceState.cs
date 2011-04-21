using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;
using Engine;

namespace Medical
{
    [SingleEnum]
    enum ControlPointModification
    {
        DoNothing,
        Move,
        Reset,
        Stop
    }

    class MuscleSequenceState : BehaviorObject
    {
        private const String RightTemporalisDynamic = "RightTemporalisDynamic";
        private const String RightMasseterDynamic = "RightMasseterDynamic";
        private const String RightMedialPterygoidDynamic = "RightMedialPterygoidDynamic";
        private const String RightLateralPterygoidDynamic = "RightLateralPterygoidDynamic";
        private const String RightDigastricDynamic = "RightDigastricDynamic";

        private const String LeftTemporalisDynamic = "LeftTemporalisDynamic";
        private const String LeftMasseterDynamic = "LeftMasseterDynamic";
        private const String LeftMedialPterygoidDynamic = "LeftMedialPterygoidDynamic";
        private const String LeftLateralPterygoidDynamic = "LeftLateralPterygoidDynamic";
        private const String LeftDigastricDynamic = "LeftDigastricDynamic";

        [Editable]
        private float duration = 0.0f;

        [Editable]
        private float rightTemporalisForce = 0.0f;
        [Editable]
        private float rightMasseterForce = 0.0f;
        [Editable]
        private float rightMedialPterygoidForce = 0.0f;
        [Editable]
        private float rightLateralPterygoidForce = 0.0f;
        [Editable]
        private float rightDigastricForce = 0.0f;

        [Editable]
        private float leftTemporalisForce = 0.0f;
        [Editable]
        private float leftMasseterForce = 0.0f;
        [Editable]
        private float leftMedialPterygoidForce = 0.0f;
        [Editable]
        private float leftLateralPterygoidForce = 0.0f;
        [Editable]
        private float leftDigastricForce = 0.0f;

        [Editable]
        private bool teethLoose = false;

        [Editable]
        private ControlPointModification controlPointModification = ControlPointModification.DoNothing;

        [Editable]
        private float controlPointMoveSpeed = 0.0f;

        [Editable]
        private float controlPointTarget = 0.0f;

        public MuscleSequenceState()
        {

        }

        public void apply()
        {
            MuscleController.changeForce(RightDigastricDynamic, rightDigastricForce);
            MuscleController.changeForce(RightLateralPterygoidDynamic, rightLateralPterygoidForce);
            MuscleController.changeForce(RightMasseterDynamic, rightMasseterForce);
            MuscleController.changeForce(RightMedialPterygoidDynamic, rightMedialPterygoidForce);
            MuscleController.changeForce(RightTemporalisDynamic, rightTemporalisForce);

            MuscleController.changeForce(LeftDigastricDynamic, leftDigastricForce);
            MuscleController.changeForce(LeftLateralPterygoidDynamic, leftLateralPterygoidForce);
            MuscleController.changeForce(LeftMasseterDynamic, leftMasseterForce);
            MuscleController.changeForce(LeftMedialPterygoidDynamic, leftMedialPterygoidForce);
            MuscleController.changeForce(LeftTemporalisDynamic, leftTemporalisForce);

            TeethController.setTeethLoose(teethLoose);

            if (controlPointModification == ControlPointModification.Move)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                leftCP.moveToLocation(controlPointTarget, controlPointMoveSpeed);
                rightCP.moveToLocation(controlPointTarget, controlPointMoveSpeed);
            }
            else if (controlPointModification == ControlPointModification.Reset)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                leftCP.moveToLocation(leftCP.NeutralLocation, controlPointMoveSpeed);
                rightCP.moveToLocation(rightCP.NeutralLocation, controlPointMoveSpeed);
            }
            else if (controlPointModification == ControlPointModification.Stop)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                leftCP.stopMovement();
                rightCP.stopMovement();
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
        }

        #region Saveable Members

        protected MuscleSequenceState(LoadInfo info)
            :base(info)
        {

        }

        #endregion
    }
}
