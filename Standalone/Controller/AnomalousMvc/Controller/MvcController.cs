using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;

namespace Medical.Controller.AnomalousMvc
{
    public class MvcController : SaveableEditableItem
    {
        private ControllerActionCollection actionCollection;

        public MvcController(String name)
            :base(name)
        {
            actionCollection = new ControllerActionCollection();
        }

        public MvcController(String name, params ControllerAction[] actions)
            : this(name)
        {
            actionCollection.addRange(actions);
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
            EditInterface editInterface = actionCollection.getEditInterface(Name);
            editInterface.IconReferenceTag = "MvcContextEditor/ControllerIcon";
            return editInterface;
        }

        protected MvcController(LoadInfo info)
            :base(info)
        {

        }
    }
}
