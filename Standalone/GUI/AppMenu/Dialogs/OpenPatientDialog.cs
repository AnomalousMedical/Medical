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
using System.Diagnostics;

namespace Medical.GUI
{
    public partial class OpenPatientDialog : Dialog
    {
        public event EventHandler OpenFile;

        private BackgroundWorker fileListWorker = new BackgroundWorker();
        private MultiList fileDataGrid;
        private Edit locationTextBox;
        private StaticImage warningImage;
        private StaticText warningText;
        private Progress loadingProgress;
        private Button openButton;
        private Button deleteButton;
        private Edit searchBox;

        private PatientDataFile currentFile = null;
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

        public OpenPatientDialog()
            : base("Medical.GUI.AppMenu.Dialogs.OpenPatientDialog.layout")
        {
            fileDataGrid = window.findWidget("Open/FileList") as MultiList;
            locationTextBox = window.findWidget("Open/LoadLocation") as Edit;
            warningImage = window.findWidget("Open/WarningImage") as StaticImage;
            warningText = window.findWidget("Open/WarningText") as StaticText;
            loadingProgress = window.findWidget("Open/LoadingProgress") as Progress;
            openButton = window.findWidget("Open/OpenButton") as Button;
            deleteButton = window.findWidget("Open/DeleteButton") as Button;
            Button cancelButton = window.findWidget("Open/CancelButton") as Button;
            searchBox = window.findWidget("Open/SearchText") as Edit;
            Button browseButton = window.findWidget("Open/BrowseButton") as Button;

            int fileGridWidth = fileDataGrid.Width - 2;
            fileDataGrid.addColumn("First Name", fileGridWidth / 3);
            fileDataGrid.addColumn("Last Name", fileGridWidth / 3);
            fileDataGrid.addColumn("Date Modified", fileGridWidth / 3);
            fileDataGrid.ListChangePosition += new MyGUIEvent(fileDataGrid_ListChangePosition);
            fileDataGrid.ListSelectAccept += new MyGUIEvent(fileDataGrid_ListSelectAccept);
            fileDataGrid.SortOnChanges = false;
            
            locationTextBox.Caption = MedicalConfig.SaveDirectory;
            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);

            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);

            warningImage.Visible = warningText.Visible = false;
            loadingProgress.Visible = false;
            loadingProgress.Range = 100;

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
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            uint index;
            if (fileDataGrid.findSubItemWith(1, searchBox.Caption, out index))
            {
                fileDataGrid.setIndexSelected(index);
            }
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
            if (FileChosen && OpenFile != null)
            {
                OpenFile.Invoke(this, EventArgs.Empty);
            }
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
            if (validSearchDirectory)
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();

                String[] files = Directory.GetFiles(locationTextBox.Caption, "*.pdt");
                int totalFiles = files.Length;

                int bufferSize = 200;// totalFiles / 3 + totalFiles % 3;
                int bufMax = bufferSize - 1;
                PatientDataFile[] dataFileBuffer = new PatientDataFile[bufferSize];
                int dataFileBufferPosition = 0;

                dataFileBufferPosition = 0;
                int currentPosition = 0;
                foreach (String file in files)
                {
                    if (fileListWorker.CancellationPending)
                    {
                        bgThreadKnowsAboutCancel = true;
                        ThreadManager.invokeAndWait(cancelListFilesCallback);
                        break;
                    }
                    PatientDataFile patient = new PatientDataFile(file);
                    if (patient.loadHeader())
                    {
                        currentPosition = dataFileBufferPosition++ % bufferSize;
                        dataFileBuffer[currentPosition] = patient;
                        if (currentPosition == bufMax)
                        {
                            ThreadManager.invokeAndWait(updateFileListCallback, dataFileBuffer, currentPosition);
                            fileListWorker.ReportProgress((int)(((float)dataFileBufferPosition / totalFiles) * 100.0f));
                        }
                    }
                }
                if (!bgThreadKnowsAboutCancel && files.Length > 0)
                {
                    ThreadManager.invokeAndWait(updateFileListCallback, dataFileBuffer, currentPosition);
                    fileListWorker.ReportProgress(0);
                }

                //sw.Stop();
                //Log.Debug("Scanned files in {0}", sw.ElapsedMilliseconds);
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
            loadingProgress.Position = (uint)e.ProgressPercentage;
        }

        void fileListWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingProgress.Visible = false;
            //Log.Debug("Total patients {0}.", fileDataGrid.getItemCount());
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

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.DirDialog dirDialog = new wx.DirDialog(MainWindow.Instance, "Choose the path to load files from.", locationTextBox.Caption))
            {
                if (dirDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    locationTextBox.Caption = dirDialog.Path;
                    listFiles();
                }
            }
        }
    }
}
