using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Logging;

namespace Medical.GUI
{
    public partial class LeftCondylarGrowthPanel : BoneManipulatorPanel
    {
        public LeftCondylarGrowthPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Left Condyle Growth";
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset the condylar growth to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to undo the condylar growth to before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                resetToOpeningState();
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            panelController.MeasurementGrid.Visible = false;
        }
    }
}
