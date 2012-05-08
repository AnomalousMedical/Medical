using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    class RestoreLayersCommand : ActionCommand
    {
        public RestoreLayersCommand()
        {
            Name = "Default";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.restoreLayers(Name);
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
                return "Restore Layers";
            }
        }

        protected RestoreLayersCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
