using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class BoneManipulatorSlider : UserControl
    {
        private BoneManipulator manipulator;
        private int blendDelta = 0;
        private int blendStart;
        private bool allowUpdate = true;

        public BoneManipulatorSlider()
        {
            InitializeComponent();
            valueTrackBar.ValueChanged += new EventHandler(valueTrackBar_ValueChanged);
        }

        public void initialize(BoneManipulator manipulator)
        {
            this.manipulator = manipulator;
            allowUpdate = false;
            valueTrackBar.Value = (int)(manipulator.Position * valueTrackBar.Maximum);
            allowUpdate = true;
        }

        public void clearManipulator()
        {
            this.manipulator = null;
        }

        void valueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdate)
            {
                manipulator.Position = (float)valueTrackBar.Value / valueTrackBar.Maximum;
            }
        }

        public void startBlend(float blendTarget)
        {
            blendStart = valueTrackBar.Value;
            blendDelta = (int)(blendTarget * valueTrackBar.Maximum) - valueTrackBar.Value;
        }

        public void blend(float amount)
        {
            valueTrackBar.Value = blendStart + (int)(blendDelta * amount);
        }

        public void setToDefault()
        {
            valueTrackBar.Value = (int)(manipulator.DefaultPosition * valueTrackBar.Maximum);
        }

        public String LabelText
        {
            get
            {
                return sliderNameLabel.Text;
            }
            set
            {
                sliderNameLabel.Text = value;
            }
        }

        public float Value
        {
            get
            {
                return valueTrackBar.Value / valueTrackBar.Maximum;
            }
            set
            {
                valueTrackBar.Value = (int)(value * valueTrackBar.Maximum);
            }
        }
    }
}
