using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class SaveViewLayoutCommand : ActionCommand
    {
        public SaveViewLayoutCommand(String storedViewsName = StoredViewCollection.DefaultName)
        {
            this.StoredViewsName = storedViewsName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.saveViewLayout(StoredViewsName);
        }

        [Editable]
        public String StoredViewsName { get; set; }

        public override string Type
        {
            get
            {
                return "Save View Layout";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/CameraSavePositionIcon";
            }
        }

        protected SaveViewLayoutCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
