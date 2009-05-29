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
            rightAntegonialNotchSlider.Value = 0;
            rightCondyleDegenerationSlider.Value = 0;
            rightCondyleHeightSlider.Value = 0;
            rightCondyleRotationSlider.Value = 0;
            rightMandibularNotchSlider.Value = 0;
            rightRamusHeightSlider.Value = 0;
        }

        private void leftNormal_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.Value = 0;
            leftCondyleDegenerationSlider.Value = 0;
            leftCondyleHeightSlider.Value = 0;
            leftCondyleRotationSlider.Value = 0;
            leftMandibularNotchSlider.Value = 0;
            leftRamusHeightSlider.Value = 0;
        }

        private void rightGrowth_Click(object sender, EventArgs e)
        {
            rightAntegonialNotchSlider.Value = 100;
            rightCondyleDegenerationSlider.Value = 0;
            rightCondyleHeightSlider.Value = 30;
            rightCondyleRotationSlider.Value = 75;
            rightMandibularNotchSlider.Value = 55;
            rightRamusHeightSlider.Value = 80;
        }

        private void leftGrowth_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.Value = 100;
            leftCondyleDegenerationSlider.Value = 0;
            leftCondyleHeightSlider.Value = 30;
            leftCondyleRotationSlider.Value = 75;
            leftMandibularNotchSlider.Value = 55;
            leftRamusHeightSlider.Value = 80;
        }

        private void rightDegenerated_Click(object sender, EventArgs e)
        {
            rightAntegonialNotchSlider.Value = 100;
            rightCondyleDegenerationSlider.Value = 100;
            rightCondyleHeightSlider.Value = 60;
            rightCondyleRotationSlider.Value = 100;
            rightMandibularNotchSlider.Value = 60;
            rightRamusHeightSlider.Value = 0;
        }

        private void leftDegenerated_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.Value = 100;
            leftCondyleDegenerationSlider.Value = 100;
            leftCondyleHeightSlider.Value = 60;
            leftCondyleRotationSlider.Value = 100;
            leftMandibularNotchSlider.Value = 60;
            leftRamusHeightSlider.Value = 0;
        }
    }
}
