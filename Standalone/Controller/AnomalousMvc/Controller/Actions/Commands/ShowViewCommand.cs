using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    class ShowViewCommand : ActionCommand
    {
        public ShowViewCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.queueShowView(View);
        }

        [EditableView]
        public String View { get; set; }

        public override string Type
        {
            get
            {
                return "Show View";
            }
        }

        protected ShowViewCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
