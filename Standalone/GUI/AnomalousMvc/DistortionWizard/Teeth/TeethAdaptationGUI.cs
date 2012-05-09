using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class TeethAdaptationGUI : WizardPanel<TeethAdaptationView>
    {
        private TeethMovementGUI teethMovementPanel;

        private Button undoButton;
        private Button resetButton;

        private AnatomyPickingMode startingPickingMode;

        GridPropertiesControl gridPropertiesControl;

        public TeethAdaptationGUI(TeethAdaptationView wizardView, AnomalousMvcContext context)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Teeth.TeethAdaptationGUI.layout", wizardView, context)
        {
            gridPropertiesControl = new GridPropertiesControl(context.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 2;

            startingPickingMode = context.AnatomyController.PickingMode;
            context.AnatomyController.PickingMode = AnatomyPickingMode.None;

            teethMovementPanel = new TeethMovementGUI(widget, wizardView, context);

            undoButton = widget.findWidget("TeethAdaptationPanel/UndoButton") as Button;
            resetButton = widget.findWidget("TeethAdaptationPanel/ResetButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            resetButton.MouseButtonClick += new MyGUIEvent(resetButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            teethMovementPanel.disableAllButtons();
            base.Dispose();
        }

        public override void opening()
        {
            ControlPointBehavior leftCP = ControlPointController.getControlPoint("LeftCP");
            ControlPointBehavior rightCP = ControlPointController.getControlPoint("RightCP");
            MuscleBehavior movingMuscle = MuscleController.getMuscle("MovingMuscleDynamic");
            MovingMuscleTarget movingMuscleTarget = MuscleController.MovingTarget;

            leftCP.setLocation(leftCP.NeutralLocation);
            rightCP.setLocation(rightCP.NeutralLocation);
            movingMuscle.changeForce(TeethController.AdaptForce);
            movingMuscleTarget.Offset = Vector3.Zero;

            gridPropertiesControl.Origin = TeethController.getToothCenter();
            gridPropertiesControl.updateGrid();
            teethMovementPanel.setDefaultTools();

            base.opening();
        }

        public override void closing()
        {
            base.closing();
            context.AnatomyController.PickingMode = startingPickingMode;
            TeethController.showTeethTools(false, false);
            context.MeasurementGrid.Visible = false;
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
                context.runAction(wizardView.UndoAction, this);
            }
        }
    }
}
