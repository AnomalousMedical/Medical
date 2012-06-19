using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Logging;

namespace Medical.GUI
{
    public class RunMvcContextActionTask : Task
    {
        public String actionName;

        public RunMvcContextActionTask(String uniqueName, String name, String iconName, String category, String actionName, AnomalousMvcContext context)
            :base(uniqueName, name, iconName, category)
        {
            this.actionName = actionName;
            this.Context = context;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            if (Context != null)
            {
                Context.runAction(actionName);
            }
            else
            {
                Log.Warning("Context was null cannot run action '{0}'", actionName);
            }
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        public AnomalousMvcContext Context { get; set; }
    }
}
