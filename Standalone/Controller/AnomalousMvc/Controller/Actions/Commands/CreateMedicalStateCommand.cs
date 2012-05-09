using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Medical.Model;

namespace Medical.Controller.AnomalousMvc
{
    class CreateMedicalStateCommand : ActionCommand
    {
        public CreateMedicalStateCommand()
        {
            WizardStateInfoName = "DefaultWizardStateInfo";
        }

        public override void execute(AnomalousMvcContext context)
        {
            WizardStateInfo stateInfo = context.getModel<WizardStateInfo>(WizardStateInfoName);
            if (stateInfo == null)
            {
                stateInfo = new WizardStateInfo();
            }
            context.createMedicalState(stateInfo);
        }

        [Editable]
        public String WizardStateInfoName { get; set; }

        public override string Type
        {
            get
            {
                return "Create Medical State";
            }
        }

        protected CreateMedicalStateCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
