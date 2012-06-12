using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class ShowViewIfNotOpenCommand : ActionCommand
    {
        public ShowViewIfNotOpenCommand()
        {

        }

        public ShowViewIfNotOpenCommand(String view)
        {
            this.View = view;
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (!context.isViewOpen(View))
            {
                context.queueShowView(View);
            }
        }

        [EditableView]
        public String View { get; set; }

        public override string Type
        {
            get
            {
                return "Show View If Not Open";
            }
        }

        protected ShowViewIfNotOpenCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
