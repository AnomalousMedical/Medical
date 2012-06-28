using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class SaveCameraPositionCommand : ActionCommand
    {
        public SaveCameraPositionCommand()
        {
            Name = "DefaultCamera";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                context.saveCamera(Name);
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
                return "Save Camera Position";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/CameraSavePositionIcon";
            }
        }

        protected SaveCameraPositionCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
