using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class ProfileDistortionGUI : TimelineWizardPanel
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

        private ProfileDistortionGUIData profileGUIData;

        public ProfileDistortionGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Profile.ProfileDistortionGUI.layout", wizard)
        {
            rightSideCamera = widget.findWidget("ProfilePanel/RightCamera") as Button;
            rightMidCamera = widget.findWidget("ProfilePanel/RightMidCamera") as Button;
            midlineCamera = widget.findWidget("ProfilePanel/MidlineCamera") as Button;
            leftMidCamera = widget.findWidget("ProfilePanel/MidLeftCamera") as Button;
            leftSideCamera = widget.findWidget("ProfilePanel/LeftCamera") as Button;

            rightSideCamera.MouseButtonClick += new MyGUIEvent(rightSideCamera_MouseButtonClick);
            rightMidCamera.MouseButtonClick += new MyGUIEvent(rightMidCamera_MouseButtonClick);
            midlineCamera.MouseButtonClick += new MyGUIEvent(midlineCamera_MouseButtonClick);
            leftMidCamera.MouseButtonClick += new MyGUIEvent(leftMidCamera_MouseButtonClick);
            leftSideCamera.MouseButtonClick += new MyGUIEvent(leftSideCamera_MouseButtonClick);

            heightControl = new HeightControl(widget.findWidget("ProfilePanel/LeftSideSlider") as VScroll,
                widget.findWidget("ProfilePanel/RightSideSlider") as VScroll,
                widget.findWidget("ProfilePanel/BothSidesSlider") as VScroll);

            adaptButton = new CheckButton(widget.findWidget("ProfilePanel/AdaptButton") as Button);
            adaptButton.CheckedChanged += new MyGUIEvent(adaptButton_CheckedChanged);

            undoButton = widget.findWidget("ProfilePanel/UndoButton") as Button;
            makeNormalButton = widget.findWidget("ProfilePanel/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);
        }

        public override void initialize(Medical.ShowTimelineGUIAction showGUIAction)
        {
            base.initialize(showGUIAction);
            profileGUIData = (ProfileDistortionGUIData)showGUIAction.GUIData;
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            heightControl.sceneChanged();
            heightControl.getPositionFromScene();
        }

        void leftSideCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            applyCameraPosition(profileGUIData.LeftSideCamera);
        }

        void leftMidCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            applyCameraPosition(profileGUIData.LeftMidCamera);
        }

        void midlineCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            applyCameraPosition(profileGUIData.MidlineCamera);
        }

        void rightMidCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            applyCameraPosition(profileGUIData.RightMidCamera);
        }

        void rightSideCamera_MouseButtonClick(Widget source, EventArgs e)
        {
            applyCameraPosition(profileGUIData.RightCamera);
        }

        void adaptButton_CheckedChanged(Widget sender, EventArgs e)
        {
            if (adaptButton.Checked)
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
