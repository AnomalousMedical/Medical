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
    class NavigateToCommand : ActionCommand
    {
        public NavigateToCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (LinkName != null)
            {
                NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
                if (navModel != null)
                {
                    NavigationLink namedAction = navModel.getNamed(LinkName);
                    if (namedAction != null)
                    {
                        context.queueRunAction(namedAction.Action);
                    }
                    else
                    {
                        Log.Warning("Cannot find named navigation link '{0}' in navigation model. Has it been setup properly?", LinkName, NavigationModelName);
                    }
                }
                else
                {
                    Log.Warning("Cannot find navigation model '{0}'. Has it been setup properly?", NavigationModelName);
                }
            }
            else
            {
                Log.Warning("Link name is null. Cannot navigate.");
            }
        }

        [Editable]
        public String LinkName { get; set; }

        public override string Type
        {
            get
            {
                return "Navigate To";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateToCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
