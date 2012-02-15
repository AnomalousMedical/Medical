using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class ChangeMedicalStateDataField : DataField
    {
        public enum CustomEditQueries
        {
            CapturePresetState
        }

        private PresetState presetState;

        public ChangeMedicalStateDataField(String name)
            :base(name)
        {
            presetState = new CompoundPresetState(name, "", "");
            Duration = 1.0f;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
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

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Capture State", captureState));
        }

        private void captureState(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery(CustomEditQueries.CapturePresetState, presetStateResult);
        }

        private bool presetStateResult(Object result, ref string errorPrompt)
        {
            if (result is PresetState)
            {
                presetState = (PresetState)result;
                errorPrompt = "";
                return true;
            }
            else
            {
                errorPrompt = "The result is not a Preset State";
                return false;
            }
        }

        protected ChangeMedicalStateDataField(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
