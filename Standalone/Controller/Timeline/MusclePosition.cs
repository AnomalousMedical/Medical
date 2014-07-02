using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;

namespace Medical
{
    public partial class MusclePosition : Saveable
    {
        private float leftCPPosition;
        private float rightCPPosition;
        private Vector3 movingTargetPosition;
        private float muscleForce;
        private FKChainState pelvisChainState;
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
            FKLink pelvisLink;
            if (PoseableObjectsManager.tryGetFkChainRoot("Pelvis", out pelvisLink))
            {
                pelvisChainState = new FKChainState();
                pelvisLink.addToChainState(pelvisChainState);
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
            if (MuscleController.MovingTarget != null) //If this is null then the whole mandible simulation is invalid and its better to do nothing
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

            FKLink pelvisLink;
            if (pelvisChainState != null && targetState.pelvisChainState != null && PoseableObjectsManager.tryGetFkChainRoot("Pelvis", out pelvisLink))
            {
                interpolatedPelvisChainState.interpolateFrom(pelvisChainState, targetState.pelvisChainState, blendFactor);
                pelvisLink.applyChainState(interpolatedPelvisChainState);
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

        #region Saveable Members

        private const String LEFT_CP_POSITION = "leftCPPosition";
        private const String RIGHT_CP_POSITION = "rightCPPosition";
        private const String MOVING_TARGET_POSITION = "movingTargetPosition";
        private const String MUSCLE_FORCE = "muscleForce";
        private const String PELIVS_CHAIN_STATE = "pelvisChainState";

        protected MusclePosition(LoadInfo info)
        {
            leftCPPosition = info.GetFloat(LEFT_CP_POSITION);
            rightCPPosition = info.GetFloat(RIGHT_CP_POSITION);
            movingTargetPosition = info.GetVector3(MOVING_TARGET_POSITION);
            muscleForce = info.GetFloat(MUSCLE_FORCE);
            pelvisChainState = info.GetValue<FKChainState>(PELIVS_CHAIN_STATE, null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(LEFT_CP_POSITION, leftCPPosition);
            info.AddValue(RIGHT_CP_POSITION, rightCPPosition);
            info.AddValue(MOVING_TARGET_POSITION, movingTargetPosition);
            info.AddValue(MUSCLE_FORCE, muscleForce);
            info.AddValue(PELIVS_CHAIN_STATE, pelvisChainState);
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
