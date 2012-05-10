using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Logging;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    class NavigatePreviousCommand : ActionCommand
    {
        public NavigatePreviousCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink previousAction = navModel.getPrevious();
                if (previousAction != null)
                {
                    context.queueRunAction(previousAction.Action);
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
                return "Navigate Previous";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigatePreviousCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
