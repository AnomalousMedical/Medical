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
            if (adaptButton.Checked)
            {
                ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
                ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
                MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
                MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

                leftCP.setLocation(leftCP.getNeutralLocation());
                rightCP.setLocation(rightCP.getNeutralLocation());
                movingMuscle.changeForce(2.0f);
                movingMuscleTarget.Offset = Vector3.Zero;
            }

            TeethController.adaptAllTeeth(adaptButton.Checked);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            TeethState undo = panelController.StateBlender.UndoState.Teeth;
            foreach (ToothState toothState in undo.StateEnum)
            {
                Tooth tooth = TeethController.getTooth(toothState.Name);
                tooth.Offset = toothState.Offset;
                tooth.Rotation = toothState.Rotation;
            }
            AnimationManipulatorState animUndo = panelController.StateBlender.UndoState.BoneManipulator;
            animUndo.blend(animUndo, 0.0f);
            heightControl1.getPositionFromScene();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            TeethController.setAllOffsets(Vector3.Zero);
            TeethController.setAllRotations(Quaternion.Identity);
            heightControl1.setToDefault();
        }
    }
}
