using Engine.Saving;
using Logging;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class GoBackAction : ActionCommand
    {
        public GoBackAction()
        {
            BackStackModelName = BackStackModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            BackStackModel backStack = context.getModel<BackStackModel>(BackStackModelName);
            if (backStack != null)
            {
                String backAction = backStack.getPreviousAction();
                if (backAction != null)
                {
                    backStack.ignoreNextCurrentAction();
                    context.queueRunAction(backAction);
                }
            }
            else
            {
                Log.Warning("Cannot find BackStackModel '{0}'. Has it been setup properly?", BackStackModelName);
            }
        }

        public override string Type
        {
            get
            {
                return "Go Back";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        [EditableModel(typeof(BackStackModel))]
        public String BackStackModelName { get; set; }

        protected GoBackAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
