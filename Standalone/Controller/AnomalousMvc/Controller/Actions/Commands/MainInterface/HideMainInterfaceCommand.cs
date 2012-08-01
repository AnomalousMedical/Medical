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
            context.hideMainInterface();
        }

        public override string Type
        {
            get
            {
                return "Hide Main Interface";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MainInterfaceHideIcon";
            }
        }

        protected HideMainInterfaceCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
