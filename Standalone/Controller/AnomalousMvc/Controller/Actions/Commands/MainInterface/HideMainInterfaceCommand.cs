using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class HideMainInterfaceCommand : ActionCommand
    {
        public HideMainInterfaceCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.hideMainInterface(ShowSharedGui);
        }

        [Editable]
        public bool ShowSharedGui { get; set; }

        public override string Type
        {
            get
            {
                return "Hide Main Interface";
            }
        }

        protected HideMainInterfaceCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
