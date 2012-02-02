using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using MyGUIPlugin;

namespace Medical
{
    public partial class DataFieldCollection : Saveable
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

        public void removeDataField(DataField field)
        {
            dataFields.Remove(field);
            if(editInterface != null)
            {
                removeDataFieldDefinition(field);
            }
        }

        public void insertDataField(int index, DataField field)
        {
            dataFields.Insert(index, field);
            if (editInterface != null)
            {
                refreshDataFieldDefinitions();
            }
        }

        public void createControls(DataControlFactory factory)
        {
            factory.pushColumnLayout();
            foreach (DataField dataField in dataFields)
            {
                dataField.createControl(factory);
            }
            factory.popColumnLayout();
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
                editInterface.addCommand(new EditInterfaceCommand("Add Example Item", addExampleItem));
                editInterface.addCommand(new EditInterfaceCommand("Add Numeric Field", addNumericField));
                editInterface.addCommand(new EditInterfaceCommand("Add Boolean Field", addBooleanField));
                editInterface.addCommand(new EditInterfaceCommand("Add Multiple Choice Field", addMultipleChoiceField));
                editInterface.addCommand(new EditInterfaceCommand("Add Notes Field", addNotesField));
                editInterface.addCommand(new EditInterfaceCommand("Add Static Text Field", addStaticTextField));

                dataFieldEdits = new EditInterfaceManager<DataField>(editInterface);
                dataFieldEdits.addCommand(new EditInterfaceCommand("Remove", removeField));
                dataFieldEdits.addCommand(new EditInterfaceCommand("Rename", renameField));
                dataFieldEdits.addCommand(new EditInterfaceCommand("Move Up", moveUp));
                dataFieldEdits.addCommand(new EditInterfaceCommand("Move Down", moveDown));

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

        private void addExampleItem(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    PlayExampleDataField dataField = new PlayExampleDataField(input);
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

        private void addBooleanField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    BooleanDataField field = new BooleanDataField(input);
                    addDataField(field);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addMultipleChoiceField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    MultipleChoiceField field = new MultipleChoiceField(input);
                    addDataField(field);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addNotesField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    NotesDataField field = new NotesDataField(input);
                    addDataField(field);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void addStaticTextField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    StaticTextDataField field = new StaticTextDataField(input);
                    addDataField(field);
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void removeField(EditUICallback callback, EditInterfaceCommand command)
        {
            DataField field = dataFieldEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeDataField(field);
        }

        private void renameField(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a new name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasDataField(input))
                {
                    //This works, but need to update the EditInterface
                    EditInterface editInterface = callback.getSelectedEditInterface();
                    DataField field = dataFieldEdits.resolveSourceObject(editInterface);
                    field.Name = input;
                    editInterface.setName(String.Format("{0} - {1}", input, field.Type));
                    return true;
                }
                errorPrompt = String.Format("A Data Field named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void moveUp(EditUICallback callback, EditInterfaceCommand command)
        {
            DataField field = dataFieldEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = dataFields.IndexOf(field) - 1;
            if (index >= 0)
            {
                removeDataField(field);
                insertDataField(index, field);
            }
        }

        private void moveDown(EditUICallback callback, EditInterfaceCommand command)
        {
            DataField field = dataFieldEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = dataFields.IndexOf(field) + 1;
            if (index < dataFields.Count)
            {
                removeDataField(field);
                insertDataField(index, field);
            }
        }

        private void addDataFieldDefinition(DataField field)
        {
            dataFieldEdits.addSubInterface(field, field.getEditInterface());
        }

        private void removeDataFieldDefinition(DataField field)
        {
            dataFieldEdits.removeSubInterface(field);
        }

        private void refreshDataFieldDefinitions()
        {
            dataFieldEdits.clearSubInterfaces();
            foreach (DataField field in dataFields)
            {
                field.getEditInterface().clearCommands();
                dataFieldEdits.addSubInterface(field, field.getEditInterface());
            }
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