using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    class SaveMedicalStateCommand : ActionCommand
    {
        public SaveMedicalStateCommand()
        {
            Name = "Default";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.saveMedicalState(Name);
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
                return "Save Medical State";
            }
        }

        protected SaveMedicalStateCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
