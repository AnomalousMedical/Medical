using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;

namespace Medical.GUI
{
    public partial class SimpleMandibleControl : GUIElement
    {
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private MuscleBehavior movingMuscle;
        private MovingMuscleTarget movingMuscleTarget;
        private bool allowSyncronization = true;
        private bool allowSceneManipulation = true;
        private bool lowForce = true;

        public SimpleMandibleControl()
        {
            InitializeComponent();
            leftForwardBack.ValueChanged += leftSliderValueChanged;
            rightForwardBack.ValueChanged += rightSliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
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

        protected override void fixedLoopUpdate(Engine.Platform.Clock time)
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

        protected override void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            movingMuscleTarget = MuscleController.MovingTarget;
            Enabled = leftCP != null && rightCP != null && movingMuscle != null && movingMuscleTarget != null;
            if (Enabled)
            {
                //setup ui
                synchronizeLeftCP(leftCP, leftCP.getNeutralLocation());
                synchronizeRightCP(rightCP, rightCP.getNeutralLocation());
                bothForwardBack.Value = rightForwardBack.Value;
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

        protected override void sceneUnloading()
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
            float value = bothForwardBack.Value / (float)bothForwardBack.Maximum;
            synchronizeLeftCP(bothForwardBack, value);
            synchronizeRightCP(bothForwardBack, value);
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            synchronizeLeftCP(distortionButton, leftCP.getNeutralLocation());
            synchronizeRightCP(distortionButton, rightCP.getNeutralLocation());
            bothForwardBack.Value = rightForwardBack.Value;
            synchronizeMovingMuscleOffset(distortionButton, Vector3.Zero);
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
                    openTrackBar.Value = (int)(position.y * (-openTrackBar.Minimum / 30.0f));
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
            synchronizeMovingMuscleOffset(openTrackBar, new Vector3(0.0f, openTrackBar.Value / (-openTrackBar.Minimum / 30.0f), 0.0f));
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
                    leftForwardBack.Value = (int)(position * leftForwardBack.Maximum);
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
            synchronizeLeftCP(leftForwardBack, leftForwardBack.Value / (float)leftForwardBack.Maximum);
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
                    rightForwardBack.Value = (int)(position * rightForwardBack.Maximum);
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
            synchronizeRightCP(rightForwardBack, rightForwardBack.Value / (float)rightForwardBack.Maximum);
        }
    }
}
