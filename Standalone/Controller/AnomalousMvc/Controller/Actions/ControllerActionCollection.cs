using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    public class ControllerActionCollection : SaveableEditableItemCollection<ControllerAction>
    {
        public ControllerActionCollection()
        {

        }

        public override void customizeEditInterface(Engine.Editing.EditInterface editInterface, Engine.Editing.EditInterfaceManager<ControllerAction> itemEdits)
        {
            addItemCreation("Add Command Action", delegate(String name)
            {
                return new RunCommandsAction(name);
            });
        }

        protected ControllerActionCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
