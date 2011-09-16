using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class TeethHeightAdaptationGUI : TimelineWizardPanel
    {
        private TeethMovementGUI teethMovementPanel;
        private HeightControl heightControl;

        private Button undoButton;
        private Button makeNormalButton;

        private AnatomyPickingMode startingPickingMode;
        private TimelineWizard wizard;

        //GridPropertiesControl gridPropertiesControl;

        public TeethHeightAdaptationGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Teeth.TeethHeightAdaptationGUI.layout", wizard)
        {
            //gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            //gridPropertiesControl.GridSpacing = 2;

            this.wizard = wizard;
            startingPickingMode = wizard.AnatomyController.PickingMode;
            wizard.AnatomyController.PickingMode = AnatomyPickingMode.None;
        }

        public override void initialize(ShowTimelineGUIAction showGUIAction)
        {
            heightControl = new HeightControl(widget.findWidget("TeethAdaptPanel/LeftSideSlider") as VScroll,
                widget.findWidget("TeethAdaptPanel/RightSideSlider") as VScroll,
                widget.findWidget("TeethAdaptPanel/BothSidesSlider") as VScroll);
            teethMovementPanel = new TeethMovementGUI(widget, (TeethAdaptationGUIData)showGUIAction.GUIData, this);

            undoButton = widget.findWidget("TeethAdaptPanel/UndoButton") as Button;
            makeNormalButton = widget.findWidget("TeethAdaptPanel/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            base.initialize(showGUIAction);
        }

        public override void Dispose()
        {
            TeethController.showTeethTools(false, false);
            //controller.MeasurementGrid.Visible = false;
            teethMovementPanel.disableAllButtons();
            base.Dispose();
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
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

            //gridPropertiesControl.Origin = TeethController.getToothCenter();
            //gridPropertiesControl.updateGrid();
            teethMovementPanel.setDefaultTools();
            heightControl.getPositionFromScene();
        }

        protected override void closing()
        {
            base.closing();
            wizard.AnatomyController.PickingMode = startingPickingMode;
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
                TeethState undo = timelineWizard.StateBlender.UndoState.Teeth;
                foreach (ToothState toothState in undo.StateEnum)
                {
                    Tooth tooth = TeethController.getTooth(toothState.Name);
                    tooth.Offset = toothState.Offset;
                    tooth.Rotation = toothState.Rotation;
                }
                AnimationManipulatorState animUndo = timelineWizard.StateBlender.UndoState.BoneManipulator;
                animUndo.blend(animUndo, 0.0f);
                heightControl.getPositionFromScene();
            }
        }
    }
}
