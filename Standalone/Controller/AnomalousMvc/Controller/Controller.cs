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

        public override string Type
        {
            get
            {
                return "Controller";
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(actionCollection.getEditInterface());
        }

        protected Controller(LoadInfo info)
            :base(info)
        {

        }
    }
}
