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
using Logging;
using System.Threading;

namespace Medical.GUI
{
    public partial class OpenPatientDialog : KryptonForm
    {
        private static char[] SEPS = { ',' };
        private PatientBindingSource patientData = new PatientBindingSource();

        private PatientDataFile currentFile = null;
        private PropertyDescriptor lastNameDescriptor;
        private bool validSearchDirectory = true;

        private delegate void UpdateCallback(PatientDataFile[] dataFileBuffer, int dataFileBufferPosition);
        private UpdateCallback updateFileListCallback;

        public OpenPatientDialog()
        {
            InitializeComponent();
            this.AllowFormChrome = !WindowsInfo.CompositionEnabled;
            fileDataGrid.SelectionChanged += new EventHandler(fileDataGrid_SelectionChanged);
            fileDataGrid.CellDoubleClick += new DataGridViewCellEventHandler(fileDataGrid_CellDoubleClick);
            fileDataGrid.CellClick += new DataGridViewCellEventHandler(fileDataGrid_CellClick);
            fileDataGrid.AutoGenerateColumns = false;
            fileDataGrid.DataSource = patientData;
            locationTextBox.Text = MedicalConfig.SaveDirectory;
            locationTextBox.TextChanged += new EventHandler(locationTextBox_TextChanged);
            warningLabel.Visible = false;
            loadingProgress.Visible = false;
            searchBox.TextChanged += new EventHandler(searchBox_TextChanged);

            lastNameDescriptor = TypeDescriptor.GetProperties(typeof(PatientBindingSource)).Find("LastName", false);
            fileListWorker.DoWork += new DoWorkEventHandler(fileListWorker_DoWork);
            fileListWorker.ProgressChanged += new ProgressChangedEventHandler(fileListWorker_ProgressChanged);
            fileListWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fileListWorker_RunWorkerCompleted);

            updateFileListCallback = new UpdateCallback(this.updateFileList);
        }

        void searchBox_TextChanged(object sender, EventArgs e)
        {
            int index = ((IBindingList)patientData).Find(lastNameDescriptor, searchBox.Text);
            if (index != -1)
            {
                fileDataGrid.Rows[index].Selected = true;
                fileDataGrid.FirstDisplayedScrollingRowIndex = index;
            }
        }

        void locationTextBox_TextChanged(object sender, EventArgs e)
        {
            validSearchDirectory = Directory.Exists(locationTextBox.Text);
            warningLabel.Visible = !validSearchDirectory;
            listFiles();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            listFiles();
            currentFile = null;
            openButton.Enabled = fileDataGrid.SelectedRows.Count > 0;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = fileListWorker.IsBusy;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            patientData.Clear();
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
            currentFile = null;
            this.Close();
        }

        void fileDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            openButton.Enabled = fileDataGrid.SelectedRows.Count > 0;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                locationTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void listFiles()
        {
            patientData.Clear();
            if (validSearchDirectory)
            {
                loadingProgress.Visible = true;
                fileListWorker.RunWorkerAsync();
            }
        }

        void fileListWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int bufferSize = 15;
            int bufMax = bufferSize - 1;
            PatientDataFile[] dataFileBuffer = new PatientDataFile[bufferSize];
            int dataFileBufferPosition = 0;
            int totalFiles = 0;
            if (validSearchDirectory)
            {
                dataFileBufferPosition = 0;
                String[] files = Directory.GetFiles(locationTextBox.Text, "*.pdt");
                totalFiles = files.Length;
                int currentPosition = 0;
                foreach (String file in files)
                {
                    PatientDataFile patient = new PatientDataFile(file);
                    if (patient.loadHeader())
                    {
                        currentPosition = dataFileBufferPosition++ % bufferSize;
                        dataFileBuffer[currentPosition] = patient;
                        if (currentPosition == bufMax)
                        {
                            this.Invoke(updateFileListCallback, dataFileBuffer, currentPosition);
                            fileListWorker.ReportProgress((int)(((float)dataFileBufferPosition / totalFiles) * 100.0f));
                        }
                    }
                }
                this.Invoke(updateFileListCallback, dataFileBuffer, currentPosition);
                fileListWorker.ReportProgress(0);
            }
        }

        void updateFileList(PatientDataFile[] dataFileBuffer, int dataFileBufferPosition)
        {
            for (int i = 0; i <= dataFileBufferPosition; ++i)
            {
                patientData.Add(dataFileBuffer[i]);
            }
        }

        void fileListWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loadingProgress.Step = e.ProgressPercentage - loadingProgress.Value;
            loadingProgress.PerformStep();
        }

        void fileListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingProgress.Visible = false;
            Log.Debug("Total patients {0}.", patientData.Count);
        }
    }
}
