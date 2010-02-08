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
    public partial class RightCondylarGrowthPanel : BoneManipulatorPanel
    {
        public RightCondylarGrowthPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Right Condyle Growth";
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            resetToOpeningState();
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            setToDefault();
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
