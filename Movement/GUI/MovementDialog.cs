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
    class MovementDialog : MDIDialog
    {
        private MedicalController medicalController;
        private MusclePositionController musclePositionController;
        private SceneControlManager sceneControlManager;

        CheckButton cSpineFlexExt;
        CheckButton cSpineLateral;
        CheckButton cSpineAxial;

        CheckButton showPinControls;
        Button undoButton;
        Button redoButton;

        public MovementDialog(MusclePositionController musclePositionController, MedicalController medicalController, SceneControlManager sceneControlManager)
            : base("Medical.Movement.GUI.MovementDialog.layout")
        {
            this.medicalController = medicalController;
            this.musclePositionController = musclePositionController;
            this.sceneControlManager = sceneControlManager;

            musclePositionController.OnUndoRedoChanged += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnRedo += musclePositionController_UndoRedoStateAltered;
            musclePositionController.OnUndo += musclePositionController_UndoRedoStateAltered;

            cSpineFlexExt = new CheckButton((Button)window.findWidget("CSpineFlexExt"));
            cSpineLateral = new CheckButton((Button)window.findWidget("CSpineLateral"));
            cSpineAxial = new CheckButton((Button)window.findWidget("CSpineAxial"));

            cSpineFlexExt.CheckedChanged += cSpineFlexExt_CheckedChanged;
            cSpineLateral.CheckedChanged += cSpineLateral_CheckedChanged;
            cSpineAxial.CheckedChanged += cSpineAxial_CheckedChanged;

            showPinControls = new CheckButton(window.findWidget("ShowPinControls") as Button);
            showPinControls.Checked = sceneControlManager.Visible;
            showPinControls.CheckedChanged += showPinControls_CheckedChanged;

            undoButton = window.findWidget("Undo") as Button;
            undoButton.MouseButtonClick += undoButton_MouseButtonClick;

            redoButton = window.findWidget("Redo") as Button;
            redoButton.MouseButtonClick += redoButton_MouseButtonClick;

            Button resetButton = (Button)window.findWidget("Reset");
            resetButton.MouseButtonClick += resetButton_MouseButtonClick;

            musclePositionController_UndoRedoStateAltered(musclePositionController);
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

        void cSpineAxial_CheckedChanged(Widget source, EventArgs e)
        {
            bool locked = cSpineAxial.Checked;
            foreach(var joint in cSpineJoints())
            {
                ((BEPUikTwistLimit)joint.getElement("AxialRotationLimit")).Locked = locked;
            }
        }

        void cSpineLateral_CheckedChanged(Widget source, EventArgs e)
        {
            bool locked = cSpineLateral.Checked;
            foreach (var joint in cSpineJoints())
            {
                ((BEPUikTwistLimit)joint.getElement("LateralBendingLimit")).Locked = locked;
            }
        }

        void cSpineFlexExt_CheckedChanged(Widget source, EventArgs e)
        {
            bool locked = cSpineFlexExt.Checked;
            foreach (var joint in cSpineJoints())
            {
                ((BEPUikTwistLimit)joint.getElement("FlexExtLimit")).Locked = locked;
            }
        }

        void showPinControls_CheckedChanged(Widget source, EventArgs e)
        {
            sceneControlManager.Visible = showPinControls.Checked;
        }

        private IEnumerable<SimObject> cSpineJoints()
        {
            var first = medicalController.getSimObject("C_T_SpineJoint");
            if (first != null)
            {
                yield return first;
                yield return medicalController.getSimObject("UpperT_LowerT_SpineJoint");
                yield return medicalController.getSimObject("LowerT_L_SpineJoint");
                yield return medicalController.getSimObject("L_Pelvis_Joint");
            }
            else
            {
                yield return medicalController.getSimObject("SkullSpineC1Joint");
                yield return medicalController.getSimObject("SpineC1_C2_Joint");
                yield return medicalController.getSimObject("SpineC2_C3_Joint");
                yield return medicalController.getSimObject("SpineC3_C4_Joint");
                yield return medicalController.getSimObject("SpineC4_C5_Joint");
                yield return medicalController.getSimObject("SpineC5_C6_Joint");
                yield return medicalController.getSimObject("SpineC6_C7_Joint");
                yield return medicalController.getSimObject("SpineC7_T1_Joint");
                yield return medicalController.getSimObject("SpineT1_T2_Joint");
                yield return medicalController.getSimObject("SpineT2_T3_Joint");
                yield return medicalController.getSimObject("SpineT3_T4_Joint");
                yield return medicalController.getSimObject("SpineT4_T5_Joint");
                yield return medicalController.getSimObject("SpineT5_T6_Joint");
                yield return medicalController.getSimObject("SpineT6_T7_Joint");
                yield return medicalController.getSimObject("SpineT7_T8_Joint");
                yield return medicalController.getSimObject("SpineT8_T9_Joint");
                yield return medicalController.getSimObject("SpineT9_T10_Joint");
                yield return medicalController.getSimObject("SpineT10_T11_Joint");
                yield return medicalController.getSimObject("SpineT11_T12_Joint");
                yield return medicalController.getSimObject("SpineT12_L1_Joint");
                yield return medicalController.getSimObject("SpineL1_L2_Joint");
                yield return medicalController.getSimObject("SpineL2_L3_Joint");
                yield return medicalController.getSimObject("SpineL3_L4_Joint");
                yield return medicalController.getSimObject("SpineL4_L5_Joint");
                yield return medicalController.getSimObject("SpineL5_PelvisJoint");
            }
        }

        void musclePositionController_UndoRedoStateAltered(MusclePositionController musclePositionController)
        {
            undoButton.Enabled = musclePositionController.HasUndo;
            redoButton.Enabled = musclePositionController.HasRedo;
        }
    }
}
