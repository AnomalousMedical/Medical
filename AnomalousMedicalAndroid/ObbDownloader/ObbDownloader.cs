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
using System.IO;
using System.Threading;

namespace AnomalousMedicalAndroid
{
    public class ObbDownloader : IDownloaderClient
    {
        private IDownloaderService downloaderService;
        private IDownloaderServiceConnection downloaderServiceConnection;
        private Activity activity;

        /// <summary>
        /// This event is fired if we need permission from the user to use their cell data.
        /// </summary>
        public event Action NeedCellularPermission;

        /// <summary>
        /// Called when the download fails.
        /// </summary>
        public event Action DownloadFailed;

        /// <summary>
        /// Called when the download is successful.
        /// </summary>
        public event Action DownloadSucceeded;

        /// <summary>
        /// Called when the download progress updates.
        /// </summary>
        public event Action<String, int, int> DownloadProgressUpdated;

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
            if (DownloaderService.NeedsUpdateDownloads(activity))
            {
                DownloadsDatabase.Reset();
            }
            var downloads = DownloadsDatabase.GetDownloads();
            //True if we have no associated downloads or all downloads match the expected values.
            return (succeedIfEmpty && downloads.Count == 0) 
                || (downloads.Any() && downloads.All(x => Helpers.DoesFileExist(activity, x.FileName, x.TotalBytes, false)));
        }

        /// <summary>
        /// Gets the expansion files.
        /// </summary>
        public void GetExpansionFiles()
        {
            activity.RunOnUiThread(() =>
            {
                try
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
                    DownloadServiceRequirement startResult = DownloaderService.StartDownloadServiceIfRequired(activity, pendingIntent, typeof(AnomalousMedicalDownloaderService));
                    if(startResult != DownloadServiceRequirement.NoDownloadRequired)
                    {
                        downloaderServiceConnection = ClientMarshaller.CreateStub(this, typeof(AnomalousMedicalDownloaderService));
                        downloaderServiceConnection.Connect(activity);
                    }
                    else
                    {
                        fireDownloadSucceeded();
                    }
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    //Debug.WriteLine("Cannot find own package! MAYDAY!");
                    e.PrintStackTrace();
                }
            });
        }

        /// <summary>
        /// If you are in a NeedCellularPermission, call this function to start the download back
        /// up over cellular data.
        /// </summary>
        public void resumeOnCellData()
        {
            activity.RunOnUiThread(() =>
            {
                this.downloaderService.SetDownloadFlags(ServiceFlags.FlagsDownloadOverCellular);
                this.downloaderService.RequestContinueDownload();
            });
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
                    fireDownloadFailed();
                    break;
                case DownloaderState.Completed:
                    deleteFiles();
                    fireDownloadSucceeded();
                    break;
                case DownloaderState.PausedWifiDisabledNeedCellularPermission:
                    fireNeedCellularPermission();
                    break;
            }
        }

        public void OnServiceConnected(Messenger m)
        {
            this.downloaderService = ServiceMarshaller.CreateProxy(m);
            this.downloaderService.OnClientUpdated(this.downloaderServiceConnection.GetMessenger());
        }

        public String LastStateMessage { get; private set; }

        public long TotalDownloadSize
        {
            get
            {
                long total = 0;
                foreach (var file in DownloadsDatabase.GetDownloads())
                {
                    total += file.TotalBytes;
                }
                return total;
            }
        }

        private void fireDownloadFailed()
        {
            ThreadManager.invoke(() =>
            {
                if (DownloadFailed != null)
                {
                    DownloadFailed.Invoke();
                }
            });
        }

        private void fireDownloadSucceeded()
        {
            ThreadManager.invoke(() =>
            {
                if (DownloadSucceeded != null)
                {
                    DownloadSucceeded.Invoke();
                }
            });
        }

        private void fireNeedCellularPermission()
        {
            ThreadManager.invoke(() =>
            {
                if (NeedCellularPermission != null)
                {
                    NeedCellularPermission.Invoke();
                }
                else if(DownloadFailed != null)
                {
                    DownloadFailed.Invoke();
                }
            });
        }

        private void deleteFiles()
        {
            //Do this on a background thread, just deleting extra files.
            ThreadPool.QueueUserWorkItem(arg =>
            {
                var downloads = DownloadsDatabase.GetDownloads();
                //Delete all old files
                foreach (var file in Directory.EnumerateFiles(Application.Context.ObbDir.AbsolutePath, "*", SearchOption.AllDirectories).Where(f => !downloads.Any(x => x.FileName.Equals(Path.GetFileName(f)))))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Error("{0} deleting obb file {1}\nMessage:", ex.GetType(), file, ex.Message);
                    }
                }
            });
        }

        #endregion
    }
}

