using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class RestoreViewLayoutCommand : ActionCommand
    {
        public RestoreViewLayoutCommand(String storedViewsName = StoredViewCollection.DefaultName)
        {
            this.StoredViewsName = storedViewsName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.restoreViewLayout(StoredViewsName);
        }

        [Editable]
        public String StoredViewsName { get; set; }

        public override string Type
        {
            get
            {
                return "Restore View Layout";
            }
        }

        public override string Icon
        {
            get
            {
                return "StandaloneIcons/NoIcon";
            }
        }

        protected RestoreViewLayoutCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
