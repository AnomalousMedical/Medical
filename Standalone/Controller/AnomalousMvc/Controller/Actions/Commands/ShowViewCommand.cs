using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class ShowViewCommand : ActionCommand
    {
        public ShowViewCommand()
        {

        }

        public override void execute(AnomalousMvcContext context)
        {
            context.showView(View);
        }

        [Editable]
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
