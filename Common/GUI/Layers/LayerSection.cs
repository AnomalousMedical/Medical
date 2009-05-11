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
        DataGridViewTextBoxColumn textColumn;
        DataGridViewNumericUpDownColumn alphaColumn;


        public LayerSection()
        {
            InitializeComponent();

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

            DataGridViewTextBoxColumn hiddenColumn = new DataGridViewTextBoxColumn();
            hiddenColumn.Visible = false;
            alphaDataGrid.Columns.Add(hiddenColumn);
        }

        void groupTransparency_ValueChanged(object sender, EventArgs e)
        {
            if (alphaDataGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in alphaDataGrid.SelectedRows)
                {
                    row.Cells[alphaColumn.Index].Value = groupTransparency.Value;
                }
            }
            else
            {
                foreach (DataGridViewRow row in alphaDataGrid.Rows)
                {
                    row.Cells[alphaColumn.Index].Value = groupTransparency.Value;
                }
            }
        }

        public void addObject(String name)
        {
            object[] values = {name, 1.0};
            alphaDataGrid.Rows.Add(values);
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
