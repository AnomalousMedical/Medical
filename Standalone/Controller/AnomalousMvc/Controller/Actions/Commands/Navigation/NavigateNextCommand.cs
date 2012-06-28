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
    class NavigateNextCommand : ActionCommand
    {
        public NavigateNextCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
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

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/NavigateNextIcon";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateNextCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
