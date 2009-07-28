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
            leftDiscSpace.ValueChanged += new EventHandler(leftDiscSpace_ValueChanged);
            rightDiscSpace.ValueChanged += new EventHandler(rightDiscSpace_ValueChanged);
            horizontalDisc.ValueChanged += new EventHandler(horizontalDisc_ValueChanged);
        }

        void horizontalDisc_ValueChanged(object sender, EventArgs e)
        {
            switch (horizontalDisc.Value)
            {
                case -2:
                    horizontalSpace.Text = "Large Right Shift";
                    break;
                case -1:
                    horizontalSpace.Text = "Small Right Shift";
                    break;
                case 0:
                    horizontalSpace.Text = "Normal";
                    break;
                case 1:
                    horizontalSpace.Text = "Small Left Shift";
                    break;
                case 2:
                    horizontalSpace.Text = "Large Right Shift";
                    break;
            }
        }

        void rightDiscSpace_ValueChanged(object sender, EventArgs e)
        {
            switch (rightDiscSpace.Value)
            {
                case 0:
                    rightCondyleSpace.Text = "Large";
                    break;
                case 1:
                    rightCondyleSpace.Text = "Normal";
                    break;
                case 2:
                    rightCondyleSpace.Text = "Lightly Reduced";
                    break;
                case 3:
                    rightCondyleSpace.Text = "Greatly Reduced";
                    break;
                case 4:
                    rightCondyleSpace.Text = "Bone on Bone";
                    break;
            }
        }

        void leftDiscSpace_ValueChanged(object sender, EventArgs e)
        {
            switch (leftDiscSpace.Value)
            {
                case 0:
                    leftCondyleSpace.Text = "Large";
                    break;
                case 1:
                    leftCondyleSpace.Text = "Normal";
                    break;
                case 2:
                    leftCondyleSpace.Text = "Lightly Reduced";
                    break;
                case 3:
                    leftCondyleSpace.Text = "Greatly Reduced";
                    break;
                case 4:
                    leftCondyleSpace.Text = "Bone on Bone";
                    break;
            }
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
