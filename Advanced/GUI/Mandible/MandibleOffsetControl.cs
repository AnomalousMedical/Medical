﻿using System;
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
            allowUpdates = false;
            Enabled = leftCP != null && rightCP != null;
            if (Enabled)
            {
                leftForwardBack.Value = (int)(leftCP.getNeutralLocation() * leftForwardBack.Maximum);
                rightForwardBack.Value = (int)(rightCP.getNeutralLocation() * rightForwardBack.Maximum);
                bothForwardBack.Value = rightForwardBack.Value;
                leftOffset.Value = (decimal)leftCP.getNeutralLocation();
                rightOffset.Value = (decimal)rightCP.getNeutralLocation();
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
    }
}
