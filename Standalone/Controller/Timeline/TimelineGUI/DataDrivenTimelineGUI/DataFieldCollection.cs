using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical
{
    partial class DataFieldCollection : Saveable
    {
        private List<DataField> dataFields = new List<DataField>();

        public DataFieldCollection()
        {

        }

        public void addDataField(DataField field)
        {
            dataFields.Add(field);
            if (editInterface != null)
            {
                addDataFieldDefinition(field);
            }
        }

        public DataControl createControls(Widget parent)
        {
            FlowLayoutDataControl layoutControl = new FlowLayoutDataControl();
            layoutControl.SuppressLayout = true;
            foreach (DataField dataField in dataFields)
            {
                layoutControl.addControl(dataField.createControl(parent));
            }
            layoutControl.SuppressLayout = false;
            return layoutControl;
        }

        protected DataFieldCollection(LoadInfo info)
        {
            info.RebuildList<DataField>("DataFields", dataFields);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<DataField>("DataFields", dataFields);
        }
    }

    //EditInterface functions
    partial class DataFieldCollection
    {
        private EditInterface editInterface;
        private EditInterfaceManager<DataField> dataFieldEdits;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Data Fields");
                EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                editInterface.setPropertyInfo(propertyInfo);
                editInterface.addCommand(new EditInterfaceCommand("Add Menu Item", addMenuItem));
                editInterface.addCommand(new EditInterfaceCommand("Add Numeric Field", addNumericField));

                dataFieldEdits = new EditInterfaceManager<DataField>(editInterface);
                dataFieldEdits.addCommand(new EditInterfaceCommand("Remove", removeField));

                foreach (DataField field in dataFields)
                {
                    addDataFieldDefinition(field);
                }
            }
            return editInterface;
        }

        private void addMenuItem(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    MenuItemField dataField = new MenuItemField(input);
                    addDataField(dataField);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addNumericField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    NumericDataField numericDataField = new NumericDataField(input);
                    addDataField(numericDataField);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void removeField(EditUICallback callback, EditInterfaceCommand command)
        {

        }

        private void addDataFieldDefinition(DataField field)
        {
            dataFieldEdits.addSubInterface(field, field.getEditInterface());
        }

        private bool hasDataField(String name)
        {
            foreach (DataField field in dataFields)
            {
                if (field.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
