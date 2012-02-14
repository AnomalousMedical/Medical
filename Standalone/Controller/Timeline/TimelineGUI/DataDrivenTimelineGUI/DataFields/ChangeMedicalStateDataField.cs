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
        private MedicalState medicalState;

        public ChangeMedicalStateDataField(String name)
            :base(name)
        {
            medicalState = new MedicalState(name);
            Duration = 1.0f;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        public MedicalState MedicalState
        {
            get
            {
                return medicalState;
            }
            set
            {
                medicalState = value;
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
            medicalState.update();
        }

        protected ChangeMedicalStateDataField(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
