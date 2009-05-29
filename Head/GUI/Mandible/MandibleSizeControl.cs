using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Logging;

namespace Medical.GUI
{
    public partial class MandibleSizeControl : DockContent
    {
        public MandibleSizeControl()
        {
            InitializeComponent();
        }            

        public void sceneChanged()
        {
            foreach (BoneManipulatorSlider slider in sliderPanel.Controls)
            {
                if (slider.Tag != null)
                {
                    BoneManipulator manipulator = BoneManipulatorController.getManipulator(slider.Tag.ToString());
                    if (manipulator != null)
                    {
                        slider.initialize(manipulator);
                    }
                    else
                    {
                        Log.Default.sendMessage("Could not find manipulator named {0}.", LogLevel.Warning, "Head", slider.Tag.ToString());
                    }
                }
                else
                {
                    Log.Default.sendMessage("No tag set on slider. Cannot search for manipulator.", LogLevel.Warning, "Head");
                }
            }
        }

        internal void sceneUnloading()
        {
            foreach (BoneManipulatorSlider slider in sliderPanel.Controls)
            {
                slider.clearManipulator();
            }
        }

        private void rightNormal_Click(object sender, EventArgs e)
        {
            
        }

        private void leftNormal_Click(object sender, EventArgs e)
        {

        }
    }
}
