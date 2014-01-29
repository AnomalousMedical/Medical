using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class ChangeMedicalStateCommand : ActionCommand
    {
        public enum CustomEditQueries
        {
            CapturePresetState
        }

        private PresetState presetState;

        public ChangeMedicalStateCommand()
        {
            presetState = new CompoundPresetState("", "", "");
            Duration = 1.0f;
        }

        public override void execute(AnomalousMvcContext context)
        {
 	        context.applyPresetState(presetState, Duration);
        }

        public void captureFromMedicalState(MedicalState medicalState)
        {
            CompoundPresetState compoundPresetState = new CompoundPresetState("", "", "");

            DiscPresetState leftDiscPreset = new DiscPresetState("LeftTMJDisc", "", "", "");
            leftDiscPreset.captureFromState(medicalState.Disc.getPosition("LeftTMJDisc"));
            compoundPresetState.addSubState(leftDiscPreset);

            DiscPresetState rightDiscPreset = new DiscPresetState("RightTMJDisc", "", "", "");
            rightDiscPreset.captureFromState(medicalState.Disc.getPosition("RightTMJDisc"));
            compoundPresetState.addSubState(rightDiscPreset);
            
            FossaPresetState leftFossaPreset = new FossaPresetState("", "", "");
            leftFossaPreset.captureFromState("LeftFossa", medicalState.Fossa);
            compoundPresetState.addSubState(leftFossaPreset);
            
            FossaPresetState rightFossaPreset = new FossaPresetState("", "", "");
            rightFossaPreset.captureFromState("RightFossa", medicalState.Fossa);
            compoundPresetState.addSubState(rightFossaPreset);
            
            AnimationManipulatorPresetState animationManipPresetState = new AnimationManipulatorPresetState("", "", "");
            animationManipPresetState.captureFromState(medicalState.BoneManipulator);
            compoundPresetState.addSubState(animationManipPresetState);
                
            TeethPresetState teethPreset = new TeethPresetState("", "", "");
            teethPreset.captureFromState(medicalState.Teeth);
            compoundPresetState.addSubState(teethPreset);

            presetState = compoundPresetState;
        }

        public PresetState PresetState
        {
            get
            {
                return presetState;
            }
            set
            {
                presetState = value;
            }
        }

        [Editable]
        public float Duration { get; set; }

        public override string Type
        {
            get
            {
                return "Change Medical State";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MedicalStateChangeIcon";
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Capture State", captureState));
        }

        private void captureState(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery<PresetState>(CustomEditQueries.CapturePresetState, presetStateResult);
        }

        private bool presetStateResult(PresetState result, ref string errorPrompt)
        {
            presetState = (PresetState)result;
            errorPrompt = "";
            return true;
        }

        protected ChangeMedicalStateCommand(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
