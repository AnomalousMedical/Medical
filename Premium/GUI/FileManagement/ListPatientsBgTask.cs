using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
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
    internal enum CancelPostAction
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

    class ListPatientsBgTask : CancelableBackgroundWorkTask<PatientFileBuffer>
    {
        private MultiListBox fileDataGrid;
        private ProgressBar loadingProgress;
        private TextBox locationTextBox;
        private OpenPatientDialog dialog;

        public ListPatientsBgTask(MultiListBox fileDataGrid, ProgressBar loadingProgress, TextBox locationTextBox, OpenPatientDialog dialog)
        {
            this.fileDataGrid = fileDataGrid;
            this.loadingProgress = loadingProgress;
            this.locationTextBox = locationTextBox;
            this.dialog = dialog;
        }

        public bool CanDoWork { get; set; }

        public IEnumerable<PatientFileBuffer> WorkUnits
        {
            get
            {
                String[] files = Directory.GetFiles(locationTextBox.Caption, "*.pdt");
                int totalFiles = files.Length;
                PatientFileBuffer buffer = new PatientFileBuffer();

                for (int i = 0; i < totalFiles; ++i)
                {
                    //Missing here is a check per file that we have not been canceled, it makes ending the bg task
                    //more responsive the code was the cancelation pending that the worker does do but since we are buffering
                    //multiple files we may want it here too
                    //if (fileListWorker.CancellationPending)

                    PatientDataFile patient = new PatientDataFile(files[i]);
                    if (patient.loadHeader())
                    {
                        if (buffer.addPatient(patient))
                        {
                            Progress = (int)(((float)i / totalFiles) * 100.0f);
                            yield return buffer;
                        }
                    }
                }
                if(buffer.HasResults) //Make sure any extra files are also returned.
                {
                    Progress = 0;
                    yield return buffer;
                }
            }
        }

        public int Progress { get; private set; }

        public void resetDisplay()
        {
            fileDataGrid.removeAllItems();
        }

        public void processingStarted()
        {
            loadingProgress.Visible = true;
        }

        public void workProcessed(PatientFileBuffer item)
        {
            foreach(PatientDataFile file in item.Files)
            {
                fileDataGrid.addItem(file.FirstName, file);
                uint newIndex = fileDataGrid.getItemCount() - 1;
                fileDataGrid.setSubItemNameAt(1, newIndex, file.LastName);
                fileDataGrid.setSubItemNameAt(2, newIndex, file.DateModified.ToString());
            }
            item.reset();
        }

        public void updateProgress(int progressPercentage)
        {
            loadingProgress.Position = (uint)progressPercentage;
        }

        public void runWorkCompleted()
        {
            loadingProgress.Visible = false;
        }

        public void startedWhileBusy()
        {
            CancelPostAction = CancelPostAction.ProcessNewDirectory;
        }

        public bool canceled()
        {
            bool restart = false;
            switch (CancelPostAction)
            {
                case CancelPostAction.Close:
                    dialog.hide();
                    break;
                case CancelPostAction.ProcessNewDirectory:
                    restart = true;
                    break;
            }
            this.CancelPostAction = CancelPostAction.None;
            return restart;
        }

        public CancelPostAction CancelPostAction { get; set; }
    }
}
