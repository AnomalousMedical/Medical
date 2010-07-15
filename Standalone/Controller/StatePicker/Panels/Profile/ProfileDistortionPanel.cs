using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ProfileDistortionPanel : StateWizardPanel
    {
        private Button rightSideCamera;
        private Button rightMidCamera;
        private Button midlineCamera;
        private Button leftMidCamera;
        private Button leftSideCamera;

        private HeightControl heightControl;

        private CheckButton adaptButton;

        private Button undoButton;
        private Button makeNormalButton;

        public ProfileDistortionPanel(String panelFile, StateWizardPanelController controller)
            :base(panelFile, controller)
        {
            

            //gridPropertiesControl1.setGrid(panelController.MeasurementGrid);

            rightSideCamera = mainWidget.findWidget("ProfilePanel/RightCamera") as Button;
            rightMidCamera = mainWidget.findWidget("ProfilePanel/RightMidCamera") as Button;
            midlineCamera = mainWidget.findWidget("ProfilePanel/MidlineCamera") as Button;
            leftMidCamera = mainWidget.findWidget("ProfilePanel/MidLeftCamera") as Button;
            leftSideCamera = mainWidget.findWidget("ProfilePanel/LeftCamera") as Button;

            rightSideCamera.MouseButtonClick += new MyGUIEvent(rightSideCamera_MouseButtonClick);
            rightMidCamera.MouseButtonClick += new MyGUIEvent(rightMidCamera_MouseButtonClick);
            midlineCamera.MouseButtonClick += new MyGUIEvent(midlineCamera_MouseButtonClick);
            leftMidCamera.MouseButtonClick += new MyGUIEvent(leftMidCamera_MouseButtonClick);
            leftSideCamera.MouseButtonClick += new MyGUIEvent(leftSideCamera_MouseButtonClick);

            heightControl = new HeightControl(mainWidget.findWidget("ProfilePanel/LeftSideSlider") as VScroll,
                mainWidget.findWidget("ProfilePanel/RightSideSlider") as VScroll,
                mainWidget.findWidget("ProfilePanel/BothSidesSlider") as VScroll);

            adaptButton = new CheckButton(mainWidget.findWidget("ProfilePanel/AdaptButton") as Button);
            adaptButton.CheckedChanged += new MyGUIEvent(adaptButton_CheckedChanged);

            undoButton = mainWidget.findWidget("ProfilePanel/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("ProfilePanel/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            heightControl.sceneChanged();
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            heightControl.getPositionFromScene();
            //gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            //panelController.MeasurementGrid.Visible = false;
        }

        void leftSideCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardLeftLateral");
        }

        void leftMidCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardLeftMidAnterior");
        }

        void midlineCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardMidlineAnterior");
        }

        void rightMidCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardRightMidAnterior");
        }

        void rightSideCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            this.setNavigationState("WizardRightLateral");
        }

        void adaptButton_CheckedChanged(Widget sender, EventArgs e)
        {
            if (adaptButton.Checked)
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

            TeethController.adaptAllTeeth(adaptButton.Checked);
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the profile to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
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
            MessageBox.show("Are you sure you want to undo the profile to how it was before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
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
