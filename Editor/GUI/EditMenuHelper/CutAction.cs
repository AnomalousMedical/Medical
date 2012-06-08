using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class CutAction : ControllerAction
    {
        private String menuImplementorName;

        public CutAction(String menuImplementorName = EditMenuHelper.MenuImplementorDefaultName, String name = "Cut")
            :base(name)
        {
            this.menuImplementorName = menuImplementorName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            EditMenuImplementor menuImplementor = context.getModel<EditMenuImplementor>(menuImplementorName);
            menuImplementor.cut();
        }

        protected CutAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
