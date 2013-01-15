using Engine.Saving;
using Logging;
using Medical.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public class CloseViewIfOpen : ActionCommand
    {
        public CloseViewIfOpen()
        {

        }

        public CloseViewIfOpen(String view)
        {
            this.View = view;
        }

        public override void execute(AnomalousMvcContext context)
        {
            ViewHost viewHost = context.findViewHost(View);
            if (viewHost != null)
            {
                context.queueCloseView(viewHost);
            }
        }

        [EditableView]
        public String View { get; set; }

        public override string Type
        {
            get
            {
                return "Close View If Open";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/ViewCloseIcon";
            }
        }

        protected CloseViewIfOpen(LoadInfo info)
            : base(info)
        {

        }
    }
}
