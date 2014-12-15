using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using Engine.Platform;
using Engine.ObjectManagement;
using Medical.GUI;
using Medical;

namespace DentalSim.GUI
{
    class MandibleMovementDialog : PinableMDIDialog
    {
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private MuscleBehavior movingMuscle;
        private MovingMuscleTarget movingMuscleTarget;
        private bool allowSyncronization = true;
        private bool allowSceneManipulation = true;

        private MandibleControlSlider openTrackBar;
        private MandibleControlSlider rightForwardBack;
        private MandibleControlSlider leftForwardBack;
        private MandibleControlSlider bothForwardBack;
        private MandibleControlSlider forceSlider;
        private Button resetButton;
        private Button undoButton;
        private Button redoButton;

        private MusclePositionController musclePositionController;
        private MusclePosition slideStartMusclePosition = null;

        public MandibleMovementDialog(MovementSequenceController movementSequenceController, MusclePositionController musclePositionController)
            : base("DentalSim.GUI.MandibleMovement.MandibleMovementDialog.layout")
        {
            this.musclePositionController = musclePositionController;
            musclePositionController.OnUndoRedoChanged += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnRedo += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnUndo += musclePositionController_UndoRedoStateAltered;

            openTrackBar = new MandibleControlSlider(window.findWidget("Movement/HingeSlider") as ScrollBar);
            openTrackBar.Minimum = -3;
            openTrackBar.Maximum = 10;
            openTrackBar.ValueChangeStarted += mandibleMotionTrackBar_ValueChangeStarted;
            openTrackBar.ValueChangeEnded += mandibleMotionTrackBar_ValueChangeEnded;

            rightForwardBack = new MandibleControlSlider(window.findWidget("Movement/ExcursionRightSlider") as ScrollBar);
            rightForwardBack.Minimum = 0;
            rightForwardBack.Maximum = 1;
            rightForwardBack.ValueChangeStarted += mandibleMotionTrackBar_ValueChangeStarted;
            rightForwardBack.ValueChangeEnded += mandibleMotionTrackBar_ValueChangeEnded;
            
            leftForwardBack = new MandibleControlSlider(window.findWidget("Movement/ExcursionLeftSlider") as ScrollBar);
            leftForwardBack.Minimum = 0;
            leftForwardBack.Maximum = 1;
            leftForwardBack.ValueChangeStarted += mandibleMotionTrackBar_ValueChangeStarted;
            leftForwardBack.ValueChangeEnded += mandibleMotionTrackBar_ValueChangeEnded;
            
            bothForwardBack = new MandibleControlSlider(window.findWidget("Movement/ProtrusionSlider") as ScrollBar);
            bothForwardBack.Minimum = 0;
            bothForwardBack.Maximum = 1;
            bothForwardBack.ValueChangeStarted += mandibleMotionTrackBar_ValueChangeStarted;
            bothForwardBack.ValueChangeEnded += mandibleMotionTrackBar_ValueChangeEnded;
            
            forceSlider = new MandibleControlSlider(window.findWidget("Movement/ForceSlider") as ScrollBar);
            forceSlider.Minimum = 0;
            forceSlider.Maximum = 100;
            forceSlider.ValueChangeStarted += mandibleMotionTrackBar_ValueChangeStarted;
            forceSlider.ValueChangeEnded += mandibleMotionTrackBar_ValueChangeEnded;

            resetButton = window.findWidget("Movement/Reset") as Button;
            undoButton = window.findWidget("Undo") as Button;
            undoButton.MouseButtonClick += undoButton_MouseButtonClick;
            redoButton = window.findWidget("Redo") as Button;
            redoButton.MouseButtonClick += redoButton_MouseButtonClick;
            undoButton.Enabled = musclePositionController.HasUndo;
            redoButton.Enabled = musclePositionController.HasRedo;

            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
            rightForwardBack.ValueChanged += rightSliderValueChanged;
            leftForwardBack.ValueChanged += leftSliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            forceSlider.ValueChanged += forceSlider_ValueChanged;
            resetButton.MouseButtonClick += resetButton_Click;

            movementSequenceController.PlaybackStarted += new MovementSequenceEvent(movementSequenceController_PlaybackStarted);
            movementSequenceController.PlaybackStopped += new MovementSequenceEvent(movementSequenceController_PlaybackStopped);
        }

        void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            resetButton.Enabled = true;
            undoButton.Enabled = musclePositionController.HasUndo;
            redoButton.Enabled = musclePositionController.HasRedo;
        }

