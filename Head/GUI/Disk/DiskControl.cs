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
    public partial class DiskControl : DockContent
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

        public void sceneChanged()
        {
            leftDisc = DiscController.getDisc("LeftTMJDisc");
            rightDisc = DiscController.getDisc("RightTMJDisc");
            allowUpdates = false;
            Enabled = leftDisc != null && rightDisc != null;
            if (Enabled)
            {
                Vector3 offset = leftDisc.getOffset(0.0f);
                leftForwardBack.Value = (int)(offset.z * leftForwardBack.Maximum);
                leftUpDown.Value = (int)(offset.y * -leftUpDown.Minimum);

                offset = rightDisc.getOffset(0.0f);
                rightForwardBack.Value = (int)(offset.z * rightForwardBack.Maximum);
                rightUpDown.Value = (int)(offset.y * -rightUpDown.Minimum);
                centerTrackBar.Value = (int)(offset.x * centerTrackBar.Maximum);
            }
            allowUpdates = true;
        }

        public void sceneUnloading()
        {
            leftDisc = null;
            rightDisc = null;
        }
    }
}
