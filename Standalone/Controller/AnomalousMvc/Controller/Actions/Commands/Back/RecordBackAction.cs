using Engine.Saving;
using Logging;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class RecordBackAction : ActionCommand
    {
        public RecordBackAction()
        {
            BackStackModelName = BackStackModel.DefaultName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            BackStackModel backStack = context.getModel<BackStackModel>(BackStackModelName);
            if (backStack != null)
            {
                backStack.setCurrentAction(context.ExecutingAction);
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
                return "Record Back";
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

        protected RecordBackAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
