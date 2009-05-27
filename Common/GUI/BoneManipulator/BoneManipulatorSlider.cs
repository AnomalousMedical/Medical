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

        public BoneManipulatorSlider()
        {
            InitializeComponent();
            valueTrackBar.ValueChanged += new EventHandler(valueTrackBar_ValueChanged);
        }

        public void initialize(BoneManipulator manipulator)
        {
            this.manipulator = manipulator;
            this.sliderNameLabel.Text = manipulator.getUIName();
        }

        void valueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            manipulator.setPosition(valueTrackBar.Value / 100.0f);
        }
    }
}
