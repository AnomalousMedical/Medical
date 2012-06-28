using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class CallbackCommand : ActionCommand
    {
        private Action<AnomalousMvcContext> callback;

        public CallbackCommand(Action<AnomalousMvcContext> callback)
        {
            this.callback = callback;
        }

        public override void execute(AnomalousMvcContext context)
        {
            callback(context);
        }

        public override string Type
        {
            get
            {
                return "Callback";
            }
        }

        public override string Icon
        {
            get
            {
                return "NoIcon";
            }
        }

        protected CallbackCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
