using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using System.Threading;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class OpenPatientDialog : Dialog
    {
        private BackgroundWorker fileListWorker = new BackgroundWorker();
        private MultiList fileDataGrid;
        private Edit locationTextBox;
        private StaticImage warningImage;
        private StaticText warningText;
        private Widget loadingProgress;
        private Button openButton;
        private Button deleteButton;

        private PatientDataFile currentFile = null;
        //private PropertyDescriptor lastNameDescriptor;
        private bool validSearchDirectory = true;

        private delegate void UpdateCallback(PatientDataFile[] dataFileBuffer, int dataFileBufferPosition);
        private UpdateCallback updateFileListCallback;

        /**
         * The following variables work on the lifecycle that allows the
         * background worker to be canceled. This works by doing the sequences
         * described in the enum. The general sequence works as such.
         *   Some event happens on the UI thread that requires the background
         *   thread to shut down before it can continue. The UI thread looks to
         *   see if the background thread is running. If it is it will set the
         *   appropriate CancelPostAction and call the CancelAsync method on the
         *   background worker. The background worker will check each time it is
         *   scanning a new file to see if it is allowed to continue or is
         *   canceled. If it is canceled it will set the
         *   bgThreadKnowsAboutCancel variable to true and will not attempt to
         *   call anymore ui thread functions or sync its data. It will then
         *   call the cancelListFilesCallback on the UI thread. This will start
         *   the action originally requested on the UI thread. It will also
         *   break its loop and shutdown as fast as possible calling its
         *   RunCompleted method.
         */
        /// <summary>
        /// An enum of actions to take after the background worker is canceled.
        /// </summary>
        private enum CancelPostAction
        {
            /// <summary>
            /// Take no action on close.
            /// </summary>
            None,
            /// <summary>
            /// The listFiles function will check to see if the background
            /// thread is already running. If it is it will set the
            /// CancelPostAction to ProcessNewDirectory and then cancel the
            /// background worker. When the background worker signals that is
            /// knows about the cancel the
            /// startNewDirectoryScanOnBackgroundThreadStop variable will be set
            /// to true. Now when the RunWorkerCompleted method is called the
            /// listFiles() function will be called again from that method.
            /// </summary>
            ProcessNewDirectory,
            /// <summary>
            /// The form's onClosing event is handled. If the background thread
            /// is busy the close is canceled and the background thread's
            /// cancelAsync method is called. The CancelPostAction is set to
            /// Close and the BackgroundWorker cancelAsync method is called. In
            /// the CancelCallback the form will actually be closed and it will
            /// not be canceled by the OnClosing event because
            /// bgThreadKnowsAboutCancel will be true ensuring the background
            /// thread no longer calls any UI thread functions.
            /// </summary>
            Close,
        }

        private delegate void CancelCallback();
        private CancelCallback cancelListFilesCallback;
        private CancelPostAction cancelPostAction;
        private bool bgThreadKnowsAboutCancel = false;
        private bool startNewDirectoryScanOnBackgroundThreadStop = false;

        public OpenPatientDialog(String layoutFile)
            :base(layoutFile)
        {
            this.SmoothShow = false;

            fileDataGrid = window.findWidget("Open/FileList") as MultiList;
            locationTextBox = window.findWidget("Open/LoadLocation") as Edit;
            warningImage = window.findWidget("Open/WarningImage") as StaticImage;
            warningText = window.findWidget("Open/WarningText") as StaticText;
            loadingProgress = window.findWidget("Open/LoadingProgress");
            openButton = window.findWidget("Open/OpenButton") as Button;
            deleteButton = window.findWidget("Open/DeleteButton") as Button;
            Button cancelButton = window.findWidget("Open/CancelButton") as Button;

            fileDataGrid.addColumn("First Name", fileDataGrid.getWidth() / 3);
            fileDataGrid.addColumn("Last Name", fileDataGrid.getWidth() / 3);
            fileDataGrid.addColumn("Date Modified", fileDataGrid.getWidth() / 3);
            fileDataGrid.ListChangePosition += new MyGUIEvent(fileDataGrid_ListChangePosition);
            fileDataGrid.ListSelectAccept += new MyGUIEvent(fileDataGrid_ListSelectAccept);
            
            locationTextBox.Caption = MedicalConfig.SaveDirectory;
            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);

            //searchBox.TextChanged += new EventHandler(searchBox_TextChanged);

            warningImage.Visible = warningText.Visible = false;
            loadingProgress.Visible = false;

            //lastNameDescriptor = TypeDescriptor.GetProperties(typeof(PatientBindingSource)).Find("LastName", false);
            fileListWorker.WorkerReportsProgress = true;
            fileListWorker.WorkerSupportsCancellation = true;
            fileListWorker.DoWork += new DoWorkEventHandler(fileListWorker_DoWork);
            fileListWorker.ProgressChanged += new ProgressChangedEventHandler(fileListWorker_ProgressChanged);
            fileListWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fileListWorker_RunWorkerCompleted);

            updateFileListCallback = new UpdateCallback(this.updateFileList);
            cancelListFilesCallback = new CancelCallback(listFilesCanceled);

            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            deleteButton.MouseButtonClick += new MyGUIEvent(deleteButton_MouseButtonClick);
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        void searchBox_TextChanged(object sender, EventArgs e)
        {
            //int index = ((IBindingList)patientData).Find(lastNameDescriptor, searchBox.Text);
            //if (index != -1)
            //{
            //    fileDataGrid.Rows[index].Selected = true;
            //    fileDataGrid.FirstDisplayedScrollingRowIndex = index;
            //}
        }

        void locationTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            validSearchDirectory = Directory.Exists(locationTextBox.Caption);
            warningImage.Visible = warningText.Visible = !validSearchDirectory;
            listFiles();
        }

        protected override void onShown(EventArgs e)
        {
            base.onShown(e);
            listFiles();
            currentFile = null;
            openButton.Enabled = fileDataGrid.hasItemSelected();
            deleteButton.Enabled = fileDataGrid.hasItemSelected();
            cancelPostAction = CancelPostAction.None;
        }

        protected override void onClosing(CancelEventArgs e)
        {
            base.onClosing(e);
            if (fileListWorker.IsBusy && !bgThreadKnowsAboutCancel)
            {
                e.Cancel = true;
                cancelPostAction = CancelPostAction.Close;
                fileListWorker.CancelAsync();
            }
        }

        protected override void onClosed(EventArgs e)
        {
            base.onClosed(e);
            fileDataGrid.removeAllItems();
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

        void fileDataGrid_ListSelectAccept(Widget source, EventArgs e)
        {
            openButton_MouseButtonClick(null, null);
        }

        void fileDataGrid_ListChangePosition(Widget source, EventArgs e)
        {
            deleteButton.Enabled = openButton.Enabled = fileDataGrid.hasItemSelected();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentFile = null;
            this.close();
        }

        void deleteButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (fileDataGrid.hasItemSelected())
            {
                uint selectedIndex = fileDataGrid.getIndexSelected();
                PatientDataFile deleteFile = (PatientDataFile)fileDataGrid.getItemDataAt(selectedIndex);
                try
                {
                    File.Delete(deleteFile.BackingFile);
                    fileDataGrid.removeItemAt(selectedIndex);
                    fileDataGrid_ListChangePosition(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.show(String.Format("Could not delete file {0}.\nReason\n{1}.", deleteFile.BackingFile, ex.Message), "Delete Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError, null);
                }
            }
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (fileDataGrid.hasItemSelected())
            {
                currentFile = (PatientDataFile)fileDataGrid.getItemDataAt(fileDataGrid.getIndexSelected());
                this.close();
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            //if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            //{
            //    locationTextBox.Text = folderBrowserDialog.SelectedPath;
            //}
        }

        private void listFiles()
        {
            if (fileListWorker.IsBusy)
            {
                cancelPostAction = CancelPostAction.ProcessNewDirectory;
                fileListWorker.CancelAsync();
            }
            else
            {
                fileDataGrid.removeAllItems();
                if (validSearchDirectory)
                {
                    startNewDirectoryScanOnBackgroundThreadStop = false;
                    bgThreadKnowsAboutCancel = false;
                    loadingProgress.Visible = true;
                    fileListWorker.RunWorkerAsync();
                }
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
                String[] files = Directory.GetFiles(locationTextBox.Caption, "*.pdt");
                totalFiles = files.Length;
                int currentPosition = 0;
                foreach (String file in files)
                {
                    if (fileListWorker.CancellationPending)
                    {
                        bgThreadKnowsAboutCancel = true;
                        ThreadManager.invoke(cancelListFilesCallback);
                        break;
                    }
                    PatientDataFile patient = new PatientDataFile(file);
                    if (patient.loadHeader())
                    {
                        currentPosition = dataFileBufferPosition++ % bufferSize;
                        dataFileBuffer[currentPosition] = patient;
                        if (currentPosition == bufMax)
                        {
                            ThreadManager.invoke(updateFileListCallback, dataFileBuffer, currentPosition);
                            fileListWorker.ReportProgress((int)(((float)dataFileBufferPosition / totalFiles) * 100.0f));
                        }
                    }
                }
                if (!bgThreadKnowsAboutCancel && files.Length > 0)
                {
                    ThreadManager.invoke(updateFileListCallback, dataFileBuffer, currentPosition);
                    fileListWorker.ReportProgress(0);
                }
            }
        }

        void updateFileList(PatientDataFile[] dataFileBuffer, int dataFileBufferPosition)
        {
            for (int i = 0; i <= dataFileBufferPosition; ++i)
            {
                fileDataGrid.addItem(dataFileBuffer[i].FirstName, dataFileBuffer[i]);
                uint newIndex = fileDataGrid.getItemCount() - 1;
                fileDataGrid.setSubItemNameAt(1, newIndex, dataFileBuffer[i].LastName);
                fileDataGrid.setSubItemNameAt(2, newIndex, dataFileBuffer[i].DateModified.ToString());
            }
        }

        void fileListWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //loadingProgress.Step = e.ProgressPercentage - loadingProgress.Value;
            //loadingProgress.PerformStep();
        }

        void fileListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingProgress.Visible = false;
            //Log.Debug("Total patients {0}.", patientData.Count);
            if (startNewDirectoryScanOnBackgroundThreadStop)
            {
                listFiles();
            }
        }

        void listFilesCanceled()
        {
            switch (cancelPostAction)
            {
                case CancelPostAction.Close:
                    this.close();
                    break;
                case CancelPostAction.ProcessNewDirectory:
                    startNewDirectoryScanOnBackgroundThreadStop = true;
                    break;
            }
            cancelPostAction = CancelPostAction.None;
        }
    }
}
