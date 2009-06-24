using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI.StateWizard
{
    public partial class JointPanel : StateWizardPanel
    {
        public JointPanel()
        {
            InitializeComponent();
        }

        public override void applyToState(MedicalState state)
        {
            FossaState fossaState = new FossaState();
            getRightFossaState(fossaState);
            getLeftFossaState(fossaState);
            state.Fossa = fossaState;

            DiscState discState = new DiscState();
            getRightDiscState(discState);
            getLeftDiscState(discState);
            state.Disc = discState;
        }

        public override void setToDefault()
        {
            rightEminenceNormal.Checked = true;
            leftEminenceNormal.Checked = true;
            rightNormalJointSpace.Checked = true;
            leftNormalJointSpace.Checked = true;
        }

        private void getRightFossaState(FossaState fossaState)
        {
            if (rightEminenceNormal.Checked)
            {
                fossaState.addPosition("RightFossa", 0.0f);
            }
            else if (rightEminenceModerate.Checked)
            {
                fossaState.addPosition("RightFossa", 0.5f);
            }
            else if (rightEminenceSevere.Checked)
            {
                fossaState.addPosition("RightFossa", 1.0f);
            }
        }

        private void getLeftFossaState(FossaState fossaState)
        {
            if (leftEminenceNormal.Checked)
            {
                fossaState.addPosition("LeftFossa", 0.0f);
            }
            else if (leftEminenceModerate.Checked)
            {
                fossaState.addPosition("LeftFossa", 0.5f);
            }
            else if (leftEminenceSevere.Checked)
            {
                fossaState.addPosition("LeftFossa", 1.0f);
            }
        }

        private void getRightDiscState(DiscState discState)
        {
            Disc disc = DiscController.getDisc("RightTMJDisc");
            Vector3 discSpacing = disc.getNormalOffset();
            if (rightNormalJointSpace.Checked)
            {
                discState.addPosition("RightTMJDisc", discSpacing);
            }
            else if (rightReducedJointSpace.Checked)
            {
                discState.addPosition("RightTMJDisc", discSpacing / 2.0f);
            }
            else if (rightBoneOnBone.Checked)
            {
                discState.addPosition("RightTMJDisc", Vector3.Zero);
            }
        }

        private void getLeftDiscState(DiscState discState)
        {
            Disc disc = DiscController.getDisc("LeftTMJDisc");
            Vector3 discSpacing = disc.getNormalOffset();
            if (leftNormalJointSpace.Checked)
            {
                discState.addPosition("LeftTMJDisc", discSpacing);
            }
            else if (leftReducedJointSpace.Checked)
            {
                discState.addPosition("LeftTMJDisc", discSpacing / 2.0f);
            }
            else if (leftBoneOnBone.Checked)
            {
                discState.addPosition("LeftTMJDisc", Vector3.Zero);
            }
        }
    }
}
