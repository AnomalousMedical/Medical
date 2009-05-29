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
using Engine.Platform;

namespace Medical.GUI
{
    public partial class MandibleSizeControl : DockContent
    {
        float rightBlendPercent = 0.0f;
        float leftBlendPercent = 0.0f;
        float blendRate = 0.25f;

        private MedicalController controller;

        public MandibleSizeControl()
        {
            InitializeComponent();
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
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

        void startRightTimer()
        {
            controller.LoopUpdate += updateRightTimer;
            rightBlendPercent = 0.0f;
        }

        void stopRightTimer()
        {
            controller.LoopUpdate -= updateRightTimer;
        }

        public void updateRightTimer(Clock clock)
        {
            rightBlendPercent += blendRate * (float)clock.Seconds;
            if (rightBlendPercent >= 1.0f)
            {
                stopRightTimer();
                rightBlendPercent = 1.0f;
            }
            rightAntegonialNotchSlider.blend(rightBlendPercent);
            rightCondyleDegenerationSlider.blend(rightBlendPercent);
            rightCondyleHeightSlider.blend(rightBlendPercent);
            rightCondyleRotationSlider.blend(rightBlendPercent);
            rightMandibularNotchSlider.blend(rightBlendPercent);
            rightRamusHeightSlider.blend(rightBlendPercent);
        }

        void startLeftTimer()
        {
            controller.LoopUpdate += updateLeftTimer;
            leftBlendPercent = 0.0f;
        }

        void stopLeftTimer()
        {
            controller.LoopUpdate -= updateLeftTimer;
        }

        public void updateLeftTimer(Clock clock)
        {
            leftBlendPercent += blendRate * (float)clock.Seconds;
            if (leftBlendPercent >= 1.0f)
            {
                stopLeftTimer();
                leftBlendPercent = 1.0f;
            }
            leftAntegonialNotchSlider.blend(leftBlendPercent);
            leftCondyleDegenerationSlider.blend(leftBlendPercent);
            leftCondyleHeightSlider.blend(leftBlendPercent);
            leftCondyleRotationSlider.blend(leftBlendPercent);
            leftMandibularNotchSlider.blend(leftBlendPercent);
            leftRamusHeightSlider.blend(leftBlendPercent);
        }

        private void rightNormal_Click(object sender, EventArgs e)
        {
            rightAntegonialNotchSlider.startBlend(0f);
            rightCondyleDegenerationSlider.startBlend(0f);
            rightCondyleHeightSlider.startBlend(0f);
            rightCondyleRotationSlider.startBlend(0f);
            rightMandibularNotchSlider.startBlend(0f);
            rightRamusHeightSlider.startBlend(0f);
            startRightTimer();
        }

        private void leftNormal_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.startBlend(0f);
            leftCondyleDegenerationSlider.startBlend(0f);
            leftCondyleHeightSlider.startBlend(0f);
            leftCondyleRotationSlider.startBlend(0f);
            leftMandibularNotchSlider.startBlend(0f);
            leftRamusHeightSlider.startBlend(0f);
            startLeftTimer();
        }

        private void rightGrowth_Click(object sender, EventArgs e)
        {
            rightAntegonialNotchSlider.startBlend(1f);
            rightCondyleDegenerationSlider.startBlend(0f);
            rightCondyleHeightSlider.startBlend(.3f);
            rightCondyleRotationSlider.startBlend(.75f);
            rightMandibularNotchSlider.startBlend(.55f);
            rightRamusHeightSlider.startBlend(.80f);
            startRightTimer();
        }

        private void leftGrowth_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.startBlend(1f);
            leftCondyleDegenerationSlider.startBlend(0f);
            leftCondyleHeightSlider.startBlend(.3f);
            leftCondyleRotationSlider.startBlend(.75f);
            leftMandibularNotchSlider.startBlend(.55f);
            leftRamusHeightSlider.startBlend(.8f);
            startLeftTimer();
        }

        private void rightDegenerated_Click(object sender, EventArgs e)
        {
            rightAntegonialNotchSlider.startBlend(1f);
            rightCondyleDegenerationSlider.startBlend(1f);
            rightCondyleHeightSlider.startBlend(.6f);
            rightCondyleRotationSlider.startBlend(1);
            rightMandibularNotchSlider.startBlend(.6f);
            rightRamusHeightSlider.startBlend(0f);
            startRightTimer();
        }

        private void leftDegenerated_Click(object sender, EventArgs e)
        {
            leftAntegonialNotchSlider.startBlend(1f);
            leftCondyleDegenerationSlider.startBlend(1f);
            leftCondyleHeightSlider.startBlend(.6f);
            leftCondyleRotationSlider.startBlend(1f);
            leftMandibularNotchSlider.startBlend(.6f);
            leftRamusHeightSlider.startBlend(0f);
            startLeftTimer();
        }
    }
}
