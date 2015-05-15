using System;
using ExpansionDownloader.Service;
using ExpansionDownloader;
using ExpansionDownloader.Client;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Content.PM;
using Engine.Threads;
using ExpansionDownloader.Database;
using System.Linq;

namespace AnomalousMedicalAndroid
{
    public class ObbDownloader : IDownloaderClient
    {
        private IDownloaderService downloaderService;
        private IDownloaderServiceConnection downloaderServiceConnection;

        public event Action DownloadFailed;
        public event Action DownloadSucceeded;
        public event Action<String, int, int> DownloadProgressUpdated;
        public event Action PromptDownload;

        private Activity activity;

        public ObbDownloader(Activity activity)
        {
            this.activity = activity;
        }

        /// <summary>
        /// Are the expansion files delivered.
        /// </summary>
        /// <returns><c>true</c>, if expansion files delivered delivered, <c>false</c> otherwise.</returns>
        /// <param name="succeedIfEmpty">If set to <c>true</c> succeed if the metadata says this apk has no files.</param>
        public bool AreExpansionFilesDelivered(bool succeedIfEmpty)
        {
            var downloads = DownloadsDatabase.GetDownloads();
            //True if we have no associated downloads or all downloads match the expected values.
            return (succeedIfEmpty && downloads.Count == 0) 
                || (downloads.Any() && downloads.All(x => Helpers.DoesFileExist(activity, x.FileName, x.TotalBytes, false)));
        }

        public void GetExpansionFiles()
        {
            try
            {
                activity.RunOnUiThread(() =>
                    {
                        DownloadServiceRequirement startResult = DownloaderService.DetermineDownloadStatus(activity);
                        if(startResult == DownloadServiceRequirement.NoDownloadRequired)
                        {
                            ThreadManager.invoke(() =>
                            {
                                if(DownloadSucceeded != null)
                                {
                                    DownloadSucceeded.Invoke();
                                }
                            });
                        }
                        else
                        {
                            ThreadManager.invoke(() =>
                            {
                                if(PromptDownload != null)
                                {
                                    PromptDownload.Invoke();
                                }
                            });
                        }
                    });
            }
            catch (PackageManager.NameNotFoundException e)
            {
                //Debug.WriteLine("Cannot find own package! MAYDAY!");
                e.PrintStackTrace();
            }
        }

        /// <summary>
        /// Call this during the promptDownload callback to actually start the download.
        /// </summary>
        public void startDownload()
        {
            // Build the intent that launches this activity.
            Intent launchIntent = activity.Intent;
            var intent = new Intent(activity, typeof(MainActivity));
            //intent.SetFlags(ActivityFlags. | ActivityFlags.ClearTop);
            intent.SetAction(launchIntent.Action);

            if (launchIntent.Categories != null)
            {
                foreach (string category in launchIntent.Categories)
                {
                    intent.AddCategory(category);
                }
            }

            // Build PendingIntent used to open this activity when user 
            // taps the notification.
            PendingIntent pendingIntent = PendingIntent.GetActivity(activity, 0, intent, PendingIntentFlags.UpdateCurrent);

            //Start the download
            DownloaderService.StartDownloadServiceIfRequired(DownloadServiceRequirement.DownloadRequired, activity, pendingIntent, typeof(AnomalousMedicalDownloaderService));
            downloaderServiceConnection = ClientMarshaller.CreateStub(this, typeof(AnomalousMedicalDownloaderService));
            downloaderServiceConnection.Connect(activity);
        }

        #region IDownloaderClient implementation

        public void OnDownloadProgress(DownloadProgressInfo progress)
        {
            String progressMessage = String.Format("Downloading expansion files, {0} of {1:0.00} MB completed, {2} remaining.", 
                Helpers.GetDownloadProgressPercent(progress.OverallProgress, progress.OverallTotal), 
                progress.OverallTotal / 1048576.0f, 
                Helpers.GetTimeRemaining(progress.TimeRemaining));

            ThreadManager.invoke(() =>
            {
                if(DownloadProgressUpdated != null)
                {
                    DownloadProgressUpdated.Invoke(progressMessage, (int)(progress.OverallProgress >> 8), (int)(progress.OverallTotal >> 8));
                }
            });
        }

        public void OnDownloadStateChanged(DownloaderState newState)
        {
            LastStateMessage = Helpers.GetDownloaderStringFromState(activity, newState);
            switch (newState)
            {
                case DownloaderState.Failed:
                case DownloaderState.FailedCanceled:
                case DownloaderState.FailedFetchingUrl:
                case DownloaderState.FailedSdCardFull:
                case DownloaderState.FailedUnlicensed:
                    ThreadManager.invoke(() =>
                    {
                        if (DownloadFailed != null)
                        {
                            DownloadFailed.Invoke();
                        }
                    });
                    break;
                case DownloaderState.Completed:
                    ThreadManager.invoke(() =>
                    {
                        if (DownloadSucceeded != null)
                        {
                            DownloadSucceeded.Invoke();
                        }
                    });
                    break;
            }
        }

        public void OnServiceConnected(Messenger m)
        {
            this.downloaderService = ServiceMarshaller.CreateProxy(m);
            this.downloaderService.OnClientUpdated(this.downloaderServiceConnection.GetMessenger());
        }

        public String LastStateMessage { get; private set; }

        #endregion
    }
}

