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
    public partial class MandibleOffsetControl : DockContent
    {
        private ControlPointBehavior leftCP;
        private ControlPointBehavior rightCP;
        private bool allowUpdates = true;

        public MandibleOffsetControl()
        {
            InitializeComponent();
            leftForwardBack.ValueChanged += offsetValueChanged;
            leftUpDown.ValueChanged += offsetValueChanged;
            centerTrackBar.ValueChanged += offsetValueChanged;
            rightForwardBack.ValueChanged += offsetValueChanged;
            rightUpDown.ValueChanged += offsetValueChanged;
            bothForwardBack.ValueChanged += bothForwardBackChanged;
        }

        public void sceneChanged()
        {
            leftCP = ControlPointController.getControlPoint("LeftCP");
            rightCP = ControlPointController.getControlPoint("RightCP");
            allowUpdates = false;
            if (leftCP != null)
            {
                leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
            }
            if (rightCP != null)
            {
                rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
            }
            bothForwardBack.Value = rightForwardBack.Value;
            allowUpdates = true;
        }

        public void sceneUnloading()
        {
            leftCP = null;
            rightCP = null;
        }

        void offsetValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                if (leftCP != null)
                {
                    //Vector3 offset = new Vector3(centerTrackBar.Value / 5000.0f, leftUpDown.Value / 5000.0f, leftForwardBack.Value / -5000.0f);
                    leftCP.setLocation(leftForwardBack.Value / (float)leftForwardBack.Maximum);
                }
                if (rightCP != null)
                {
                   // Vector3 offset = new Vector3(centerTrackBar.Value / 5000.0f, rightUpDown.Value / 5000.0f, rightForwardBack.Value / -5000.0f);
                    rightCP.setLocation(rightForwardBack.Value / (float)rightForwardBack.Maximum);
                }
            }
        }

        void bothForwardBackChanged(object sender, EventArgs e)
        {
            allowUpdates = false;
            leftForwardBack.Value = bothForwardBack.Value;
            allowUpdates = true;
            rightForwardBack.Value = bothForwardBack.Value;
        }

        private void distortionButton_Click(object sender, EventArgs e)
        {
            allowUpdates = false;
            centerTrackBar.Value = 0;
            rightForwardBack.Value = 0;
            rightUpDown.Value = 0;
            leftForwardBack.Value = 0;
            leftUpDown.Value = 0;
            allowUpdates = true;
            offsetValueChanged(sender, e);
        }   
    }
}
