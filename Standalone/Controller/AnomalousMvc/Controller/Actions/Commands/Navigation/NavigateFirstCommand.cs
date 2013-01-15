using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Logging;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class NavigateFirstCommand : ActionCommand
    {
        public NavigateFirstCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink namedAction = navModel.getFirst();
                if (namedAction != null)
                {
                    context.queueRunAction(namedAction.Action);
                }
                else
                {
                    Log.Warning("Cannot find a first element for navigation model '{0}', which means its empty. Has it been setup properly?", NavigationModelName);
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
                return "Navigate To First";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/NavigatePreviousIcon";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateFirstCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
