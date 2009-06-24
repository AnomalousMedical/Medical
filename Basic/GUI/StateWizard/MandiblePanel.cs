using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.StateWizard
{
    public partial class MandiblePanel : StateWizardPanel
    {
        public MandiblePanel()
        {
            InitializeComponent();
        }

        public override void applyToState(MedicalState state)
        {
            BoneManipulatorState boneManipulator = new BoneManipulatorState();
            getRightGrowthDefect(boneManipulator);
            getRightDegeneration(boneManipulator);
            getLeftGrowthDefect(boneManipulator);
            getLeftDegeneration(boneManipulator);
            state.BoneManipulator = boneManipulator;
        }

        public override void setToDefault()
        {
            leftDegenerationNormal.Checked = true;
            rightDegenerationNormal.Checked = true;
            leftGrowthDefectNormal.Checked = true;
            rightGrowthDefectNormal.Checked = true;
        }

        private void getRightGrowthDefect(BoneManipulatorState boneManipulator)
        {
            if (rightGrowthDefectNormal.Checked)
            {
                boneManipulator.addPosition("rightRamusHeightMandible", 0.0f);
                boneManipulator.addPosition("rightAntegonialNotchMandible", 0.0f);
                boneManipulator.addPosition("rightCondyleHeightMandible", 0.0f);
                boneManipulator.addPosition("rightMandibularNotchMandible", 0.0f);
                boneManipulator.addPosition("rightCondyleRotationMandible", 0.0f);
            }
            else if (rightGrowthDefectModerate.Checked)
            {
                boneManipulator.addPosition("rightRamusHeightMandible", 0.5f);
                boneManipulator.addPosition("rightAntegonialNotchMandible", 0.5f);
                boneManipulator.addPosition("rightCondyleHeightMandible", 0.5f);
                boneManipulator.addPosition("rightMandibularNotchMandible", 0.5f);
                boneManipulator.addPosition("rightCondyleRotationMandible", 0.5f);
            }
            else if (rightGrowthDefectSevere.Checked)
            {
                boneManipulator.addPosition("rightRamusHeightMandible", 1.0f);
                boneManipulator.addPosition("rightAntegonialNotchMandible", 1.0f);
                boneManipulator.addPosition("rightCondyleHeightMandible", 1.0f);
                boneManipulator.addPosition("rightMandibularNotchMandible", 1.0f);
                boneManipulator.addPosition("rightCondyleRotationMandible", 1.0f);
            }
        }

        private void getRightDegeneration(BoneManipulatorState boneManipulator)
        {
            if (rightDegenerationNormal.Checked)
            {
                boneManipulator.addPosition("rightCondyleDegenerationMandible", 0.0f);
            }
            else if (rightDegenerationModerate.Checked)
            {
                boneManipulator.addPosition("rightCondyleDegenerationMandible", 0.5f);
            }
            else if (rightDegenerationSevere.Checked)
            {
                boneManipulator.addPosition("rightCondyleDegenerationMandible", 1.0f);
            }
        }

        private void getLeftGrowthDefect(BoneManipulatorState boneManipulator)
        {
            if (leftGrowthDefectNormal.Checked)
            {
                boneManipulator.addPosition("leftRamusHeightMandible", 0.0f);
                boneManipulator.addPosition("leftAntegonialNotchMandible", 0.0f);
                boneManipulator.addPosition("leftCondyleHeightMandible", 0.0f);
                boneManipulator.addPosition("leftMandibularNotchMandible", 0.0f);
                boneManipulator.addPosition("leftCondyleRotationMandible", 0.0f);
            }
            else if (leftGrowthDefectModerate.Checked)
            {
                boneManipulator.addPosition("leftRamusHeightMandible", 0.5f);
                boneManipulator.addPosition("leftAntegonialNotchMandible", 0.5f);
                boneManipulator.addPosition("leftCondyleHeightMandible", 0.5f);
                boneManipulator.addPosition("leftMandibularNotchMandible", 0.5f);
                boneManipulator.addPosition("leftCondyleRotationMandible", 0.5f);
            }
            else if (leftGrowthDefectSevere.Checked)
            {
                boneManipulator.addPosition("leftRamusHeightMandible", 1.0f);
                boneManipulator.addPosition("leftAntegonialNotchMandible", 1.0f);
                boneManipulator.addPosition("leftCondyleHeightMandible", 1.0f);
                boneManipulator.addPosition("leftMandibularNotchMandible", 1.0f);
                boneManipulator.addPosition("leftCondyleRotationMandible", 1.0f);
            }
        }

        private void getLeftDegeneration(BoneManipulatorState boneManipulator)
        {
            if (leftDegenerationNormal.Checked)
            {
                boneManipulator.addPosition("leftCondyleDegenerationMandible", 0.0f);
            }
            else if (leftDegenerationModerate.Checked)
            {
                boneManipulator.addPosition("leftCondyleDegenerationMandible", 0.5f);
            }
            else if (leftDegenerationSevere.Checked)
            {
                boneManipulator.addPosition("leftCondyleDegenerationMandible", 1.0f);
            }
        }
    }
}

/*
rightRamusHeightMandible
rightAntegonialNotchMandible
rightCondyleHeightMandible
rightMandibularNotchMandible
rightCondyleDegenerationMandible
rightCondyleRotationMandible
 
leftRamusHeightMandible
leftCondyleHeightMandible
leftAntegonialNotchMandible
leftMandibularNotchMandible
leftCondyleDegenerationMandible
leftCondyleRotationMandible

*/