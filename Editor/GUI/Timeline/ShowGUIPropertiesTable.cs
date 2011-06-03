using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class ShowGUIPropertiesTable
    {
        private Table propertiesTable;
        private EditablePropertyInfo currentPropInfo;
        private Dictionary<TableRow, EditableProperty> rowProperties = new Dictionary<TableRow, EditableProperty>();
        private bool allowValidation = true;

        public ShowGUIPropertiesTable(Table propertiesTable)
        {
            this.propertiesTable = propertiesTable;
            propertiesTable.CellValidating += new EventHandler<TableCellValidationEventArgs>(propertiesTable_CellValidating);
            propertiesTable.CellValueChanged += new EventHandler(propertiesTable_CellValueChanged);
        }

        public void clear()
        {
            rowProperties.Clear();
            propertiesTable.Columns.clear();
            propertiesTable.Rows.clear();
        }

        public void setEditInterface(EditInterface editInterface)
        {
            if (editInterface != null)
            {
                currentPropInfo = editInterface.getPropertyInfo();
                if (currentPropInfo != null)
                {
                    foreach (EditablePropertyColumn column in currentPropInfo.getColumns())
                    {
                        TableColumn dgvColumn = new TableColumn(column.Header);
                        dgvColumn.ReadOnly = column.ReadOnly;
                        propertiesTable.Columns.add(dgvColumn);
                    }
                }
                if (editInterface.hasEditableProperties())
                {
                    foreach (EditableProperty editProp in editInterface.getEditableProperties())
                    {
                        addProperty(editProp);
                    }
                }
                propertiesTable.layout();
            }
            else
            {
                currentPropInfo = null;
            }
        }

        void propertiesTable_CellValueChanged(object sender, EventArgs e)
        {
            TableCell cell = (TableCell)sender;
            EditableProperty var = rowProperties[cell.Row];
            if (var != null)
            {
                int rowIndex = cell.RowIndex;
                int columnIndex = cell.ColumnIndex;
                var.setValueStr(columnIndex, cell.Value.ToString());
                //changesMade = true;
                //if (EditablePropertyValueChanged != null)
                //{
                //    EditablePropertyValueChanged.Invoke(var);
                //}
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
                TableCell newCell = createCell(property.getPropertyType(i));
                newCell.Value = property.getValue(i);
                newRow.Cells.add(newCell);
            }

            propertiesTable.Rows.add(newRow);
            rowProperties.Add(newRow, property);
        }

        private TableCell createCell(Type propType)
        {
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
            //if (propType == typeof(bool))
            //{
            //    return new DataGridViewCheckBoxCell();
            //}
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
    }
}
