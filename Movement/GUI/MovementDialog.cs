using BEPUikPlugin;
using Engine.ObjectManagement;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Movement.GUI
{
    class MovementDialog : PinableMDIDialog
    {
        private MedicalController medicalController;
        private MusclePositionController musclePositionController;
        private PoseController poseController;

        private Widget controlButtonPanel;
        private Button undoButton;
        private Button redoButton;

        public MovementDialog(MusclePositionController musclePositionController, PoseController poseController, MedicalController medicalController)
            : base("Medical.Movement.GUI.MovementDialog.layout")
        {
            this.medicalController = medicalController;
            this.musclePositionController = musclePositionController;
            this.poseController = poseController;
            poseController.PoseModeActivated += poseController_PoseModeActivated;
            poseController.PoseModeDeactivated += poseController_PoseModeDeactivated;

            musclePositionController.OnUndoRedoChanged += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnRedo += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnUndo += musclePositionController_UndoRedoStateAltered;

            controlButtonPanel = window.findWidget("ControlButtonPanel");
            foreach(Widget widget in controlButtonPanel.Children)
            {
                Button button = widget as Button;
                if (button != null)
                {
                    button.MouseButtonClick += widget_MouseButtonClick;
                }
            }

            undoButton = window.findWidget("Undo") as Button;
            undoButton.MouseButtonClick += undoButton_MouseButtonClick;

            redoButton = window.findWidget("Redo") as Button;
            redoButton.MouseButtonClick += redoButton_MouseButtonClick;

            Button resetButton = (Button)window.findWidget("Reset");
            resetButton.MouseButtonClick += resetButton_MouseButtonClick;

            musclePositionController_UndoRedoStateAltered(musclePositionController);
        }

        void widget_MouseButtonClick(Widget source, EventArgs e)
        {
            Button button = source as Button;
            poseController.setMode(button.Name, !button.Selected);
        }

        public override void Dispose()
        {
            musclePositionController.OnUndoRedoChanged -= musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnRedo -= musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnUndo -= musclePositionController_UndoRedoStateAltered;
            base.Dispose();
        }

        void redoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            musclePositionController.redo();
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            musclePositionController.undo(); 
        }

        private void resetButton_MouseButtonClick(object sender, EventArgs e)
        {
            musclePositionController.pushUndoState(new MusclePosition(true), musclePositionController.BindPosition);

            musclePositionController.timedBlend(musclePositionController.BindPosition, MedicalConfig.CameraTransitionTime);
        }

        void poseController_PoseModeActivated(PoseController poseController, string mode)
        {
            Button button = controlButtonPanel.findWidget(mode) as Button;
            if(button != null)
            {
                button.Selected = true;
            }
        }

        void poseController_PoseModeDeactivated(PoseController poseController, string mode)
        {
            Button button = controlButtonPanel.findWidget(mode) as Button;
            if (button != null)
            {
                button.Selected = false;
            }
        }

        void musclePositionController_UndoRedoStateAltered(MusclePositionController musclePositionController)
        {
            undoButton.Enabled = musclePositionController.HasUndo;
            redoButton.Enabled = musclePositionController.HasRedo;
        }
    }
}
