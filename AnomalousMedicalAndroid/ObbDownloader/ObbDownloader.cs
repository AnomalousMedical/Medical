using System;
using ExpansionDownloader.Service;
using ExpansionDownloader;
using ExpansionDownloader.Client;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Content.PM;

namespace AnomalousMedicalAndroid
{
    public class ObbDownloader : IDownloaderClient
    {
        private IDownloaderService downloaderService;
        private IDownloaderServiceConnection downloaderServiceConnection;

        public ObbDownloader()
        {
        }

        public bool GetExpansionFiles(Activity activity)
        {
            bool result = false;

            try
            {
                activity.RunOnUiThread(() =>
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

                        // Request to start the download
                        DownloadServiceRequirement startResult = DownloaderService.StartDownloadServiceIfRequired(activity, pendingIntent, typeof(AnomalousMedicalDownloaderService));

                        // The DownloaderService has started downloading the files, 
                        // show progress otherwise, the download is not needed so  we 
                        // fall through to starting the actual app.
                        if (startResult != DownloadServiceRequirement.NoDownloadRequired)
                        {
                            downloaderServiceConnection = ClientMarshaller.CreateStub(this, typeof(AnomalousMedicalDownloaderService));
                            downloaderServiceConnection.Connect(activity);
                            //this.InitializeDownloadUi();
                            result = true;
                        }
                    });
            }
            catch (PackageManager.NameNotFoundException e)
            {
                //Debug.WriteLine("Cannot find own package! MAYDAY!");
                e.PrintStackTrace();
            }

            return result;
        }

        #region IDownloaderClient implementation

        public void OnDownloadProgress(DownloadProgressInfo progress)
        {
            Console.WriteLine(progress.OverallProgress.ToString());
        }

        public void OnDownloadStateChanged(DownloaderState newState)
        {
            Console.WriteLine(newState.ToString());
        }

        public void OnServiceConnected(Messenger m)
        {
            this.downloaderService = ServiceMarshaller.CreateProxy(m);
            this.downloaderService.OnClientUpdated(this.downloaderServiceConnection.GetMessenger());
            Console.WriteLine("Service connected");
        }

        #endregion
    }
}

