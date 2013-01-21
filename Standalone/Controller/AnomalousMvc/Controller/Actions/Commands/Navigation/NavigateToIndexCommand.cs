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
    public class NavigateToIndexCommand : ActionCommand
    {
        public NavigateToIndexCommand()
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel navModel = context.getModel<NavigationModel>(NavigationModelName);
            if (navModel != null)
            {
                NavigationLink namedAction = navModel.getAt(Index);
                if (namedAction != null)
                {
                    context.queueRunAction(namedAction.Action);
                }
                else
                {
                    Log.Warning("Cannot find navigation link at index '{0}' in navigation model '{1}'. Has it been setup properly?", Index, NavigationModelName);
                }
            }
            else
            {
                Log.Warning("Cannot find navigation model '{0}'. Has it been setup properly?", NavigationModelName);
            }
        }

        [Editable]
        public int Index { get; set; }

        public override string Type
        {
            get
            {
                return "Navigate To Index";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/NavigateToIcon";
            }
        }

        [EditableModel(typeof(NavigationModel))]
        public String NavigationModelName { get; set; }

        protected NavigateToIndexCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
