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
    public partial class LeftCondylarDegenrationPanel : BoneManipulatorPanel
    {
        private bool open = false;
        private bool showingWear = false;

        public LeftCondylarDegenrationPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Left Condyle Degeneration";
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            leftCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (open && !showingWear)
            {
                this.setNavigationState("WizardLeftTMJ");
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (open && showingWear)
            {
                this.setNavigationState(NavigationState);
                showingWear = false;
            }
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset the condylar degeneration to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to undo the condylar degeneration to before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                resetToOpeningState();
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            open = true;
            gridPropertiesControl1.updateGrid();
            showingWear = false;
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            open = false;
            panelController.MeasurementGrid.Visible = false;
        }
    }
}
