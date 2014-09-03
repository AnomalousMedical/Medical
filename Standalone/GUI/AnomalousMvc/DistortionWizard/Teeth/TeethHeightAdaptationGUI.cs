using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class TeethHeightAdaptationGUI : WizardComponent<TeethHeightAdaptationView>
    {
        private TeethMovementGUI teethMovementPanel;
        private HeightControl heightControl;

        private Button undoButton;
        private Button makeNormalButton;

        private AnatomyPickingMode startingPickingMode;

        GridPropertiesControl gridPropertiesControl;

        public TeethHeightAdaptationGUI(TeethHeightAdaptationView wizardView, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Teeth.TeethHeightAdaptationGUI.layout", wizardView, context, viewHost)
        {
            gridPropertiesControl = new GridPropertiesControl(context.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 2;

            startingPickingMode = context.AnatomyController.PickingMode;
            context.AnatomyController.PickingMode = AnatomyPickingMode.None;

            heightControl = new HeightControl(widget.findWidget("TeethAdaptPanel/LeftSideSlider") as ScrollBar,
                widget.findWidget("TeethAdaptPanel/RightSideSlider") as ScrollBar,
                widget.findWidget("TeethAdaptPanel/BothSidesSlider") as ScrollBar);
            teethMovementPanel = new TeethMovementGUI(widget, wizardView, context);

            undoButton = widget.findWidget("TeethAdaptPanel/UndoButton") as Button;
            makeNormalButton = widget.findWidget("TeethAdaptPanel/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            teethMovementPanel.disableAllButtons();
            base.Dispose();
        }

        public override void opening()
        {
            heightControl.sceneChanged();

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
            heightControl.getPositionFromScene();
        }

        public override void closing()
        {
            base.closing();
            context.AnatomyController.PickingMode = startingPickingMode;
            TeethController.showTeethTools(false, false);
            context.MeasurementGrid.Visible = false;
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the teeth to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle style)
        {
            if (style == MessageBoxStyle.Yes)
            {
                TeethController.setAllOffsets(Vector3.Zero);
                TeethController.setAllRotations(Quaternion.Identity);
                heightControl.setToDefault();
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the teeth to how they were before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle style)
        {
            if (style == MessageBoxStyle.Yes)
            {
                context.runAction(wizardView.UndoAction, ViewHost);
                heightControl.getPositionFromScene();
            }
        }
    }
}
