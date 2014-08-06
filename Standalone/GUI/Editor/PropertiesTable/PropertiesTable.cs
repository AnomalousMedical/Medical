using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    public delegate void EditablePropertyValueChanged(EditableProperty var);

    /// <summary>
    /// This will create a MyGUI Table that interfaces with an EditInterface.
    /// </summary>
    public sealed class PropertiesTable : PropertyEditor, IDisposable
    {
        public event EditablePropertyValueChanged EditablePropertyValueChanged;

        private Table propertiesTable;
        private EditablePropertyInfo currentPropInfo;
        private EditInterface currentEditInterface = null;
        private Dictionary<TableRow, EditableProperty> rowProperties = new Dictionary<TableRow, EditableProperty>();
        private bool allowValidation = true;
        private List<ICustomCellProvider> customCells = new List<ICustomCellProvider>();
        private AddRemoveButtons addRemoveButtons = null;
        private EditUICallback uiCallback = null;

        public PropertiesTable(Table propertiesTable)
            :this(propertiesTable, null, null)
        {
            
        }

        public PropertiesTable(Table propertiesTable, EditUICallback uiCallback)
            : this(propertiesTable, uiCallback, null)
        {

        }

        public PropertiesTable(Table propertiesTable, EditUICallback uiCallback, AddRemoveButtons buttons)
        {
            this.uiCallback = uiCallback;
            this.propertiesTable = propertiesTable;
            propertiesTable.CellValidating += new EventHandler<TableCellValidationEventArgs>(propertiesTable_CellValidating);
            propertiesTable.CellValueChanged += new EventHandler(propertiesTable_CellValueChanged);
            addRemoveButtons = buttons;
            if (addRemoveButtons != null)
            {
                addRemoveButtons.AddButtonClicked += addRemoveButtons_AddButtonClicked;
                addRemoveButtons.RemoveButtonClicked += addRemoveButtons_RemoveButtonClicked;
            }
        }

        public void Dispose()
        {
            if (addRemoveButtons != null)
            {
                addRemoveButtons.AddButtonClicked -= addRemoveButtons_AddButtonClicked;
                addRemoveButtons.RemoveButtonClicked -= addRemoveButtons_RemoveButtonClicked;
            }
            clear();
        }

        public void clear()
        {
            rowProperties.Clear();
            propertiesTable.Columns.clear();
            propertiesTable.Rows.clear();
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditInterface;
            }
            set
            {
                allowValidation = false;
                if (currentEditInterface != null)
                {
                    currentEditInterface.OnPropertyAdded -= new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved -= new PropertyRemoved(currentEditInterface_OnPropertyRemoved);
                }
                clear();
                if (value != null)
                {
                    currentEditInterface = value;
                    currentEditInterface.OnPropertyAdded += new PropertyAdded(currentEditInterface_OnPropertyAdded);
                    currentEditInterface.OnPropertyRemoved += new PropertyRemoved(currentEditInterface_OnPropertyRemoved);

                    if (addRemoveButtons != null)
                    {
                        addRemoveButtons.Visible = currentEditInterface.canAddRemoveProperties();
                    }
                    currentPropInfo = value.getPropertyInfo();
                    if (currentPropInfo != null)
                    {
                        foreach (EditablePropertyColumn column in currentPropInfo.getColumns())
                        {
                            TableColumn dgvColumn = new TableColumn(column.Header);
                            dgvColumn.ReadOnly = column.ReadOnly;
                            propertiesTable.Columns.add(dgvColumn);
                        }
                    }
                    if (value.hasEditableProperties())
                    {
                        foreach (EditableProperty editProp in value.getEditableProperties())
                        {
                            addProperty(editProp);
                        }
                    }
                    propertiesTable.layout();
                }
                else
                {
                    currentPropInfo = null;
                    currentEditInterface = null;
                    if (addRemoveButtons != null)
                    {
                        addRemoveButtons.Visible = false;
                    }
                }
                allowValidation = true;
            }
        }

        public void addCustomCellProvider(ICustomCellProvider customCellProvider)
        {
            customCells.Add(customCellProvider);
        }

        public void removeCustomCellProvider(ICustomCellProvider customCellProvider)
        {
            customCells.Remove(customCellProvider);
        }

        void currentEditInterface_OnPropertyRemoved(EditableProperty property)
        {
            removeProperty(property);
            propertiesTable.layout();
        }

        void currentEditInterface_OnPropertyAdded(EditableProperty property)
        {
            addProperty(property);
            propertiesTable.layout();
        }

        void propertiesTable_CellValueChanged(object sender, EventArgs e)
        {
            TableCell cell = (TableCell)sender;
            EditableProperty var = rowProperties[cell.Row];
            if (var != null)
            {
                int rowIndex = cell.RowIndex;
                int columnIndex = cell.ColumnIndex;
                var.setValueStr(columnIndex, cell.Value != null ? cell.Value.ToString() : null);
                //changesMade = true;
                if (EditablePropertyValueChanged != null)
                {
                    EditablePropertyValueChanged.Invoke(var);
                }
                updateData();
            }
        }

        void propertiesTable_CellValidating(object sender, TableCellValidationEventArgs e)
        {
            TableCell cell = (TableCell)sender;
            if (!currentPropInfo.getColumn(cell.ColumnIndex).ReadOnly)
            {
                e.Cancel = !validateEditCell(cell.RowIndex, cell.ColumnIndex, e.EditValue.ToString());
            }
        }

        private void updateData()
        {
            foreach (TableRow row in propertiesTable.Rows)
            {
                EditableProperty var = rowProperties[row];
                if (var != null)
                {
                    for (int i = 0; i < currentPropInfo.getNumColumns(); i++)
                    {
                        row.Cells[i].Value = var.getValue(i);
                    }
                }
            }
        }

        /// <summary>
        /// Add an EditableProperty to this table.
        /// </summary>
        /// <param name="property">The property to add.</param>
        private void addProperty(EditableProperty property)
        {
            TableRow newRow = new TableRow();

            for (int i = 0; i < currentPropInfo.getNumColumns(); i++)
            {
                TableCell newCell = createCell(property.getPropertyType(i), property.hasBrowser(i), property);
                newCell.Value = property.getValue(i);
                newRow.Cells.add(newCell);
            }

            propertiesTable.Rows.add(newRow);
            rowProperties.Add(newRow, property);
        }

        private void removeProperty(EditableProperty property)
        {
            foreach (TableRow row in propertiesTable.Rows)
            {
                if (rowProperties[row] == property)
                {
                    propertiesTable.Rows.remove(row);
                    rowProperties.Remove(row);
                    row.Dispose();
                    break;
                }
            }
        }

        private TableCell createCell(Type propType, bool hasBrowser, EditableProperty property)
        {
            foreach (ICustomCellProvider customCellProvider in customCells)
            {
                TableCell cell = customCellProvider.createCell(propType, hasBrowser, property);
                if (cell != null)
                {
                    return cell;
                }
            }
            //if (propType.IsEnum)
            //{
            //    if (propType.GetCustomAttributes(typeof(SingleEnumAttribute), true).Length > 0)
            //    {
            //        SingleEnumEditorCell editorCell = new SingleEnumEditorCell();
            //        editorCell.populateCombo(propType);
            //        return editorCell;
            //    }
            //    else if (propType.GetCustomAttributes(typeof(MultiEnumAttribute), true).Length > 0)
            //    {
            //        MultiEnumEditorCell editorCell = new MultiEnumEditorCell();
            //        editorCell.EnumType = propType;
            //        return editorCell;
            //    }
            //    else
            //    {
            //        DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            //        return cell;
            //    }
            //}
            if (propType == typeof(bool))
            {
                return new CheckTableCell();
            }
            if (propType == typeof(Color))
            {
                return new ColorEditCell();
            }
            if (hasBrowser)
            {
                return new EditTableBrowserCell(uiCallback, property);
            }
            return new EditTableCell();
        }

        /// <summary>
        /// Validate the edit cell in a row.
        /// </summary>
        /// <param name="rowIndex">The row index to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the cell was valid false if not.</returns>
        bool validateEditCell(int rowIndex, int colIndex, String value)
        {
            bool valid = true;
            if (allowValidation)
            {
                TableRow row = propertiesTable.Rows[rowIndex];
                EditableProperty var = rowProperties[row];
                if (var != null)
                {
                    String errorText;
                    if (!var.canParseString(colIndex, value, out errorText))
                    {
                        valid = false;
                        //propGridView.Rows[rowIndex].ErrorText = errorText;
                    }
                    else
                    {
                        //propGridView.Rows[rowIndex].ErrorText = "";
                    }
                }
            }
            return valid;
        }

        void addRemoveButtons_AddButtonClicked(Widget source, EventArgs e)
        {
            currentEditInterface.fireAddPropertyCallback(uiCallback);
        }

        void addRemoveButtons_RemoveButtonClicked(Widget source, EventArgs e)
        {
            int lastEditedRow = propertiesTable.LastEditedRow;
            if (lastEditedRow != -1)
            {
                TableRow row = propertiesTable.Rows[propertiesTable.LastEditedRow];
                EditableProperty var = rowProperties[row];
                currentEditInterface.fireRemovePropertyCallback(uiCallback, var);
            }
        }
    }
}
