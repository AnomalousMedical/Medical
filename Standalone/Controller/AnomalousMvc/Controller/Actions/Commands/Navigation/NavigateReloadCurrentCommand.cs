using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    class NavigateReloadCurrentCommand : ActionCommand
    {
        public NavigateReloadCurrentCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink currentAction = navModel.getCurrent();
                if (currentAction != null)
                {
                    context.queueRunAction(currentAction.Action);
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
                return "Navigate Reload Current";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateReloadCurrentCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
