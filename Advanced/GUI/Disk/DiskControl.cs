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
            leftForwardBack.ValueChanged += leftSideValueChanged;
            leftUpDown.ValueChanged += leftSideValueChanged;
            centerTrackBar.ValueChanged += leftSideValueChanged;

            rightForwardBack.ValueChanged += rightSideValueChanged;
            rightUpDown.ValueChanged += rightSideValueChanged;
            centerTrackBar.ValueChanged += rightSideValueChanged;
        }

        void leftSideValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                leftDisc.setOffset(new Vector3(centerTrackBar.Value / (float)centerTrackBar.Maximum,
                                             leftUpDown.Value / (float)-leftUpDown.Minimum,
                                             leftForwardBack.Value / (float)leftForwardBack.Maximum));
            }
        }

        void rightSideValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                rightDisc.setOffset(new Vector3(centerTrackBar.Value / (float)centerTrackBar.Maximum,
                                             rightUpDown.Value / (float)-rightUpDown.Minimum,
                                             rightForwardBack.Value / (float)rightForwardBack.Maximum));
            }
        }

        public override void  sceneLoaded()
        {
            leftDisc = DiscController.getDisc("LeftTMJDisc");
            rightDisc = DiscController.getDisc("RightTMJDisc");
            Enabled = leftDisc != null && rightDisc != null;
            if (Enabled)
            {
                setSliders(leftDisc.getOffset(0.0f), rightDisc.getOffset(0.0f));
            }
        }

        public override void sceneUnloading()
        {
            leftDisc = null;
            rightDisc = null;
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            setSliders(leftDisc.getNormalOffset(), rightDisc.getNormalOffset());
            leftSideValueChanged(null, null);
            rightSideValueChanged(null, null);
        }

        private void setSliders(Vector3 leftOffset, Vector3 rightOffset)
        {
            allowUpdates = false;
            leftForwardBack.Value = (int)(leftOffset.z * leftForwardBack.Maximum);
            leftUpDown.Value = (int)(leftOffset.y * -leftUpDown.Minimum);

            rightForwardBack.Value = (int)(rightOffset.z * rightForwardBack.Maximum);
            rightUpDown.Value = (int)(rightOffset.y * -rightUpDown.Minimum);
            centerTrackBar.Value = (int)(rightOffset.x * centerTrackBar.Maximum);
            allowUpdates = true;
        }
    }
}
