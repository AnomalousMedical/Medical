using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Medical.Model;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    class CreateMedicalStateCommand : ActionCommand
    {
        public CreateMedicalStateCommand()
        {
            WizardStateInfoName = MedicalStateInfoModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            MedicalStateInfoModel stateInfo = context.getModel<MedicalStateInfoModel>(WizardStateInfoName);
            if (stateInfo == null)
            {
                stateInfo = new MedicalStateInfoModel(WizardStateInfoName);
            }
            context.createMedicalState(stateInfo);
        }

        [EditableModel(typeof(MedicalStateInfoModel))]
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
