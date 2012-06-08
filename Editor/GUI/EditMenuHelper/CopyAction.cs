using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class CopyAction : ControllerAction
    {
        private String menuImplementorName;

        public CopyAction(String menuImplementorName = EditMenuHelper.MenuImplementorDefaultName, String name = "Copy")
            : base(name)
        {
            this.menuImplementorName = menuImplementorName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            EditMenuImplementor menuImplementor = context.getModel<EditMenuImplementor>(menuImplementorName);
            menuImplementor.copy();
        }

        protected CopyAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
