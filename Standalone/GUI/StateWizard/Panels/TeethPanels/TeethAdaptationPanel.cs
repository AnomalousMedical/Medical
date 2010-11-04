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

        GridPropertiesControl gridPropertiesControl;

        public TeethAdaptationPanel(StateWizardPanelController controller)
            : base("Medical.GUI.StateWizard.Panels.TeethPanels.TeethAdaptationPanel.layout", controller)
        {
            this.panelController = controller;
            teethMovementPanel = new TeethMovementPanel(controller, mainWidget);

            undoButton = mainWidget.findWidget("TeethAdaptationPanel/UndoButton") as Button;
            resetButton = mainWidget.findWidget("TeethAdaptationPanel/ResetButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            gridPropertiesControl.GridSpacing = 2;
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
            gridPropertiesControl.Origin = TeethController.getToothCenter();
            gridPropertiesControl.updateGrid();
            teethMovementPanel.setDefaultTools();
        }

        protected override void onPanelClosing()
        {
            TeethController.showTeethTools(false, false);
            controller.MeasurementGrid.Visible = false;
            teethMovementPanel.disableAllButtons();
        }

        void resetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the teeth alignment to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                TeethController.setAllOffsets(Vector3.Zero);
                TeethController.setAllRotations(Quaternion.Identity);
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the teeth alignment to before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
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
