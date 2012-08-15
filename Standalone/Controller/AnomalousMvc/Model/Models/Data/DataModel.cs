using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Attributes;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public partial class DataModel : MvcModel, IDataProvider
    {
        public const String DefaultName = "DataModel";

        [DoNotSave]
        private Dictionary<String, DataModelItem> items = new Dictionary<String, DataModelItem>();

        public DataModel(String name = DefaultName)
            :base(name)
        {

        }

        public void addItem(DataModelItem item)
        {
            items.Add(item.Name, item);
            addItemDefinition(item);
        }

        public void removeItem(DataModelItem item)
        {
            items.Remove(item.Name);
            removeItemDefinition(item);
        }

        public bool hasItem(string name)
        {
            return items.ContainsKey(name);
        }

        public string getValue(string name)
        {
            if (hasItem(name))
            {
                return items[name].Value;
            }
            return null;
        }

        public void setValue(String name, String value)
        {
            if (hasItem(name))
            {
                items[name].Value = value;
            }
            else
            {
                DataModelItem item = new DataModelItem(name);
                item.Value = value;
                items.Add(name, item);
            }
        }
        
        protected DataModel(LoadInfo info)
            :base(info)
        {
            info.RebuildDictionary<String, DataModelItem>("Item", items);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractDictionary<String, DataModelItem>("Item", items);
        }

        public IEnumerable<Tuple<string, string>> Iterator
        {
            get
            {
                foreach (DataModelItem item in items.Values)
                {
                    yield return Tuple.Create(item.Name, item.Value);
                }
            }
        }
    }

    partial class DataModel
    {
        private EditInterfaceManager<DataModelItem> itemEdits;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Data", addData));

            itemEdits = new EditInterfaceManager<DataModelItem>(editInterface);
            itemEdits.addCommand(new EditInterfaceCommand("Remove", removeItem));

            foreach (DataModelItem link in items.Values)
            {
                addItemDefinition(link);
            }
        }

        private void addData(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name for this data item", delegate(String result, ref String errorMessage)
            {
                if (hasItem(result))
                {
                    errorMessage = String.Format("An item named {0} already exists. Please enter another.", result);
                    return false;
                }
                else
                {
                    addItem(new DataModelItem(result));
                    return true;
                }
            });
        }

        private void removeItem(EditUICallback callback, EditInterfaceCommand command)
        {
            DataModelItem item = itemEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeItem(item);
        }

        private void addItemDefinition(DataModelItem item)
        {
            if (itemEdits != null)
            {
                itemEdits.addSubInterface(item, item.getEditInterface());
            }
        }

        private void removeItemDefinition(DataModelItem item)
        {
            if (itemEdits != null)
            {
                itemEdits.removeSubInterface(item);
            }
        }
    }
}
