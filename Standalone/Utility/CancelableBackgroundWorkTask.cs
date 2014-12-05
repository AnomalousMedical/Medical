using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A task interface for the CancelableBackgroundWorker. These should only be assigned
    /// to one CancelableBackgroundWorker once created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface CancelableBackgroundWorkTask<T>
    {
        /// <summary>
        /// Return true if work can be done, this can be called on foreground or background
        /// threads so be sure what is done here is thread safe.
        /// </summary>
        bool CanDoWork { get; set; }

        /// <summary>
        /// An enumeration over work units, this function that will be called on the background
        /// thread so be sure that what you do here is thread safe.
        /// </summary>
        IEnumerable<T> WorkUnits { get; }

        /// <summary>
        /// Return an int of the current progress. This function will be called on the background thread
        /// so be sure that what you do here is thread safe.
        /// </summary>
        int Progress { get; }

        /// <summary>
        /// Called before the task is run and before it is determined if the task will run. This is called on the ThreadManager.invoke thread
        /// and will be safe to update the ui.
        /// </summary>
        void resetDisplay();

        /// <summary>
        /// Called before the task is run and after it is determined if the task will run. This is called on the ThreadManager.invoke thread
        /// and will be safe to update the ui.
        /// </summary>
        void processingStarted();

        /// <summary>
        /// A single work item from the WorkUnits enumerable has been processed. This is called on the
        /// ThreadManager.invoke thread and will be safe to update the ui.
        /// </summary>
        /// <param name="item"></param>
        void workProcessed(T item);

        /// <summary>
        /// The progress has been updated for the background task. This is called on the ThreadManager.invoke thread
        /// and will be safe to update the ui.
        /// </summary>
        /// <param name="progressPercentage"></param>
        void updateProgress(int progressPercentage);

        /// <summary>
        /// Called when the work is completed. This is called on the ThreadManager.invoke
        /// thread and is safe to update the ui.
        /// </summary>
        void runWorkCompleted();

        /// <summary>
        /// Return true to start a new scan of this tasks work units. This is called on the ThreadManager.invoke
        /// thread and is safe to update the ui.
        /// </summary>
        /// <returns></returns>
        bool canceled();

        /// <summary>
        /// This funciton is called if the task was started while the worker was busy.
        /// </summary>
        void startedWhileBusy();
    }
}
