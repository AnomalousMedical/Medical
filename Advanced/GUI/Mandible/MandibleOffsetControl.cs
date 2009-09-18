using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
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
        private bool allowSceneUpdates = true;
        private bool allowUIUpdates = true;

        public MandibleOffsetControl()
        {
            InitializeComponent();
            leftForwardBack.ValueChanged += sliderValueChanged;
            rightForwardBack.ValueChanged += sliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            //leftOffset.ValueChanged += offset_ValueChanged;
            //rightOffset.ValueChanged += offset_ValueChanged;
            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
            forceUpDown.ValueChanged += new EventHandler(forceUpDown_ValueChanged);
        }

        //void offset_ValueChanged(object sender, EventArgs e)
        //{
        //    if (allowSceneUpdates)
        //    {
        //        allowSceneUpdates = false;
        //        leftCP.setLocation((float)leftOffset.Value);
        //        rightCP.setLocation((float)rightOffset.Value);
        //        rightForwardBack.Value = (int)(rightCP.CurrentLocation * rightForwardBack.Maximum);
        //        leftForwardBack.Value = (int)(leftCP.CurrentLocation * leftForwardBack.Maximum);
        //        allowSceneUpdates = true;
        //    }
        //}

        protected override void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            movingMuscleTarget = MuscleController.MovingTarget;
            allowSceneUpdates = false;
            Enabled = leftCP != null && rightCP != null && movingMuscle != null && movingMuscleTarget != null;
            if (Enabled)
            {
                //setup ui
                leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
                rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
                bothForwardBack.Value = rightForwardBack.Value;
                leftOffset.Value = (decimal)leftCP.getNeutralLocation();
                rightOffset.Value = (decimal)rightCP.getNeutralLocation();
                forceUpDown.Value = (decimal)movingMuscle.getForce();

                //setup callbacks
                movingMuscle.ForceChanged += movingMuscle_ForceChanged;
                leftCP.PositionChanged += leftCP_PositionChanged;
                rightCP.PositionChanged += rightCP_PositionChanged;
                movingMuscleTarget.OffsetChanged += movingMuscleTarget_OffsetChanged;
            }
            bothForwardBack.Value = rightForwardBack.Value;
            allowSceneUpdates = true;
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

        void sliderValueChanged(object sender, EventArgs e)
        {
            if (allowSceneUpdates)
            {
                allowUIUpdates = false;
                leftCP.setLocation(leftForwardBack.Value / (float)leftForwardBack.Maximum);
                rightCP.setLocation(rightForwardBack.Value / (float)rightForwardBack.Maximum);
                allowUIUpdates = true;
                allowSceneUpdates = false;
                leftOffset.Value = (decimal)leftCP.CurrentLocation;
                rightOffset.Value = (decimal)rightCP.CurrentLocation;
                allowSceneUpdates = true;
            }
        }

        void bothForwardBackChanged(object sender, EventArgs e)
        {
            if (allowSceneUpdates)
            {
                allowSceneUpdates = false;
                leftForwardBack.Value = bothForwardBack.Value;
                rightForwardBack.Value = bothForwardBack.Value;
                allowSceneUpdates = true;
                sliderValueChanged(null, null);
            }
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            allowSceneUpdates = false;
            rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
            leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
            bothForwardBack.Value = rightForwardBack.Value;
            leftOffset.Value = (decimal)leftCP.getNeutralLocation();
            rightOffset.Value = (decimal)rightCP.getNeutralLocation();
            allowSceneUpdates = true;
            sliderValueChanged(sender, e);
        }

        void forceUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (allowSceneUpdates)
            {
                allowUIUpdates = false;
                MuscleController.changeForce("MovingMuscleDynamic", (float)forceUpDown.Value);
                allowUIUpdates = true;
            }
        }

        void openTrackBar_ValueChanged(object sender, EventArgs e)
        {
            allowUIUpdates = false;
            movingMuscleTarget.Offset = new Vector3(0.0f, -openTrackBar.Value / (openTrackBar.Maximum / 30.0f), 0.0f);
            allowUIUpdates = true;
        }

        void movingMuscle_ForceChanged(MuscleBehavior source, float force)
        {
            if (allowUIUpdates)
            {
                forceUpDown.Value = (decimal)force;
            }
        }

        void rightCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            if (allowUIUpdates)
            {
                rightForwardBack.Value = (int)(position * rightForwardBack.Maximum);
            }
        }

        void leftCP_PositionChanged(ControlPointBehavior behavior, float position)
        {
            if (allowUIUpdates)
            {
                leftForwardBack.Value = (int)(position * leftForwardBack.Maximum);
            }
        }

        void movingMuscleTarget_OffsetChanged(MovingMuscleTarget source, Vector3 offset)
        {
            if (allowUIUpdates)
            {
                openTrackBar.Value = (int)(-offset.y * (openTrackBar.Maximum / 30.0f));
            }
        }
    }
}
