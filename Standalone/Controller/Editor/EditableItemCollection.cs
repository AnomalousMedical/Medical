using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Editor
{
    public abstract partial class EditableItemCollection<ItemType> : IEnumerable<ItemType>
        where ItemType : EditableItem
    {
        protected List<ItemType> items = new List<ItemType>();

        public EditableItemCollection()
        {

        }

        public void add(ItemType item)
        {
            items.Add(item);
            if (editInterface != null)
            {
                addItemDefinition(item);
            }
        }

        public void remove(ItemType item)
        {
            items.Remove(item);
            if(editInterface != null)
            {
                removeItemDefinition(item);
            }
        }

        public void insert(int index, ItemType item)
        {
            items.Insert(index, item);
            if (editInterface != null)
            {
                refreshItemDefinitions();
            }
        }

        public ItemType this[String name]
        {
            get
            {
                foreach (ItemType item in items)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                throw new KeyNotFoundException("Cannot find an item named " + name);
            }
        }

        public IEnumerator<ItemType> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

    //EditInterface functions
    partial class EditableItemCollection<ItemType>
        where ItemType : EditableItem
    {
        [DoNotSave]
        private EditInterface editInterface;

        [DoNotSave]
        private EditInterfaceManager<ItemType> itemEdits;

        public EditInterface getEditInterface(String editInterfaceName)
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface(editInterfaceName);
                EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                editInterface.setPropertyInfo(propertyInfo);

                itemEdits = new EditInterfaceManager<ItemType>(editInterface);
                itemEdits.addCommand(new EditInterfaceCommand("Remove", deleteAction));
                itemEdits.addCommand(new EditInterfaceCommand("Rename", renameAction));

                customizeEditInterface(editInterface, itemEdits);

                foreach (ItemType item in items)
                {
                    addItemDefinition(item);
                }
            }
            return editInterface;
        }

        public virtual void customizeEditInterface(EditInterface editInterface, EditInterfaceManager<ItemType> itemEdits)
        {

        }

        public void addItemCreation(String commandText, CreateEditableItemCommand<ItemType>.CreateItem createCallback)
        {
            editInterface.addCommand(new CreateEditableItemCommand<ItemType>(commandText, createNewItem, createCallback));
        }

        private void createNewItem(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasItem(input))
                {
                    ItemType item = ((CreateEditableItemCommand<ItemType>)command).createItem(input);
                    add(item);
                    return true;
                }
                errorPrompt = String.Format("An item named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void deleteAction(EditUICallback callback, EditInterfaceCommand command)
        {
            ItemType item = itemEdits.resolveSourceObject(callback.getSelectedEditInterface());
            remove(item);
        }

        private void renameAction(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a new name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasItem(input))
                {
                    //This works, but need to update the EditInterface
                    EditInterface editInterface = callback.getSelectedEditInterface();
                    ItemType item = itemEdits.resolveSourceObject(editInterface);
                    item.Name = input;
                    editInterface.setName(String.Format("{0}", input));
                    return true;
                }
                errorPrompt = String.Format("An item named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addItemDefinition(ItemType item)
        {
            itemEdits.addSubInterface(item, item.getEditInterface());
        }

        private void removeItemDefinition(ItemType item)
        {
            itemEdits.removeSubInterface(item);
        }

        private void refreshItemDefinitions()
        {
            itemEdits.clearSubInterfaces();
            foreach (ItemType item in items)
            {
                item.getEditInterface().clearCommands();
                itemEdits.addSubInterface(item, item.getEditInterface());
            }
        }

        public bool hasItem(String name)
        {
            foreach (ItemType item in items)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }
    }
}
