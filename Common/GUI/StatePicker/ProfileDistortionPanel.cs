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
            AnimationManipulator ramus = MandibleController.Mandible.getAnimationManipulator("leftRamusHeightMandible");
            AnimationManipulator condyle = MandibleController.Mandible.getAnimationManipulator("leftCondyleHeightMandible");
            leftHeightSlider.initialize(condyle, ramus);

            ramus = MandibleController.Mandible.getAnimationManipulator("rightRamusHeightMandible");
            condyle = MandibleController.Mandible.getAnimationManipulator("rightCondyleHeightMandible");
            rightHeightSlider.initialize(condyle, ramus);
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            leftHeightSlider.getPositionFromScene();
            rightHeightSlider.getPositionFromScene();
            gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            panelController.MeasurementGrid.Visible = false;
        }

        private void rightSideCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("Right Lateral");
        }

        private void rightMidCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("Right Mid Anterior");
        }

        private void midlineCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("Midline Anterior");
        }

        private void leftMidCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("Left Mid Anterior");
        }

        private void leftSideCamera_Click(object sender, EventArgs e)
        {
            this.setNavigationState("Left Lateral");
        }

        void adaptButton_CheckedChanged(object sender, EventArgs e)
        {
            TeethController.adaptAllTeeth(adaptButton.Checked);
        }
    }
}
