using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI
{
    public partial class DiscSpacePanel : StatePickerPanel
    {
        public DiscSpacePanel()
        {
            InitializeComponent();
            this.Text = "Disc Spacing";
        }

        public override void applyToState(MedicalState state)
        {
            DiscState disc = state.Disc;
            getDiscState(disc, "LeftTMJDisc", leftDiscSpace.Value);
            getDiscState(disc, "RightTMJDisc", rightDiscSpace.Value);
        }

        private void getDiscState(DiscState discState, String discName, int verticalValue)
        {
            Disc disc = DiscController.getDisc(discName);
            Vector3 discSpacing = disc.getNormalOffset() / 3.0f;
            switch (verticalValue)
            {
                case 0:
                    discSpacing = discSpacing * 4.0f;
                    break;
                case 1:
                    discSpacing = disc.getNormalOffset();
                    break;
                case 2:
                    discSpacing = discSpacing * 2.0f;
                    break;
                case 3:
                    discSpacing = discSpacing * 1.0f;
                    break;
                case 4:
                    discSpacing = Vector3.Zero;
                    break;
            }
            discSpacing += disc.HorizontalTickSpacing * horizontalDisc.Value;
            discState.addPosition(discName, discSpacing);
        }

        public override void setToDefault()
        {
            
        }
    }
}
