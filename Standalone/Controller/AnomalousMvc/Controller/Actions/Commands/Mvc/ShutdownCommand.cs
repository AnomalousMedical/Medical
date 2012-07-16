using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class ShutdownCommand : ActionCommand
    {
        public ShutdownCommand()
        {
            
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueShutdown();
        }

        public override string Type
        {
            get
            {
                return "Shutdown";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MVCShutdownIcon";
            }
        }

        protected ShutdownCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
