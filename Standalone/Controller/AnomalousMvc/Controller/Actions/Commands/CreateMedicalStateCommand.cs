using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    class CreateMedicalStateCommand : ActionCommand
    {
        public CreateMedicalStateCommand()
        {
            
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.createMedicalState();
        }

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
