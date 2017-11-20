using Engine;
using Engine.Threads;
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

    class ListPatientsBgTask : CancelableBackgroundWorkTask<ObjectBuffer<PatientDataFile>>
    {
        private OpenPatientDialog dialog;
        private CancelableBackgroundWorker<ObjectBuffer<PatientDataFile>> bgWorker;
        private String currentDirectory;

        public ListPatientsBgTask(OpenPatientDialog dialog)
        {
            this.dialog = dialog;
            bgWorker = new CancelableBackgroundWorker<ObjectBuffer<PatientDataFile>>(this);
        }

        public bool CanDoWork { get; private set; }

        public IEnumerable<ObjectBuffer<PatientDataFile>> WorkUnits
        {
            get
            {
                String[] files = Directory.GetFiles(currentDirectory, "*.pdt");
                int totalFiles = files.Length;
                ObjectBuffer<PatientDataFile> buffer = new ObjectBuffer<PatientDataFile>();

                for (int i = 0; i < totalFiles; ++i)
                {
                    if (!bgWorker.CancellationPending)
                    {
                        PatientDataFile patient = new PatientDataFile(files[i]);
                        if (patient.loadHeader())
                        {
                            if (buffer.addItem(patient))
                            {
                                Progress = (int)(((float)i / totalFiles) * 100.0f);
                                yield return buffer;
                            }
                        }
                    }
                }
                if (buffer.HasItems) //Make sure any extra files are also returned.
                {
                    Progress = 0;
                    yield return buffer;
                }
            }
        }

        public int Progress { get; private set; }

        public void resetDisplay()
        {
            dialog.clearFileList();
        }

        public void processingStarted()
        {
            dialog.LoadingProgressVisible = true;
        }

        public void workProcessed(ObjectBuffer<PatientDataFile> item)
        {
            foreach (PatientDataFile file in item.Items)
            {
                dialog.addFile(file);
            }
            item.reset();
        }

        public void updateProgress(int progressPercentage)
        {
            dialog.LoadingProgress = (uint)progressPercentage;
        }

        public void runWorkCompleted()
        {
            dialog.LoadingProgressVisible = false;
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

        public bool IsWorking
        {
            get
            {
                return bgWorker.IsWorking;
            }
        }

        public void cancel()
        {
            bgWorker.cancel();
        }

        public void listFiles(String directory)
        {
            this.currentDirectory = directory;
            CanDoWork = Directory.Exists(directory);
            bgWorker.startWork();
        }
    }
}
