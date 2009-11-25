using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Logging;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class MandibleOffsetControl : GUIElement
    {
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private MuscleBehavior movingMuscle;
        private MovingMuscleTarget movingMuscleTarget;
        private bool allowSyncronization = true;

        public MandibleOffsetControl()
        {
            InitializeComponent();
            leftForwardBack.ValueChanged += leftSliderValueChanged;
            rightForwardBack.ValueChanged += rightSliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            leftOffset.ValueChanged += leftOffset_ValueChanged;
            rightOffset.ValueChanged += rightOffset_ValueChanged;
            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
            forceUpDown.ValueChanged += forceUpDown_ValueChanged;
            openUpDown.ValueChanged += openUpDown_ValueChanged;
            forceSlider.ValueChanged += forceSlider_ValueChanged;
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
                synchronizeForce(movingMuscle, movingMuscle.getForce());
                synchronizeMovingMuscleOffset(movingMuscleTarget, movingMuscleTarget.Offset);

                //setup callbacks
                movingMuscle.ForceChanged += movingMuscle_ForceChanged;
                leftCP.PositionChanged += leftCP_PositionChanged;
                rightCP.PositionChanged += rightCP_PositionChanged;
                movingMuscleTarget.OffsetChanged += movingMuscleTarget_OffsetChanged;
            }
        }

        protected override void sceneUnloading()
        {
            if (movingMuscle != null)
            {
                movingMuscle.ForceChanged -= movingMuscle_ForceChanged;
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
        //Moving muscle force
        void synchronizeForce(object sender, float force)
        {
            if (allowSyncronization)
            {
                allowSyncronization = false;
                if (sender != movingMuscle)
                {
                    movingMuscle.changeForce(force);
                }
                if (sender != forceUpDown)
                {
                    forceUpDown.Value = (decimal)force;
                }
                if (sender != forceSlider)
                {
                    forceSlider.Value = (int)force;
                }
                allowSyncronization = true;
            }
        }

        void movingMuscle_ForceChanged(MuscleBehavior source, float force)
        {
            synchronizeForce(source, force);
        }

        void forceUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeForce(forceUpDown, (float)forceUpDown.Value);
        }

        void forceSlider_ValueChanged(object sender, EventArgs e)
        {
            synchronizeForce(forceSlider, forceSlider.Value);
        }

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
                if (sender != openUpDown)
                {
                    openUpDown.Value = (decimal)position.y;
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

        void openUpDown_ValueChanged(object sender, EventArgs e)
        {
            synchronizeMovingMuscleOffset(openUpDown, new Vector3(0.0f, (float)openUpDown.Value, 0.0f));
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
                if (sender != leftOffset)
                {
                    leftOffset.Value = (decimal)position;
                }
                allowSyncronization = true;
            }
        }

        void leftCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            synchronizeLeftCP(leftCP, position);
        }

        void leftOffset_ValueChanged(object sender, EventArgs e)
        {
            synchronizeLeftCP(leftOffset, (float)leftOffset.Value);
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
                if (sender != leftOffset)
                {
                    rightOffset.Value = (decimal)position;
                }
                allowSyncronization = true;
            }
        }

        void rightCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            synchronizeRightCP(rightCP, position);
        }

        void rightOffset_ValueChanged(object sender, EventArgs e)
        {
            synchronizeRightCP(rightOffset, (float)rightOffset.Value);
        }

        void rightSliderValueChanged(object sender, EventArgs e)
        {
            synchronizeRightCP(rightForwardBack, rightForwardBack.Value / (float)rightForwardBack.Maximum);
        }
    }
}
