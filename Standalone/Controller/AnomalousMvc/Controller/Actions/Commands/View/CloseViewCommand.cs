using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class CloseViewCommand : ActionCommand
    {
        public CloseViewCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueCloseView();
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
