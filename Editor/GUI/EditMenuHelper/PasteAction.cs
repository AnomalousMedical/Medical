using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class PasteAction : ControllerAction
    {
        private String menuImplementorName;

        public PasteAction(String menuImplementorName = EditMenuHelper.MenuImplementorDefaultName, String name = "Paste")
            : base(name)
        {
            this.menuImplementorName = menuImplementorName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            EditMenuImplementor menuImplementor = context.getModel<EditMenuImplementor>(menuImplementorName);
            menuImplementor.paste();
        }

        protected PasteAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
