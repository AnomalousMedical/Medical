using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class ControllerAction : SaveableEditableItem
    {
        public ControllerAction(String name)
            :base(name)
        {

        }

        public abstract void execute(AnomalousMvcContext context);

        protected ControllerAction(LoadInfo info)
            : base(info)
        {

        }
    }
}
