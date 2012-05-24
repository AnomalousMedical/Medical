using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    class ShutdownCommand : ActionCommand
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

        protected ShutdownCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
