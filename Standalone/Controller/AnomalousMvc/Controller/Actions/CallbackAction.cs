using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// This class provides a controller action that can execute a delegate
    /// </summary>
    public class CallbackAction : ControllerAction
    {
        public delegate void ExecuteCallback(AnomalousMvcContext context);
        private ExecuteCallback executeCallback;

        public CallbackAction(String name, ExecuteCallback execute = null)
            :base(name)
        {
            this.executeCallback = execute;
        }

        public override void execute(AnomalousMvcContext context)
        {
            executeCallback.Invoke(context);
        }
    }
}
