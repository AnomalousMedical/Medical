using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;
using MyGUIPlugin;
using Standalone;
using Logging;

namespace Medical.GUI
{
    class MandibleGUIController
    {
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private MuscleBehavior movingMuscle;
        private MovingMuscleTarget movingMuscleTarget;
        private bool allowSyncronization = true;
        private bool allowSceneManipulation = true;
        private bool lowForce = true;
        private MedicalController medicalController;

        private MandibleControlSlider openTrackBar;
        private MandibleControlSlider rightForwardBack;
        private MandibleControlSlider leftForwardBack;
        private MandibleControlSlider bothForwardBack;
        private Button resetButton;
        private Button restoreButton;

        private Vector3 movingMusclePosition;
        private float leftCPPosition;
        private float rightCPPosition;

        public MandibleGUIController(Gui gui, MedicalController medicalController)
        {
            this.medicalController = medicalController;

            openTrackBar = new MandibleControlSlider(gui.findWidgetT("Movement/HingeSlider") as HScroll);
            openTrackBar.Minimum = -3;
            openTrackBar.Maximum = 10;
            rightForwardBack = new MandibleControlSlider(gui.findWidgetT("Movement/ExcursionRightSlider") as HScroll);
            rightForwardBack.Minimum = 0;
            rightForwardBack.Maximum = 1;
            leftForwardBack = new MandibleControlSlider(gui.findWidgetT("Movement/ExcursionLeftSlider") as HScroll);
            leftForwardBack.Minimum = 0;
            leftForwardBack.Maximum = 1;
            bothForwardBack = new MandibleControlSlider(gui.findWidgetT("Movement/ProtrusionSlider") as HScroll);
            bothForwardBack.Minimum = 0;
            bothForwardBack.Maximum = 1;
            resetButton = gui.findWidgetT("Movement/Reset") as Button;
            restoreButton = gui.findWidgetT("Movement/Restore") as Button;
            restoreButton.Enabled = false;

            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
            rightForwardBack.ValueChanged += rightSliderValueChanged;
            leftForwardBack.ValueChanged += leftSliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            resetButton.MouseButtonClick += resetButton_Click;
            restoreButton.MouseButtonClick += restoreButton_Click;
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
                if (allowSceneManipulation)
                {
                    this.subscribeToUpdates();
                }
                else
                {
                    this.unsubscribeFromUpdates();
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return openTrackBar.Enabled;
            }
            set
            {
                openTrackBar.Enabled = value;
                rightForwardBack.Enabled = value;
                leftForwardBack.Enabled = value;
                bothForwardBack.Enabled = value;
                resetButton.Enabled = value;
            }
        }

        protected void fixedLoopUpdate(Clock time)
        {
            //If teeth are touching and we are moving upward
            if (TeethController.anyTeethTouching() && movingMuscleTarget.Owner.Translation.y > movingMuscle.Owner.Translation.y - 5.5f)
            {
                if (!lowForce)
                {
                    movingMuscle.changeForce(6.0f);
                    lowForce = true;
                }
            }
            else
            {
                if (lowForce)
                {
                    movingMuscle.changeForce(100.0f);
                    lowForce = false;
                }
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            //restoreButton.Enabled = false;
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            movingMuscleTarget = MuscleController.MovingTarget;
            Enabled = leftCP != null && rightCP != null && movingMuscle != null && movingMuscleTarget != null;
            if (Enabled)
            {
                //setup ui
                float leftNeutral = leftCP.getNeutralLocation();
                synchronizeLeftCP(leftCP, leftNeutral);
                leftForwardBack.Minimum = leftNeutral;
                leftForwardBack.SequentialChange = (leftForwardBack.Maximum - leftForwardBack.Minimum) / 10.0f;
                float rightNeutral = rightCP.getNeutralLocation();
                synchronizeRightCP(rightCP, rightNeutral);
                rightForwardBack.Minimum = rightNeutral;
                rightForwardBack.SequentialChange = (rightForwardBack.Maximum - rightForwardBack.Minimum) / 10.0f;
                bothForwardBack.Value = rightForwardBack.Value;
                bothForwardBack.Minimum = rightForwardBack.Minimum < leftForwardBack.Minimum ? rightForwardBack.Minimum : leftForwardBack.Minimum;
                bothForwardBack.SequentialChange = rightForwardBack.SequentialChange;
                synchronizeMovingMuscleOffset(movingMuscleTarget, movingMuscleTarget.Offset);

                //setup callbacks
                leftCP.PositionChanged += leftCP_PositionChanged;
                rightCP.PositionChanged += rightCP_PositionChanged;
                movingMuscleTarget.OffsetChanged += movingMuscleTarget_OffsetChanged;
            }

            if (allowSceneManipulation)
            {
                this.subscribeToUpdates();
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            if (movingMuscle != null)
            {
                movingMuscle = null;
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

            if (allowSceneManipulation)
            {
                this.unsubscribeFromUpdates();
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
            leftCPPosition = leftCP.CurrentLocation;
            rightCPPosition = rightCP.CurrentLocation;
            movingMusclePosition = movingMuscleTarget.Offset;

            synchronizeLeftCP(resetButton, leftCP.getNeutralLocation());
            synchronizeRightCP(resetButton, rightCP.getNeutralLocation());
            bothForwardBack.Value = rightForwardBack.Value;
            synchronizeMovingMuscleOffset(resetButton, Vector3.Zero);
            restoreButton.Enabled = true;
        }

        void restoreButton_Click(object sender, EventArgs e)
        {
            synchronizeLeftCP(resetButton, leftCPPosition);
            synchronizeRightCP(resetButton, rightCPPosition);
            synchronizeMovingMuscleOffset(resetButton, movingMusclePosition);
            restoreButton.Enabled = false;
        }

        private void subscribeToUpdates()
        {
            medicalController.FixedLoopUpdate += fixedLoopUpdate;
        }

        private void unsubscribeFromUpdates()
        {
            medicalController.FixedLoopUpdate -= fixedLoopUpdate;
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
    }
}
