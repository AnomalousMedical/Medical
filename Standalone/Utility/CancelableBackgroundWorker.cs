using Medical.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class CancelableBackgroundWorker<T>
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private CancelableBackgroundWorkTask<T> task;
        private bool bgThreadKnowsAboutCancel = false;
        private bool startNewScanOnBackgroundThreadStop = false;

        public CancelableBackgroundWorker(CancelableBackgroundWorkTask<T> task)
        {
            this.task = task;

            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.ProgressChanged += bgWorker_ProgressChanged;
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
        }

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

        public void cancel()
        {
            bgWorker.CancelAsync();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (task.CanDoWork)
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();

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

                //sw.Stop();
                //Log.Debug("Scanned files in {0}", sw.ElapsedMilliseconds);
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
