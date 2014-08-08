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

        public void addRange(ItemType[] items)
        {
            foreach (ItemType item in items)
            {
                add(item);
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

        public bool tryGetValue(String name, out ItemType value)
        {
            foreach (ItemType item in items)
            {
                if (item.Name == name)
                {
                    value = item;
                    return true;
                }
            }
            value = null;
            return false;
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

        public EditInterface getEditInterface(String editInterfaceName)
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface(editInterfaceName);
                EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                editInterface.setPropertyInfo(propertyInfo);

                var itemEdits = editInterface.createEditInterfaceManager<ItemType>();
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

        /// <summary>
        /// This is an optional method that can be called by subclasses if they
        /// want to support the items being movable in the list. It should be
        /// called from customizeEditInterface.
        /// </summary>
        protected void addItemMovementCommands()
        {
            editInterface.addCommand(new EditInterfaceCommand("Move Up", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                ItemType item = editInterface.resolveSourceObject<ItemType>(callback.getSelectedEditInterface());
                int index = items.IndexOf(item) - 1;
                if (index < 0)
                {
                    index = 0;
                }
                remove(item);
                insert(index, item);
            }));

            editInterface.addCommand(new EditInterfaceCommand("Move Down", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                ItemType item = editInterface.resolveSourceObject<ItemType>(callback.getSelectedEditInterface());
                int index = items.IndexOf(item) + 1;
                if (index < items.Count)
                {
                    remove(item);
                    insert(index, item);
                }
            }));
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
            ItemType item = editInterface.resolveSourceObject<ItemType>(callback.getSelectedEditInterface());
            remove(item);
        }

        private void renameAction(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a new name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasItem(input))
                {
                    //This works, but need to update the EditInterface
                    EditInterface selectedEditInterface = callback.getSelectedEditInterface();
                    ItemType item = editInterface.resolveSourceObject<ItemType>(selectedEditInterface);
                    item.Name = input;
                    selectedEditInterface.setName(String.Format("{0}", input));
                    return true;
                }
                errorPrompt = String.Format("An item named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addItemDefinition(ItemType item)
        {
            editInterface.addSubInterface(item, item.getEditInterface());
        }

        private void removeItemDefinition(ItemType item)
        {
            editInterface.removeSubInterface(item);
        }

        private void refreshItemDefinitions()
        {
            editInterface.getEditInterfaceManager<ItemType>().clearSubInterfaces();
            foreach (ItemType item in items)
            {
                item.getEditInterface().clearCommands();
                editInterface.addSubInterface(item, item.getEditInterface());
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
