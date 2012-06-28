using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class ControllerCollection : SaveableEditableItemCollection<MvcController>
    {
        public ControllerCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<MvcController> itemEdits)
        {
            editInterface.IconReferenceTag = "MvcContextEditor/ControllerIcon";
            addItemCreation("Add Controller", delegate(String name)
            {
                return new MvcController(name);
            });
        }

        protected ControllerCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
