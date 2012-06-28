using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class CloseAllViewsCommand : ActionCommand
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

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/ViewCloseAllIcon";
            }
        }

        protected CloseAllViewsCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
