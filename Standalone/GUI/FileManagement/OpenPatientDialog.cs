using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Logging;
using System.Threading;
using MyGUIPlugin;
using Medical.Controller;
using System.Diagnostics;
using Engine.Platform;
using OgrePlugin;

namespace Medical.GUI
{
    public class OpenPatientDialog : AbstractFullscreenGUIPopup
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
        private Widget loadingWidget;
        private bool allowOpen = false;

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

        public OpenPatientDialog(GUIManager guiManager)
            : base("Medical.GUI.FileManagement.OpenPatientDialog.layout", guiManager)
        {
            fileDataGrid = widget.findWidget("Open/FileList") as MultiList;
            locationTextBox = widget.findWidget("Open/LoadLocation") as Edit;
            warningImage = widget.findWidget("Open/WarningImage") as StaticImage;
            warningText = widget.findWidget("Open/WarningText") as StaticText;
            loadingProgress = widget.findWidget("Open/LoadingProgress") as Progress;
            openButton = widget.findWidget("Open/OpenButton") as Button;
            deleteButton = widget.findWidget("Open/DeleteButton") as Button;
            Button cancelButton = widget.findWidget("Open/CancelButton") as Button;
            searchBox = widget.findWidget("Open/SearchText") as Edit;
            Button browseButton = widget.findWidget("Open/BrowseButton") as Button;

            int fileGridWidth = fileDataGrid.Width - 2;
            fileDataGrid.addColumn("First Name", 50);
            fileDataGrid.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            fileDataGrid.addColumn("Last Name", 50);
            fileDataGrid.setColumnResizingPolicyAt(1, ResizingPolicy.Fill);
            fileDataGrid.addColumn("Date Modified", 50);
            fileDataGrid.setColumnResizingPolicyAt(2, ResizingPolicy.Fill);
            fileDataGrid.ListChangePosition += new MyGUIEvent(fileDataGrid_ListChangePosition);
            fileDataGrid.ListSelectAccept += new MyGUIEvent(fileDataGrid_ListSelectAccept);
            fileDataGrid.SortOnChanges = false;
            
            String saveDirectory = MedicalConfig.SaveDirectory;
            if (!Directory.Exists(saveDirectory))
            {
                try
                {
                    Directory.CreateDirectory(saveDirectory);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not create save file directory at location {0}. Reason {1}", saveDirectory, ex.Message);
                }
            }
            locationTextBox.Caption = saveDirectory;

            locationTextBox.EventEditTextChange += new MyGUIEvent(locationTextBox_EventEditTextChange);

            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);
            searchBox.KeyButtonReleased += new MyGUIEvent(searchBox_KeyButtonReleased);

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

            loadingWidget = widget.findWidget("Loading");
            loadingWidget.Visible = false;

            this.Showing += new EventHandler(OpenPatientDialog_Showing);
            this.Shown += new EventHandler(OpenPatientDialog_Shown);
            this.Hiding += new EventHandler(OpenPatientDialog_Hiding);
            this.Hidden += new EventHandler(OpenPatientDialog_Hidden);
        }

        void searchBox_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            switch (ke.Key)
            {
                case KeyboardButtonCode.KC_RETURN:
                    if (fileDataGrid.hasItemSelected())
                    {
                        fireOpen();
                    }
                    break;
                //case KeyboardButtonCode.KC_DOWN:
                //    InputManager.Instance.resetKeyFocusWidget();
                //    InputManager.Instance.resetMouseFocusWidget();
                //    InputManager.Instance.resetMouseCaptureWidget();
                //    InputManager.Instance.setKeyFocusWidget(fileDataGrid);
                //    break;
            }
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            uint index;
            if (fileDataGrid.findSubItemWith(1, searchBox.Caption, out index))
            {
                fileDataGrid.setIndexSelected(index);
            }
            toggleButtonsEnabled();
        }

        void locationTextBox_EventEditTextChange(Widget source, EventArgs e)
        {
            validSearchDirectory = Directory.Exists(locationTextBox.Caption);
            warningImage.Visible = warningText.Visible = !validSearchDirectory;
            listFiles();
        }

        void OpenPatientDialog_Hidden(object sender, EventArgs e)
        {
            fileDataGrid.removeAllItems();
            searchBox.Caption = "";
        }

        void OpenPatientDialog_Hiding(object sender, EventArgs e)
        {
            if (fileListWorker.IsBusy && !bgThreadKnowsAboutCancel)
            {
                ((Engine.CancelEventArgs)e).Cancel = true;
                cancelPostAction = CancelPostAction.Close;
                fileListWorker.CancelAsync();
            }
        }

        void OpenPatientDialog_Showing(object sender, EventArgs e)
        {
            loadingWidget.Visible = false;
            allowOpen = true;
        }

        void OpenPatientDialog_Shown(object sender, EventArgs e)
        {
            listFiles();
            currentFile = null;
            toggleButtonsEnabled();
            cancelPostAction = CancelPostAction.None;
            InputManager.Instance.setKeyFocusWidget(searchBox);
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
            toggleButtonsEnabled();
        }

        private void toggleButtonsEnabled()
        {
            deleteButton.Enabled = openButton.Enabled = fileDataGrid.hasItemSelected();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentFile = null;
            this.hide();
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
            fireOpen();
        }

        private void fireOpen()
        {
            if (allowOpen && fileDataGrid.hasItemSelected())
            {
                allowOpen = false;
                loadingWidget.Visible = true;
                OgreInterface.Instance.OgrePrimaryWindow.OgreRenderWindow.update();
                currentFile = (PatientDataFile)fileDataGrid.getItemDataAt(fileDataGrid.getIndexSelected());
                if (OpenFile != null)
                {
                    OpenFile.Invoke(this, EventArgs.Empty);
                }
                this.hide();
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
                    this.hide();
                    break;
                case CancelPostAction.ProcessNewDirectory:
                    startNewDirectoryScanOnBackgroundThreadStop = true;
                    break;
            }
            cancelPostAction = CancelPostAction.None;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Choose the path to load files from.", locationTextBox.Caption))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    locationTextBox.Caption = dirDialog.Path;
                    listFiles();
                }
            }
        }
    }
}
