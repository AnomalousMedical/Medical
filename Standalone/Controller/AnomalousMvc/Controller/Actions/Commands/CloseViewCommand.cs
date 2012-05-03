using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class CloseViewCommand : ActionCommand
    {
        public CloseViewCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueClose();
        }

        public override string Type
        {
            get
            {
                return "Close View";
            }
        }

        protected CloseViewCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
