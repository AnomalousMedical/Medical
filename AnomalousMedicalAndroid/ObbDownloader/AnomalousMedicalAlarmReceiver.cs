using System;
using Android.Content;
using ExpansionDownloader.Service;
using Android.Content.PM;
using AnomalousMedicalAndroid;

namespace com.AnomalousMedical.Android
{
    [BroadcastReceiver(Exported = false)]
    public class AnomalousMedicalAlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                DownloaderService.StartDownloadServiceIfRequired(context, intent, typeof(AnomalousMedicalDownloaderService));
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}

