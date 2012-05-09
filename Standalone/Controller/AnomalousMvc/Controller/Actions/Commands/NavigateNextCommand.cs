using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    class NavigateNextCommand : ActionCommand
    {
        public NavigateNextCommand()
        {
            NavigationModelName = "DefaultNavigation";
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink nextAction = navModel.getNext();
                if (nextAction != null)
                {
                    context.queueRunAction(nextAction.Action);
                }
            }
            else
            {
                Log.Warning("Cannot find navigation model '{0}'. Has it been setup properly?", NavigationModelName);
            }
        }

        public override string Type
        {
            get
            {
                return "Navigate Next";
            }
        }

        [Editable]
        public String NavigationModelName { get; set; }

        protected NavigateNextCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
