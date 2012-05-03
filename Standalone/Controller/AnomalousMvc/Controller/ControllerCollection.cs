using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class ControllerCollection : SaveableEditableItemCollection<Controller>
    {
        public ControllerCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<Controller> itemEdits)
        {
            addItemCreation("Add Controller", delegate(String name)
            {
                return new Controller(name);
            });
        }

        protected ControllerCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
