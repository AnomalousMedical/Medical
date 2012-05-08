using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    class RestoreCameraPositionCommand : ActionCommand
    {
        public RestoreCameraPositionCommand()
        {
            Name = "DefaultCamera";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.restoreCamera(Name);
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
                return "Restore Camera Position";
            }
        }

        protected RestoreCameraPositionCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
