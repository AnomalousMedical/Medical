using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class Controller : SaveableEditableItem
    {
        private ControllerActionCollection actionCollection;

        public Controller(String name)
            :base(name)
        {
            actionCollection = new ControllerActionCollection();
        }

        public void runAction(String actionName, AnomalousMvcContext context)
        {
            ControllerAction action = actionCollection[actionName];
            action.execute(context);
        }

        public ControllerActionCollection Actions
        {
            get
            {
                return actionCollection;
            }
        }

        protected override EditInterface createEditInterface()
        {
            return actionCollection.getEditInterface(Name);
        }

        protected Controller(LoadInfo info)
            :base(info)
        {

        }
    }
}
