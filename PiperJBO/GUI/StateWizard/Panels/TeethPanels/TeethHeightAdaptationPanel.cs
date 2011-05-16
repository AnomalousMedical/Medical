using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class TeethHeightAdaptationPanel : StateWizardPanel
    {
        private TeethMovementPanel teethMovementPanel;
        private HeightControl heightControl;

        private Button undoButton;
        private Button makeNormalButton;

        GridPropertiesControl gridPropertiesControl;

        public TeethHeightAdaptationPanel(StateWizardPanelController panelController)
            : base("Medical.GUI.StateWizard.Panels.TeethPanels.TeethHeightAdaptationPanel.layout", panelController)
        {
            heightControl = new HeightControl(mainWidget.findWidget("TeethAdaptPanel/LeftSideSlider") as VScroll,
                mainWidget.findWidget("TeethAdaptPanel/RightSideSlider") as VScroll,
                mainWidget.findWidget("TeethAdaptPanel/BothSidesSlider") as VScroll);
            teethMovementPanel = new TeethMovementPanel(controller, mainWidget);

            undoButton = mainWidget.findWidget("TeethAdaptPanel/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("TeethAdaptPanel/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

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

            leftCP.setLocation(leftCP.NeutralLocation);
            rightCP.setLocation(rightCP.NeutralLocation);
            movingMuscle.changeForce(TeethController.AdaptForce);
            movingMuscleTarget.Offset = Vector3.Zero;
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            heightControl.sceneChanged();
        }

        protected override void onPanelOpening()
        {
            gridPropertiesControl.Origin = TeethController.getToothCenter();
            gridPropertiesControl.updateGrid();
            teethMovementPanel.setDefaultTools();
            heightControl.getPositionFromScene();
        }

        protected override void onPanelClosing()
        {
            TeethController.showTeethTools(false, false);
            controller.MeasurementGrid.Visible = false;
            teethMovementPanel.disableAllButtons();
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
                TeethState undo = controller.StateBlender.UndoState.Teeth;
                foreach (ToothState toothState in undo.StateEnum)
                {
                    Tooth tooth = TeethController.getTooth(toothState.Name);
                    tooth.Offset = toothState.Offset;
                    tooth.Rotation = toothState.Rotation;
                }
                AnimationManipulatorState animUndo = controller.StateBlender.UndoState.BoneManipulator;
                animUndo.blend(animUndo, 0.0f);
                heightControl.getPositionFromScene();
            }
        }
    }
}
