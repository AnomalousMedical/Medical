using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.Editor
{
    public class CreateEditableItemCommand<ItemType> : EditInterfaceCommand
    {
        public delegate ItemType CreateItem(String name);
        private CreateItem createItemCallback;

        public CreateEditableItemCommand(string name, EditInterfaceFunction function, CreateItem createItemCallback)
            :base(name, function)
        {
            this.createItemCallback = createItemCallback;
        }

        public ItemType createItem(String name)
        {
            return createItemCallback(name);
        }
    }
}
