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
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class DiskControl : GUIElement
    {
        private Disc leftDisc;
        private Disc rightDisc;

        private bool allowUpdates = true;

        public DiskControl()
        {
            InitializeComponent();
            leftDiscPositionSlider.ValueChanged += leftSideValueChanged;
            leftDiscOffset.ValueChanged += leftSideValueChanged;
            leftRDAOffset.ValueChanged += leftSideValueChanged;
            centerTrackBar.ValueChanged += leftSideValueChanged;

            rightDiscPositionSlider.ValueChanged += rightSideValueChanged;
            rightDiscOffset.ValueChanged += rightSideValueChanged;
            rightRDAOffset.ValueChanged += rightSideValueChanged;
            centerTrackBar.ValueChanged += rightSideValueChanged;
        }

        void leftSideValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                leftDisc.RDAOffset = new Vector3(0.0f, leftRDAOffset.Value / -(float)leftRDAOffset.Maximum, 0.0f);
                leftDisc.DiscOffset = new Vector3(0.0f, leftDiscOffset.Value / -(float)leftDiscOffset.Maximum, 0.0f);
                leftDisc.HorizontalOffset = new Vector3(centerTrackBar.Value / (float)centerTrackBar.Maximum, 0.0f, 0.0f);
                leftDisc.PopLocation = leftDiscPositionSlider.Value / (float)leftDiscPositionSlider.Maximum;
            }
        }

        void rightSideValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                rightDisc.RDAOffset = new Vector3(0.0f, rightRDAOffset.Value / -(float)rightRDAOffset.Maximum, 0.0f);
                rightDisc.DiscOffset = new Vector3(0.0f, rightDiscOffset.Value / -(float)rightDiscOffset.Maximum, 0.0f);
                rightDisc.HorizontalOffset = new Vector3(centerTrackBar.Value / (float)centerTrackBar.Maximum, 0.0f, 0.0f);
                rightDisc.PopLocation = rightDiscPositionSlider.Value / (float)rightDiscPositionSlider.Maximum;
            }
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftDisc = DiscController.getDisc("LeftTMJDisc");
            rightDisc = DiscController.getDisc("RightTMJDisc");
            Enabled = leftDisc != null && rightDisc != null;
            if (Enabled)
            {
                setSliders(leftDisc.DiscOffset.y, leftDisc.RDAOffset.y, rightDisc.DiscOffset.y, rightDisc.RDAOffset.y);
            }
        }

        protected override void sceneUnloading()
        {
            leftDisc = null;
            rightDisc = null;
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            setSliders(leftDisc.NormalDiscOffset.y, leftDisc.NormalRDAOffset.y, rightDisc.NormalDiscOffset.y, rightDisc.NormalRDAOffset.y);
            leftSideValueChanged(null, null);
            rightSideValueChanged(null, null);
        }

        private void setSliders(float leftDisc, float leftRDA, float rightDisc, float rightRDA)
        {
            allowUpdates = false;
            leftDiscOffset.Value = (int)(leftDisc * -leftDiscOffset.Maximum);
            leftRDAOffset.Value = (int)(leftRDA * -leftRDAOffset.Maximum);

            rightDiscOffset.Value = (int)(rightDisc * -rightDiscOffset.Maximum);
            rightRDAOffset.Value = (int)(rightRDA * -rightRDAOffset.Maximum);

            centerTrackBar.Value = 0;

            rightDiscLocked.Checked = this.rightDisc.Locked;
            leftDiscLocked.Checked = this.leftDisc.Locked;
            allowUpdates = true;
        }

        private void rightDiscLocked_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                rightDisc.Locked = rightDiscLocked.Checked;
            }
        }

        private void leftDiscLocked_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                leftDisc.Locked = leftDiscLocked.Checked;
            }
        }
    }
}