        void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            resetButton.Enabled = false;
            undoButton.Enabled = false;
            redoButton.Enabled = false;
        }

        public bool AllowSceneManipulation
        {
            get
            {
                return allowSceneManipulation;
            }
            set
            {
                allowSceneManipulation = value;
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            movingMuscleTarget = MuscleController.MovingTarget;
            Enabled = leftCP != null && rightCP != null && movingMuscle != null && movingMuscleTarget != null;
            if (Enabled)
            {
                //setup ui
                float leftNeutral = leftCP.NeutralLocation;
                synchronizeLeftCP(leftCP, leftNeutral);
                leftForwardBack.Minimum = leftCP.PosteriorShiftMaxLocation;
                leftForwardBack.SequentialChange = (leftForwardBack.Maximum - leftForwardBack.Minimum) / 10.0f;
                float rightNeutral = rightCP.NeutralLocation;
                synchronizeRightCP(rightCP, rightNeutral);
                rightForwardBack.Minimum = rightCP.PosteriorShiftMaxLocation;
                rightForwardBack.SequentialChange = (rightForwardBack.Maximum - rightForwardBack.Minimum) / 10.0f;
                bothForwardBack.Value = rightForwardBack.Value;
                bothForwardBack.Minimum = rightForwardBack.Minimum < leftForwardBack.Minimum ? rightForwardBack.Minimum : leftForwardBack.Minimum;
                bothForwardBack.SequentialChange = rightForwardBack.SequentialChange;
                synchronizeMovingMuscleOffset(movingMuscleTarget, movingMuscleTarget.Offset);
                synchronizeForce(movingMuscle, movingMuscle.getForce());

                //setup callbacks
                leftCP.PositionChanged += leftCP_PositionChanged;
                rightCP.PositionChanged += rightCP_PositionChanged;
                movingMuscleTarget.OffsetChanged += movingMuscleTarget_OffsetChanged;
                movingMuscle.ForceChanged += movingMuscle_ForceChanged;
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            if (movingMuscle != null)
            {
                movingMuscle.ForceChanged -= movingMuscle_ForceChanged;
                movingMuscle = null;
            }
            if (movingMuscleTarget != null)
            {
                movingMuscleTarget.OffsetChanged -= movingMuscleTarget_OffsetChanged;
                movingMuscleTarget = null;
            }
            if (leftCP != null)
            {
                leftCP.PositionChanged -= leftCP_PositionChanged;
                leftCP = null;
            }
            if (rightCP != null)
            {
                rightCP.PositionChanged -= rightCP_PositionChanged;
                rightCP = null;
            }
        }

        void bothForwardBackChanged(object sender, EventArgs e)
        {
            float value = bothForwardBack.Value;
            synchronizeLeftCP(bothForwardBack, value);
            synchronizeRightCP(bothForwardBack, value);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            musclePositionController.pushUndoState(new MusclePosition(true), musclePositionController.BindPosition);

            musclePositionController.timedBlend(musclePositionController.BindPosition, MedicalConfig.CameraTransitionTime);
        }

        //Synchronize methods
        //Moving muscle offset
        void synchronizeMovingMuscleOffset(object sender, Vector3 position)
        {
            if (allowSyncronization)
            {
                allowSyncronization = false;
                if (sender != movingMuscleTarget)
                {
                    movingMuscleTarget.Offset = position;
                }
                if (sender != openTrackBar)
                {
                    openTrackBar.Value = (int)(position.y * (openTrackBar.Maximum / -30.0f));
                }
                allowSyncronization = true;
            }
        }

        void movingMuscleTarget_OffsetChanged(MovingMuscleTarget source, Vector3 offset)
        {
            synchronizeMovingMuscleOffset(source, offset);
        }

        void openTrackBar_ValueChanged(object sender, EventArgs e)
        {
            synchronizeMovingMuscleOffset(openTrackBar, new Vector3(0.0f, openTrackBar.Value / (openTrackBar.Maximum / -30.0f), 0.0f));
        }

        //Left CP Position
        void synchronizeLeftCP(object sender, float position)
        {
            if (allowSyncronization)
            {
                allowSyncronization = false;
                if (sender != leftCP)
                {
                    leftCP.setLocation(position);
                }
                if (sender != leftForwardBack)
                {
                    leftForwardBack.Value = position;
                }
                allowSyncronization = true;
            }
        }

        void leftCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            synchronizeLeftCP(leftCP, position);
        }

        void leftSliderValueChanged(object sender, EventArgs e)
        {
            synchronizeLeftCP(leftForwardBack, leftForwardBack.Value);
        }

        //Right CP Position
        void synchronizeRightCP(object sender, float position)
        {
            if (allowSyncronization)
            {
                allowSyncronization = false;
                if (sender != rightCP)
                {
                    rightCP.setLocation(position);
                }
                if (sender != rightForwardBack)
                {
                    rightForwardBack.Value = position;
                }
                allowSyncronization = true;
            }
        }

        void rightCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            synchronizeRightCP(rightCP, position);
        }

        void rightSliderValueChanged(object sender, EventArgs e)
        {
            synchronizeRightCP(rightForwardBack, rightForwardBack.Value);
        }

        void synchronizeForce(object sender, float force)
        {
            if (allowSyncronization)
            {
                allowSyncronization = false;
                if (sender != forceSlider)
                {
                    forceSlider.Value = force;
                }
                if (sender != movingMuscle)
                {
                    movingMuscle.changeForce(force);
                }
                allowSyncronization = true;
            }
        }

        void forceSlider_ValueChanged(object sender, EventArgs e)
        {
            synchronizeForce(sender, forceSlider.Value);
        }

        void movingMuscle_ForceChanged(MuscleBehavior source, float force)
        {
            synchronizeForce(movingMuscle, movingMuscle.getForce());
        }

        void mandibleMotionTrackBar_ValueChangeEnded(MandibleControlSlider source)
        {
            if (slideStartMusclePosition != null)
            {
                musclePositionController.pushUndoState(slideStartMusclePosition);
                slideStartMusclePosition = null;
            }
        }

        void mandibleMotionTrackBar_ValueChangeStarted(MandibleControlSlider source)
        {
            slideStartMusclePosition = new MusclePosition(true);
        }

        void musclePositionController_UndoRedoStateAltered(MusclePositionController obj)
        {
            undoButton.Enabled = musclePositionController.HasUndo;
            redoButton.Enabled = musclePositionController.HasRedo;
        }

        void redoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            musclePositionController.redo();
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            musclePositionController.undo();
        }
    }
}
