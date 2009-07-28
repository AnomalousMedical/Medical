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
    public partial class FossaStatePanel : StatePickerPanel
    {
        public FossaStatePanel()
        {
            InitializeComponent();
            this.Text = "Fossa Flatness";
        }

        public override void applyToState(MedicalState state)
        {
            getRightFossaState(state.Fossa);
            getLeftFossaState(state.Fossa);
        }

        public override void setToDefault()
        {
            rightEminenceNormal.Checked = true;
            leftEminenceNormal.Checked = true;
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
    }
}
