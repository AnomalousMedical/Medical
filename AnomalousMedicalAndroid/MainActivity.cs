using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Anomalous.OSPlatform;
using Anomalous.OSPlatform.Android;
using Android.Views.InputMethods;
using Android.Content.PM;
using System.Collections.Generic;
using Android.Text;
using Engine;
using Medical;
using System.IO;
using MyGUIPlugin;

namespace AnomalousMedicalAndroid
{
    [Activity(Label = "Anomalous Medical", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AnomalousMedicalTheme", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout,
        WindowSoftInputMode = SoftInput.StateAlwaysHidden, LaunchMode=LaunchMode.SingleTop)]
    [MetaData("android.app.lib_name", Value = AndroidPlatformPlugin.LibraryName)]
    public class MainActivity : AndroidActivity
    {
        static MainActivity()
        {
            Java.Lang.JavaSystem.LoadLibrary("gnustl_shared");
            Java.Lang.JavaSystem.LoadLibrary("openal");
        }

        private ObbDownloader dl;
        private AnomalousController anomalousController;

        public MainActivity()
            : base(AnomalousMedicalAndroid.Resource.Layout.Main, AnomalousMedicalAndroid.Resource.Id.editText1)
        {

        }

        protected override void createApp()
        {
            NativePlatformPlugin.StaticInitialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.ETC2;
            OgrePlugin.OgreInterface.InitialClearColor = new Color(0.156f, 0.156f, 0.156f);

            #if DEBUG
            Logging.Log.Default.addLogListener(new Logging.LogConsoleListener());
            #endif

            OtherProcessManager.OpenUrlInBrowserOverride = openUrl;

            String obbWildcard = String.Format("main.*.{0}.obb", BaseContext.ApplicationInfo.PackageName.ToString());
            String archiveName = null;

            dl = new ObbDownloader(this);
            dl.DownloadSucceeded += Dl_DownloadSucceeded;
            dl.DownloadFailed += Dl_DownloadFailed;
            dl.DownloadProgressUpdated += Dl_DownloadProgressUpdated;

            if (dl.AreExpansionFilesDelivered())
            {
                try
                {
                    archiveName = Directory.EnumerateFiles(Application.Context.ObbDir.AbsolutePath, obbWildcard, SearchOption.AllDirectories).FirstOrDefault();
                }
                catch (Exception)
                {
                }
            }

            anomalousController = new AnomalousController()
            {
                PrimaryArchive = archiveName
            };
            anomalousController.OnInitCompleted += HandleOnInitCompleted;
            anomalousController.DataFileMissing += HandleDataFileMissing;
            anomalousController.run();
        }

        void HandleOnInitCompleted(AnomalousController anomalousController, StandaloneController controller)
        {
            setInputHandler(controller.MedicalController.InputHandler);
            printRuntimeInfo();
        }

        void openUrl(String url)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            this.StartActivity(intent);
        }

        void HandleDataFileMissing(AnomalousController anomalousController, StandaloneController controller)
        {
            MessageBox.show("Could not find resource archive. Would you like to try to download it now?", "Resource Archive Error", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, result =>
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        dl.GetExpansionFiles();
                    }
                    else
                    {
                        controller.exit();
                    }
                });
        }

        void Dl_DownloadFailed ()
        {
            MessageBox.show(String.Format("Error downloading resource archive.\nReason:\n{0}", dl.LastStateMessage), "Resource Archive Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok, r =>
                {
                    anomalousController.StandaloneController.exit();
                });
        }

        void Dl_DownloadSucceeded ()
        {
            //Reassign primary archive
            String obbWildcard = String.Format("main.*.{0}.obb", BaseContext.ApplicationInfo.PackageName.ToString());
            anomalousController.PrimaryArchive = Directory.EnumerateFiles(Application.Context.ObbDir.AbsolutePath, obbWildcard, SearchOption.AllDirectories).FirstOrDefault();
            //Run splash screen again.
            anomalousController.rerunSplashScreen();
        }

        void Dl_DownloadProgressUpdated (string message, int current, int total)
        {
            anomalousController.splashShowDownloadProgress(message, current, total);
        }
    }
}


