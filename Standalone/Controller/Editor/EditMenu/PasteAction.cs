using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class PasteAction : ControllerAction
    {
        public PasteAction(String name = "Paste", String editMenuManagerName = EditMenuManager.DefaultName)
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
                menuManager.paste();
            }
        }

        protected PasteAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
