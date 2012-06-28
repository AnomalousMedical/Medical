using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class RunActionCommand : ActionCommand
    {
        public RunActionCommand()
        {

        }

        public RunActionCommand(String action)
        {
            this.Action = action;
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueRunAction(Action);
        }

        [EditableAction]
        public String Action { get; set; }

        public override string Type
        {
            get
            {
                return "Run Action";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MVCRunACtionIcon";
            }
        }

        protected RunActionCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
