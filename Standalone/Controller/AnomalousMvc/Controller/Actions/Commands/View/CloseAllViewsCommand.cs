using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class CloseAllViewsCommand : ActionCommand
    {
        public CloseAllViewsCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueCloseAllViews();
        }

        public override string Type
        {
            get
            {
                return "Close All Views";
            }
        }

        protected CloseAllViewsCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
