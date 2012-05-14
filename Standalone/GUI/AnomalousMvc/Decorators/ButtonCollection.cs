using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    public class ButtonCollection : SaveableEditableItemCollection<ButtonDefinition>
    {
        public ButtonCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<ButtonDefinition> itemEdits)
        {
            addItemCreation("Add Button", delegate(String name)
            {
                return new ButtonDefinition(name);
            });
        }

        protected ButtonCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
