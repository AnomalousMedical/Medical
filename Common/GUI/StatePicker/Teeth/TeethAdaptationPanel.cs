using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Medical.Properties;

namespace Medical.GUI
{
    public partial class TeethAdaptationPanel : StatePickerPanel
    {
        public TeethAdaptationPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Teeth Adaptation";
            gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
            teethMovementPanel1.initialize(panelController);
        }

        public override void applyToState(MedicalState state)
        {
            foreach(ToothState toothState in state.Teeth.StateEnum)
            {
                Tooth tooth = TeethController.getTooth(toothState.Name);
                toothState.Offset = tooth.Offset;
                toothState.Rotation = tooth.Rotation;
            }
        }

        public override void modifyScene()
        {
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

            leftCP.setLocation(leftCP.getNeutralLocation());
            rightCP.setLocation(rightCP.getNeutralLocation());
            movingMuscle.changeForce(TeethController.AdaptForce);
            movingMuscleTarget.Offset = Vector3.Zero;
        }

        protected override void onPanelOpening()
        {
            gridPropertiesControl1.Origin = TeethController.getToothCenter();
            gridPropertiesControl1.updateGrid();
            teethMovementPanel1.setDefaultTools();
        }

        protected override void onPanelClosing()
        {
            TeethController.showTeethTools(false, false);
            panelController.MeasurementGrid.Visible = false;
            teethMovementPanel1.disableAllButtons();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to undo the teeth to how they were before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                TeethState undo = panelController.StateBlender.UndoState.Teeth;
                foreach (ToothState toothState in undo.StateEnum)
                {
                    Tooth tooth = TeethController.getTooth(toothState.Name);
                    tooth.Offset = toothState.Offset;
                    tooth.Rotation = toothState.Rotation;
                }
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset the teeth to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                TeethController.setAllOffsets(Vector3.Zero);
                TeethController.setAllRotations(Quaternion.Identity);
            }
        }
    }
}
