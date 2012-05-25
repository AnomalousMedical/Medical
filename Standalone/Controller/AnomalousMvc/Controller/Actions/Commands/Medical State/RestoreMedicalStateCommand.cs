using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class RestoreMedicalStateCommand : ActionCommand
    {
        public RestoreMedicalStateCommand()
        {
            Name = "DefaultMedicalState";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.restoreMedicalState(Name);
            }
            else
            {
                Log.Warning("No name defined.");
            }
        }

        [Editable]
        public String Name { get; set; }

        public override string Type
        {
            get
            {
                return "Restore Medical State";
            }
        }

        protected RestoreMedicalStateCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
