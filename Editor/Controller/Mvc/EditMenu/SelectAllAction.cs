using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class SelectAllAction : ControllerAction
    {
        public SelectAllAction(String name = "SelectAll", String editMenuManagerName = EditMenuManager.DefaultName)
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
                menuManager.selectAll();
            }
        }

        protected SelectAllAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
