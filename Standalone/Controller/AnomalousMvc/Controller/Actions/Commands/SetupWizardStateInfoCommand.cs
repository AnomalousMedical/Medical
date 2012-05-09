using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Model;
using Engine.Reflection;

namespace Medical.Controller.AnomalousMvc
{
    class SetupWizardStateInfoCommand : ActionCommand
    {
        public SetupWizardStateInfoCommand()
        {
            WizardStateInfo = new WizardStateInfo();
            WizardStateInfoName = "DefaultWizardStateInfo";
        }

        public override void execute(AnomalousMvcContext context)
        {
            WizardStateInfo.ProcedureDate = DateTime.Now;
            context.addModel(WizardStateInfoName, WizardStateInfo);
        }

        protected override void createEditInterface()
        {
            editInterface = ReflectedEditInterface.createEditInterface(WizardStateInfo, ReflectedEditInterface.DefaultScanner, Type, null);
            editInterface.addEditableProperty(new ReflectedEditableProperty("WizardStateInfoName", ReflectedVariable.createVariable(new PropertyMemberWrapper(GetType().GetProperty("WizardStateInfoName")), this)));
        }

        public String WizardStateInfoName { get; set; }

        public WizardStateInfo WizardStateInfo { get; set; }

        public override string Type
        {
            get
            {
                return "Setup Wizard State Info";
            }
        }

        protected SetupWizardStateInfoCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
