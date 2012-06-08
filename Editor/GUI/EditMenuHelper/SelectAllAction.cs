using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class SelectAllAction : ControllerAction
    {
        private String menuImplementorName;

        public SelectAllAction(String menuImplementorName = EditMenuHelper.MenuImplementorDefaultName, String name = "SelectAll")
            : base(name)
        {
            this.menuImplementorName = menuImplementorName;
        }

        public override void execute(AnomalousMvcContext context)
        {
            EditMenuImplementor menuImplementor = context.getModel<EditMenuImplementor>(menuImplementorName);
            menuImplementor.selectAll();
        }

        protected SelectAllAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
