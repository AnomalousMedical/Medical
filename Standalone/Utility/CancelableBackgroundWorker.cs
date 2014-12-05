using Medical.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class simplifies the BackgroundWorker allowing you to work with an enumerable that is
    /// processed in a background thread with results synced to the main thread. It can also be canceled.
    /// </summary>
    /// <typeparam name="T">The type of items that are processed.</typeparam>
    public class CancelableBackgroundWorker<T>
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private CancelableBackgroundWorkTask<T> task;
        private bool bgThreadKnowsAboutCancel = false;
        private bool startNewScanOnBackgroundThreadStop = false;

        /// <summary>
        /// Constructor, takes a task class to process.
        /// </summary>
        /// <param name="task">The task class to process.</param>
        public CancelableBackgroundWorker(CancelableBackgroundWorkTask<T> task)
        {
            this.task = task;

            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
        }

        /// <summary>
        /// Start processing the given task.
        /// </summary>
        public void startWork()
        {
            if (bgWorker.IsBusy)
            {
                task.startedWhileBusy();
                bgWorker.CancelAsync();
            }
            else
            {
                task.resetDisplay();
                if (task.CanDoWork)
                {
                    startNewScanOnBackgroundThreadStop = false;
                    bgThreadKnowsAboutCancel = false;
                    task.processingStarted();
                    bgWorker.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// True if this worker is working and is not aware that it has been canceled.
        /// </summary>
        public bool IsWorking
        {
            get
            {
                return bgWorker.IsBusy && !bgThreadKnowsAboutCancel;
            }
        }

        /// <summary>
        /// True if the background worker cancellation is pending.
        /// </summary>
        public bool CancellationPending
        {
            get
            {
                return bgWorker.CancellationPending;
            }
        }

        /// <summary>
        /// Alert the bg worker that it should cancel.
        /// </summary>
        public void cancel()
        {
            bgWorker.CancelAsync();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (task.CanDoWork)
            {
                foreach (T workUnit in task.WorkUnits)
                {
                    if (bgWorker.CancellationPending)
                    {
                        bgThreadKnowsAboutCancel = true;
                        ThreadManager.invokeAndWait(listFilesCanceled);
                        break;
                    }

                    ThreadManager.invokeAndWait(() => task.workProcessed(workUnit));
                    bgWorker.ReportProgress(task.Progress);
                }
            }
        }

        void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ThreadManager.invoke(() => task.updateProgress(e.ProgressPercentage));
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ThreadManager.invoke(() =>
                {
                    task.runWorkCompleted();
                    if (startNewScanOnBackgroundThreadStop)
                    {
                        startWork();
                    }
                });
        }

        void listFilesCanceled()
        {
            startNewScanOnBackgroundThreadStop = task.canceled();
        }
    }
}
