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
    class NavigateLastCommand : ActionCommand
    {
        public NavigateLastCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink namedAction = navModel.getLast();
                if (namedAction != null)
                {
                    context.queueRunAction(namedAction.Action);
                }
                else
                {
                    Log.Warning("Cannot find a last element for navigation model '{0}', which means its empty. Has it been setup properly?", NavigationModelName);
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
                return "MvcContextEditor/NavigateNextIcon";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateLastCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
