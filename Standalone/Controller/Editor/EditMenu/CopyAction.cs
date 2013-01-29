using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class CopyAction : ControllerAction
    {
        public CopyAction(String name = "Copy", String editMenuManagerName = EditMenuManager.DefaultName)
            : base(name)
        {
            EditMenuManagerName = editMenuManagerName;
        }

        [Editable]
        public String EditMenuManagerName { get; set; }

        public override void execute(AnomalousMvcContext context)
        {
            EditMenuManager menuManager = context.getModel<EditMenuManager>(EditMenuManagerName);
            if (menuManager != null)
            {
                menuManager.copy();
            }
        }

        protected CopyAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
