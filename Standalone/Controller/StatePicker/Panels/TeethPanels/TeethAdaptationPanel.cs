using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TeethAdaptationPanel : StateWizardPanel
    {
        private TeethMovementPanel teethMovementPanel;
        private StateWizardPanelController panelController;

        private Button undoButton;
        private Button resetButton;

        public TeethAdaptationPanel(String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            this.panelController = controller;
            //gridPropertiesControl1.setGrid(panelController.MeasurementGrid);
            teethMovementPanel = new TeethMovementPanel(controller, mainWidget);

            undoButton = mainWidget.findWidget("TeethAdaptationPanel/UndoButton") as Button;
            resetButton = mainWidget.findWidget("TeethAdaptationPanel/ResetButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);
        }

        public override void applyToState(MedicalState state)
        {
            foreach (ToothState toothState in state.Teeth.StateEnum)
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
            //gridPropertiesControl1.Origin = TeethController.getToothCenter();
            //gridPropertiesControl1.updateGrid();
            teethMovementPanel.setDefaultTools();
        }

        protected override void onPanelClosing()
        {
            TeethController.showTeethTools(false, false);
            //panelController.MeasurementGrid.Visible = false;
            teethMovementPanel.disableAllButtons();
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to reset the teeth to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                TeethController.setAllOffsets(Vector3.Zero);
                TeethController.setAllRotations(Quaternion.Identity);
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to undo the teeth to how they were before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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
    }
}
