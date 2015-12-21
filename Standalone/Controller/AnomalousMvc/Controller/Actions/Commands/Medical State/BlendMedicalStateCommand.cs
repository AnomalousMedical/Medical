using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class BlendMedicalStateCommand : ActionCommand
    {
        public enum CustomEditQueries
        {
            CapturePresetState
        }

        private PresetState startState;
        private PresetState endState;

        public BlendMedicalStateCommand()
        {
            startState = new CompoundPresetState("", "", "");
            endState = new CompoundPresetState("", "", "");
        }

        public override void execute(AnomalousMvcContext context)
        {
            float blend;
            float.TryParse(context.getActionArgument("value"), out blend);
            context.blendPresetStates(startState, endState, blend);
        }

        public void captureStartFromMedicalState(MedicalState medicalState)
        {
            startState = cloneMedicalState(medicalState);
        }

        public void captureEndFromMedicalState(MedicalState medicalState)
        {
            endState = cloneMedicalState(medicalState);
        }

        public PresetState StartState
        {
            get
            {
                return startState;
            }
            set
            {
                startState = value;
            }
        }

        public override string Type
        {
            get
            {
                return "Blend Medical State";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Capture Start State", cb =>
            {
                cb.runCustomQuery<PresetState>(CustomEditQueries.CapturePresetState, startStateResult);
            }));

            editInterface.addCommand(new EditInterfaceCommand("Capture End State", cb =>
            {
                cb.runCustomQuery<PresetState>(CustomEditQueries.CapturePresetState, endStateResult);
            }));
        }

        private bool startStateResult(PresetState result, ref string errorPrompt)
        {
            startState = (PresetState)result;
            errorPrompt = "";
            return true;
        }

        private bool endStateResult(PresetState result, ref string errorPrompt)
        {
            endState = (PresetState)result;
            errorPrompt = "";
            return true;
        }

        protected BlendMedicalStateCommand(LoadInfo info)
            :base(info)
        {

        }

        private static CompoundPresetState cloneMedicalState(MedicalState medicalState)
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
            return compoundPresetState;
        }
    }
}
