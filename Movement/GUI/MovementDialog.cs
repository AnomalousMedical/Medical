using BEPUikPlugin;
using Engine.ObjectManagement;
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
        private MusclePosition restorePosition;
        private MusclePositionController musclePositionController;

        CheckButton cSpineFlexExt;
        CheckButton cSpineLateral;
        CheckButton cSpineAxial;

        Button restoreButton;

        public MovementDialog(MusclePositionController musclePositionController, MedicalController medicalController)
            : base("Medical.Movement.GUI.MovementDialog.layout")
        {
            this.medicalController = medicalController;
            this.musclePositionController = musclePositionController;

            cSpineFlexExt = new CheckButton((Button)window.findWidget("CSpineFlexExt"));
            cSpineLateral = new CheckButton((Button)window.findWidget("CSpineLateral"));
            cSpineAxial = new CheckButton((Button)window.findWidget("CSpineAxial"));

            cSpineFlexExt.CheckedChanged += cSpineFlexExt_CheckedChanged;
            cSpineLateral.CheckedChanged += cSpineLateral_CheckedChanged;
            cSpineAxial.CheckedChanged += cSpineAxial_CheckedChanged;

            Button resetButton = (Button)window.findWidget("Reset");
            resetButton.MouseButtonClick += resetButton_MouseButtonClick;

            restoreButton = (Button)window.findWidget("Restore");
            restoreButton.MouseButtonClick += restoreButton_MouseButtonClick;
        }

        private void resetButton_MouseButtonClick(object sender, EventArgs e)
        {
            restorePosition = new MusclePosition();
            restorePosition.captureState();

            musclePositionController.timedBlend(musclePositionController.BindPosition, MedicalConfig.CameraTransitionTime);

            //bothForwardBack.Value = rightForwardBack.Value;
            restoreButton.Enabled = true;
        }

        void restoreButton_MouseButtonClick(object sender, EventArgs e)
        {
            if (restorePosition != null)
            {
                musclePositionController.timedBlend(restorePosition, MedicalConfig.CameraTransitionTime);
                restorePosition = null;
            }
            restoreButton.Enabled = false;
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

        public IEnumerable<SimObject> cSpineJoints()
        {
            var first = medicalController.getSimObject("Skull_C_Joint");
            if (first != null)
            {
                yield return first;
                yield return medicalController.getSimObject("C_T_SpineJoint");
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
    }
}
