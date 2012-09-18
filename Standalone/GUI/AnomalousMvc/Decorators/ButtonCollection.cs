using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    public class ButtonCollection : SaveableEditableItemCollection<ButtonDefinitionBase>
    {
        public ButtonCollection()
        {

        }

        public override void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<ButtonDefinitionBase> itemEdits)
        {
            addItemCreation("Add Button", delegate(String name)
            {
                return new ButtonDefinition(name);
            });

            editInterface.addCommand(new EditInterfaceCommand("Add Close Button", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                if (!hasItem("Close"))
                {
                    add(new CloseButtonDefinition("Close"));
                }
            }));

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

            addItemMovementCommands();
        }

        protected ButtonCollection(LoadInfo info)
            :base(info)
        {

        }
    }
}
