using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Medical.Muscles;

namespace Medical
{
    public partial class MusclePosition : Saveable
    {
        private float leftCPPosition;
        private float rightCPPosition;
        private Vector3 movingTargetPosition;
        private float muscleForce;
        private FKChainState pelvisChainState;
        private EasingFunction easingFunction = EasingFunction.EaseInOutQuadratic;
        private FKChainState interpolatedPelvisChainState = new FKChainState(); //Pooled pelvis chain state used for interpolation, prevents garbage generation. Part of instance state, not saved.

        public MusclePosition()
        {
        }

        public void captureState()
        {
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            if (movingMuscle != null)
            {
                muscleForce = movingMuscle.getForce();
            }
            if (MuscleController.MovingTarget != null)
            {
                movingTargetPosition = MuscleController.MovingTarget.Offset;
            }
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            if (leftCP != null)
            {
                leftCPPosition = leftCP.CurrentLocation;
            }
            if (rightCP != null)
            {
                rightCPPosition = rightCP.CurrentLocation;
            }

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
            blend(this, 1.0f);
        }

        public void blend(MusclePosition targetState, float blendFactor)
        {
            float modifiedBlendFactor = blendFactor;
            if (blendFactor < 1.0f)
            {
                EasingFunctions.Ease(targetState.Easing, 0, 1, blendFactor, 1);
            }

            if (MuscleController.MovingTarget != null) //If this is null then the whole mandible simulation is invalid and its better to do nothing
            {
                MuscleController.changeForce("MovingMuscleDynamic", targetState.muscleForce);
                MuscleController.MovingTarget.Offset = targetState.movingTargetPosition;

                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                float delta = targetState.leftCPPosition - leftCPPosition;
                leftCP.setLocation(leftCPPosition + delta * modifiedBlendFactor);

                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                delta = targetState.rightCPPosition - rightCPPosition;
                rightCP.setLocation(rightCPPosition + delta * modifiedBlendFactor);
            }

            FKRoot pelvis;
            if (pelvisChainState != null && targetState.pelvisChainState != null && PoseableObjectsManager.tryGetFkChainRoot("Pelvis", out pelvis))
            {
                interpolatedPelvisChainState.interpolateFrom(pelvisChainState, targetState.pelvisChainState, modifiedBlendFactor);
                pelvis.applyChainState(interpolatedPelvisChainState);
            }
        }

        [Editable]
        public float LeftCPPosition
        {
            get
            {
                return leftCPPosition;
            }
            set
            {
                leftCPPosition = value;
            }
        }

        [Editable]
        public float RightCPPosition
        {
            get
            {
                return rightCPPosition;
            }
            set
            {
                rightCPPosition = value;
            }
        }

        [Editable]
        public Vector3 MovingTargetPosition
        {
            get
            {
                return movingTargetPosition;
            }
            set
            {
                movingTargetPosition = value;
            }
        }

        [Editable]
        public float MuscleForce
        {
            get
            {
                return muscleForce;
            }
            set
            {
                muscleForce = value;
            }
        }

        [Editable]
        public EasingFunction Easing
        {
            get
            {
                return easingFunction;
            }
            set
            {
                easingFunction = value;
            }
        }

        #region Saveable Members

        private const String LEFT_CP_POSITION = "leftCPPosition";
        private const String RIGHT_CP_POSITION = "rightCPPosition";
        private const String MOVING_TARGET_POSITION = "movingTargetPosition";
        private const String MUSCLE_FORCE = "muscleForce";
        private const String PELIVS_CHAIN_STATE = "pelvisChainState";
        private const String EASING_FUNCTION = "easingFunction";

        protected MusclePosition(LoadInfo info)
        {
            leftCPPosition = info.GetFloat(LEFT_CP_POSITION);
            rightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
            movingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
            muscleForce = info.GetFloat(MUSCLE_FORCE);
            pelvisChainState = info.GetValue<FKChainState>(PELIVS_CHAIN_STATE, null);
            easingFunction = info.GetValue(EASING_FUNCTION, EasingFunction.None); //We use no easing for older muscle positions because this is how they were originally created, the new default is to use InOutQuadratic, however.
            //Can keep this around, but get rid of the simulation version checks
            //Be sure to set the version to 1 in the getInfo when you commit to this scene.
            if (info.Version == 0 && SimulationVersionManager.LoadedVersion > SimulationVersionManager.OriginalVersion)
            {
                leftCPPosition = MovementSequenceState.UpgradeCpPosition(leftCPPosition);
                rightCPPosition = MovementSequenceState.UpgradeCpPosition(rightCPPosition);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
            info.AddValue(PELIVS_CHAIN_STATE, pelvisChainState);
            info.AddValue(EASING_FUNCTION, easingFunction);
            //When you remove the conversion you need to set the version by uncommenting the line below.
            //info.Version = 1;
        }

        #endregion
    }

    partial class MusclePosition
    {
        private EditInterface editInterface = null;

        public EditInterface getEditInterface(String name = "Muscle Position")
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, name, null);
                editInterface.addCommand(new EditInterfaceCommand("Capture", delegate(EditUICallback callback, EditInterfaceCommand caller)
                {
                    captureState();
                }));
                editInterface.addCommand(new EditInterfaceCommand("Preview", delegate(EditUICallback callback, EditInterfaceCommand caller)
                {
                    preview();
                }));
            }
            return editInterface;
        }
    }
}
