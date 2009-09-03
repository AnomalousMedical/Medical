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
        private bool allowUpdates = true;

        public MandibleOffsetControl()
        {
            InitializeComponent();
            leftForwardBack.ValueChanged += sliderValueChanged;
            rightForwardBack.ValueChanged += sliderValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
            leftOffset.ValueChanged += offset_ValueChanged;
            rightOffset.ValueChanged += offset_ValueChanged;
            openTrackBar.ValueChanged += openTrackBar_ValueChanged;
            forceUpDown.ValueChanged += new EventHandler(forceUpDown_ValueChanged);
        }

        void openTrackBar_ValueChanged(object sender, EventArgs e)
        {
            MuscleController.MovingTarget.Offset = new Vector3(0.0f, -openTrackBar.Value / (openTrackBar.Maximum / 30.0f), 0.0f);
        }

        void offset_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                allowUpdates = false;
                leftCP.setLocation((float)leftOffset.Value);
                rightCP.setLocation((float)rightOffset.Value);
                rightForwardBack.Value = (int)(rightCP.CurrentLocation * rightForwardBack.Maximum);
                leftForwardBack.Value = (int)(leftCP.CurrentLocation * leftForwardBack.Maximum);
                allowUpdates = true;
            }
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            allowUpdates = false;
            Enabled = leftCP != null && rightCP != null && movingMuscle != null;
            if (Enabled)
            {
                leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
                rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
                bothForwardBack.Value = rightForwardBack.Value;
                leftOffset.Value = (decimal)leftCP.getNeutralLocation();
                rightOffset.Value = (decimal)rightCP.getNeutralLocation();
                forceUpDown.Value = (decimal)movingMuscle.getForce();
            }
            bothForwardBack.Value = rightForwardBack.Value;
            allowUpdates = true;
        }

        protected override void sceneUnloading()
        {
            leftCP = null;
            rightCP = null;
        }

        void sliderValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                allowUpdates = false;
                leftCP.setLocation(leftForwardBack.Value / (float)leftForwardBack.Maximum);
                rightCP.setLocation(rightForwardBack.Value / (float)rightForwardBack.Maximum);
                leftOffset.Value = (decimal)leftCP.CurrentLocation;
                rightOffset.Value = (decimal)rightCP.CurrentLocation;
                allowUpdates = true;
            }
        }

        void bothForwardBackChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                allowUpdates = false;
                leftForwardBack.Value = bothForwardBack.Value;
                rightForwardBack.Value = bothForwardBack.Value;
                allowUpdates = true;
                sliderValueChanged(null, null);
            }
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            allowUpdates = false;
            rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
            leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
            bothForwardBack.Value = rightForwardBack.Value;
            leftOffset.Value = (decimal)leftCP.getNeutralLocation();
            rightOffset.Value = (decimal)rightCP.getNeutralLocation();
            allowUpdates = true;
            sliderValueChanged(sender, e);
        }

        void forceUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                MuscleController.changeForce("MovingMuscleDynamic", (float)forceUpDown.Value);
            }
        }
    }
}
