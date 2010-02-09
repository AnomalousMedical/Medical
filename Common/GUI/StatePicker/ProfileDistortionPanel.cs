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
    public partial class ProfileDistortionPanel : StatePickerPanel
    {
        public ProfileDistortionPanel(StatePickerPanelController panelController)
            :base(panelController)
        {
            InitializeComponent();
            adaptButton.CheckedChanged += new EventHandler(adaptButton_CheckedChanged);
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            heightControl1.sceneChanged();
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            heightControl1.getPositionFromScene();
            gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            panelController.MeasurementGrid.Visible = false;
        }

        private void rightSideCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardRightLateral");
        }

        private void rightMidCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardRightMidAnterior");
        }

        private void midlineCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardMidlineAnterior");
        }

        private void leftMidCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardLeftMidAnterior");
        }

        private void leftSideCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("WizardLeftLateral");
        }

        void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.adaptAllTeeth(adaptButton.Checked);
        }
    }
}
