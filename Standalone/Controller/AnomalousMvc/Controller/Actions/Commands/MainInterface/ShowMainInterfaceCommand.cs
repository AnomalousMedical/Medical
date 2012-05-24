using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class ShowMainInterfaceCommand : ActionCommand
    {
        public ShowMainInterfaceCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.showMainInterface();
        }

        public override string Type
        {
            get
            {
                return "Show Main Interface";
            }
        }

        protected ShowMainInterfaceCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
