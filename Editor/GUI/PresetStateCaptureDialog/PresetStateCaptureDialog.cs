using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class PresetStateCaptureDialog : Dialog
    {
        private CheckButton leftDisc;
        private CheckButton rightDisc;
        private CheckButton leftFossa;
        private CheckButton rightFossa;
        private CheckButton mandible;
        private CheckButton teeth;

        private SendResult<Object> resultCallback;

        public PresetStateCaptureDialog(SendResult<Object> resultCallback)
            :base("Medical.GUI.PresetStateCaptureDialog.PresetStateCaptureDialog.layout")
        {
            this.resultCallback = resultCallback;

            leftDisc = new CheckButton((Button)window.findWidget("LeftDisc"));
            rightDisc = new CheckButton((Button)window.findWidget("RightDisc"));
            leftFossa = new CheckButton((Button)window.findWidget("LeftFossa"));
            rightFossa = new CheckButton((Button)window.findWidget("RightFossa"));
            mandible = new CheckButton((Button)window.findWidget("Mandible"));
            teeth = new CheckButton((Button)window.findWidget("Teeth"));

            Button captureButton = (Button)window.findWidget("Capture");
            captureButton.MouseButtonClick += new MyGUIEvent(captureButton_MouseButtonClick);
            
            Button cancelButton = (Button)window.findWidget("Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            this.Closed += new EventHandler(PresetStateCaptureDialog_Closed);
        }

        void captureButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MedicalState medicalState = new MedicalState("");
            medicalState.update();
            CompoundPresetState compoundPresetState = new CompoundPresetState("", "", "");

            if (leftDisc.Checked)
            {
                DiscPresetState leftDiscPreset = new DiscPresetState("LeftTMJDisc", "", "", "");
                leftDiscPreset.captureFromState(medicalState.Disc.getPosition("LeftTMJDisc"));
                compoundPresetState.addSubState(leftDiscPreset);
            }
            if (rightDisc.Checked)
            {
                DiscPresetState rightDiscPreset = new DiscPresetState("RightTMJDisc", "", "", "");
                rightDiscPreset.captureFromState(medicalState.Disc.getPosition("RightTMJDisc"));
                compoundPresetState.addSubState(rightDiscPreset);
            }
            if (leftFossa.Checked)
            {
                FossaPresetState leftFossaPreset = new FossaPresetState("", "", "");
                leftFossaPreset.captureFromState("LeftFossa", medicalState.Fossa);
                compoundPresetState.addSubState(leftFossaPreset);
            }
            if (rightFossa.Checked)
            {
                FossaPresetState rightFossaPreset = new FossaPresetState("", "", "");
                rightFossaPreset.captureFromState("RightFossa", medicalState.Fossa);
                compoundPresetState.addSubState(rightFossaPreset);
            }
            if (mandible.Checked)
            {
                AnimationManipulatorPresetState animationManipPresetState = new AnimationManipulatorPresetState("", "", "");
                animationManipPresetState.captureFromState(medicalState.BoneManipulator);
                compoundPresetState.addSubState(animationManipPresetState);
                //Need to implement mandible, may be just as easy to support each individual element
                /* bonescalarstateentry
                 * rightRamusHeightMandible
                 * rightAntegonialNotchMandible
                 * rightCondyleHeightMandible
                 * rightCondyleDegenerationMandible
                 * rightMandibularNotchMandible
                 * rightLateralPoleMandible
                 * rightMedialPoleScaleMandible
                 * 
                 * leftRamusHeightMandible
                 * leftCondyleHeightMandible
                 * leftAntegonialNotchMandible
                 * leftMandibularNotchMandible
                 * leftCondyleDegenerationMandible
                 * leftLateralPoleMandible
                 * leftMedialPoleScaleMandible
                 */
                /* bonerotator
                 * leftCondyleRotationMandible
                 * rightCondyleRotationMandible
                 */ 
                /* Pose Manipulator
                 * leftCondyleRoughnessMandible
                 * rightCondyleRoughnessMandible
                 */
            }
            if (teeth.Checked)
            {
                TeethPresetState teethPreset = new TeethPresetState("", "", "");
                teethPreset.captureFromState(medicalState.Teeth);
                compoundPresetState.addSubState(teethPreset);
            }

            String errorPrompt = null;
            resultCallback.Invoke(compoundPresetState, ref errorPrompt);
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void PresetStateCaptureDialog_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
