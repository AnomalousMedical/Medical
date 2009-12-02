using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public partial class OpenPatientDialog : KryptonForm
    {
        private static char[] SEPS = { ',' };
        private PatientBindingSource patientData = new PatientBindingSource();

        private PatientDataFile currentFile = null;

        public OpenPatientDialog()
        {
            InitializeComponent();
            this.AllowFormChrome = !WindowsInfo.CompositionEnabled;
            fileDataGrid.SelectionChanged += new EventHandler(fileDataGrid_SelectionChanged);
            fileDataGrid.CellDoubleClick += new DataGridViewCellEventHandler(fileDataGrid_CellDoubleClick);
            fileDataGrid.CellClick += new DataGridViewCellEventHandler(fileDataGrid_CellClick);
            fileDataGrid.AutoGenerateColumns = false;
            fileDataGrid.DataSource = patientData;
        }

        public void listFiles(String directory)
        {
            patientData.Clear();
            foreach (String file in Directory.GetFiles(directory))
            {
                if (file.EndsWith(".pdt"))
                {
                    PatientDataFile patient = new PatientDataFile(file);
                    if (patient.loadHeader())
                    {
                        patientData.Add(patient);
                    }
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            currentFile = null;
            openButton.Enabled = fileDataGrid.SelectedRows.Count > 0;
        }

        void fileList_ItemActivate(object sender, EventArgs e)
        {
            openButton_Click(null, null);
        }

        public bool FileChosen
        {
            get
            {
                return currentFile != null;
            }
        }

        public PatientDataFile CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        void fileDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                int columnIndex = e.ColumnIndex;
                DataGridViewColumn column = fileDataGrid.Columns[columnIndex];
                SortOrder order = SortOrder.Ascending;
                ListSortDirection direction = ListSortDirection.Ascending;
                if (column.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    order = SortOrder.Descending;
                    direction = ListSortDirection.Descending;
                }
                fileDataGrid.Sort(column, direction);
                column.HeaderCell.SortGlyphDirection = order;
            }
        }

        void fileDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Make sure this isnt in the header.
            if (e.RowIndex != -1)
            {
                openButton_Click(null, null);
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (fileDataGrid.SelectedRows.Count > 0)
            {
                currentFile = fileDataGrid.SelectedRows[0].DataBoundItem as PatientDataFile;
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void fileDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            openButton.Enabled = fileDataGrid.SelectedRows.Count > 0;
        }
    }
}
