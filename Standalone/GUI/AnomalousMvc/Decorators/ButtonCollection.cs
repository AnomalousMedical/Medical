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

            editInterface.addCommand(new EditInterfaceCommand("Add Navigation Buttons", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                if (!hasItem("Cancel") &&
                    !hasItem("Previous") &&
                    !hasItem("Next") &&
                    !hasItem("Finish"))
                {
                    add(new ButtonDefinition("Cancel", "Common/Cancel"));
                    add(new ButtonDefinition("Previous", "Common/Previous"));
                    add(new ButtonDefinition("Next", "Common/Next"));
                    add(new ButtonDefinition("Finish", "Common/Finish"));
                }
            }));
        }

        protected ButtonCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
