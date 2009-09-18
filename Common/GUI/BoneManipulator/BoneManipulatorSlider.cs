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
        private bool allowSynchronization = true;

        public BoneManipulatorSlider()
        {
            InitializeComponent();
            valueTrackBar.ValueChanged += new EventHandler(valueTrackBar_ValueChanged);
        }

        public void initialize(BoneManipulator manipulator)
        {
            this.manipulator = manipulator;
            synchronizeValue(manipulator, manipulator.Position);
        }

        public void clearManipulator()
        {
            if (manipulator != null)
            {
                this.manipulator = null;
            }
        }

        public void setToDefault()
        {
            synchronizeValue(this, manipulator.DefaultPosition);
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
                synchronizeValue(this, value);
            }
        }

        void valueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            synchronizeValue(valueTrackBar, (float)valueTrackBar.Value / valueTrackBar.Maximum);
        }

        private void synchronizeValue(object sender, float value)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (manipulator != null && sender != manipulator)
                {
                    manipulator.Position = value;
                }
                if (sender != valueTrackBar)
                {
                    valueTrackBar.Value = (int)(value * valueTrackBar.Maximum);
                }
                allowSynchronization = true;
            }
        }
    }
}
