using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.Layers
{
    public partial class LayerSection : UserControl
    {
        private bool expanded = false;
        private DataGridViewTextBoxColumn textColumn;
        private DataGridViewNumericUpDownColumn alphaColumn;
        private DataGridViewTextBoxColumn hiddenColumn;
        private TransparencyGroup group;
        private DataGridViewNumericUpDownEditingControl currentEditCell = null;
        private EventHandler upDownValueChange;
        private int currentCellRow;

        public LayerSection(TransparencyGroup group)
        {
            InitializeComponent();

            upDownValueChange = new EventHandler(numUpDown_ValueChanged);

            this.group = group;
            Text = group.Name.ToString();

            groupTransparency.ValueChanged += new EventHandler(groupTransparency_ValueChanged);

            textColumn = new DataGridViewTextBoxColumn();
            textColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //textColumn.FillWeight = 0.5f;
            textColumn.ReadOnly = true;
            textColumn.Resizable = DataGridViewTriState.False;
            textColumn.HeaderText = "Object";
            alphaDataGrid.Columns.Add(textColumn);

            alphaColumn = new DataGridViewNumericUpDownColumn();
            alphaColumn.DecimalPlaces = 1;
            using (Graphics graphics = this.CreateGraphics())
            {
                alphaColumn.Width = alphaColumn.DefaultCellStyle.Padding.Horizontal + (int)graphics.MeasureString("Transparencyy", alphaDataGrid.Font).Width;
            }
            alphaColumn.Increment = 0.1m;
            alphaColumn.HeaderText = "Transparency";
            alphaColumn.Minimum = 0.0m;
            alphaColumn.Maximum = 1.0m;
            alphaColumn.Resizable = DataGridViewTriState.False;
            alphaColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            alphaDataGrid.Columns.Add(alphaColumn);

            hiddenColumn = new DataGridViewTextBoxColumn();
            hiddenColumn.Visible = false;
            alphaDataGrid.Columns.Add(hiddenColumn);
            alphaDataGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(alphaDataGrid_EditingControlShowing);
            alphaDataGrid.CellBeginEdit += new DataGridViewCellCancelEventHandler(alphaDataGrid_CellBeginEdit);

            foreach (TransparencyInterface transparency in group.getTransparencyObjectIter())
            {
                object[] values = { transparency.ObjectName, transparency.CurrentAlpha, transparency };
                alphaDataGrid.Rows.Add(values);
            }
        }

        void alphaDataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            currentCellRow = e.RowIndex;
        }

        void alphaDataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (currentEditCell != e.Control)
            {
                if (currentEditCell != null)
                {
                    currentEditCell.ValueChanged -= upDownValueChange;
                }
                currentEditCell = e.Control as DataGridViewNumericUpDownEditingControl;
                if (currentEditCell != null)
                {
                    currentEditCell.ValueChanged += upDownValueChange;
                }
            }
        }

        void groupTransparency_ValueChanged(object sender, EventArgs e)
        {
            if (alphaDataGrid.SelectedRows.Count > 0)
            {
                //set the value for all selected rows
                foreach (DataGridViewRow row in alphaDataGrid.SelectedRows)
                {
                    row.Cells[alphaColumn.Index].Value = groupTransparency.Value;
                    group.setAlphaValue(row.Cells[textColumn.Index].Value.ToString(), (float)groupTransparency.Value);
                }
            }
            else
            {
                //set the value for the entire group
                group.setAlphaValue((float)groupTransparency.Value);
                foreach (DataGridViewRow row in alphaDataGrid.Rows)
                {
                    row.Cells[alphaColumn.Index].Value = groupTransparency.Value;
                }
            }
        }

        void numUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown upDown = sender as NumericUpDown;
            if (upDown != null)
            {
                DataGridViewRow row = alphaDataGrid.Rows[currentCellRow];
                group.setAlphaValue(row.Cells[textColumn.Index].Value.ToString(), (float)upDown.Value);
            }
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            if (expanded)
            {
                expandButton.Text = "+";
                expanded = false;
                this.Height = this.PreferredSize.Height;
                alphaDataGrid.ClearSelection();
            }
            else
            {
                expandButton.Text = "-";
                expanded = true;
                this.Height = this.alphaDataGrid.PreferredSize.Height;
            }
        }

        public override string Text
        {
            get
            {
                return categoryLabel.Text;
            }
            set
            {
                categoryLabel.Text = value;
            }
        }
    }
}
