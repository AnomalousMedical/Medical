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
        WindowSoftInputMode = SoftInput.StateAlwaysHidden, LaunchMode = LaunchMode.SingleTop)]
    [MetaData("android.app.lib_name", Value = AndroidPlatformPlugin.LibraryName)]
    public class MainActivity : AndroidActivity
    {
        static MainActivity()
        {
            Java.Lang.JavaSystem.LoadLibrary("gnustl_shared");
            Java.Lang.JavaSystem.LoadLibrary("openal");
			ServerConnection.EnableUnsafeTLS1_0 = true;
        }

        private ObbDownloader dl;
        private AnomalousController anomalousController;

        #if DEBUG
        private const bool SucceedIfEmpty = true;
        #else
        private const bool SucceedIfEmpty = false;
        #endif

        public MainActivity()
            : base(AnomalousMedicalAndroid.Resource.Layout.Main, AnomalousMedicalAndroid.Resource.Id.editText1)
        {

        }

        protected override void createApp()
        {
            NativePlatformPlugin.StaticInitialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;
            OgrePlugin.OgreInterface.InitialClearColor = new Color(0.156f, 0.156f, 0.156f);

            #if DEBUG
            Logging.Log.Default.addLogListener(new Logging.LogConsoleListener());
            #endif

            OtherProcessManager.OpenUrlInBrowserOverride = openUrl;

            dl = new ObbDownloader(this);
            dl.DownloadSucceeded += Dl_DownloadSucceeded;
            dl.DownloadFailed += Dl_DownloadFailed;
            dl.DownloadProgressUpdated += Dl_DownloadProgressUpdated;
            dl.NeedCellularPermission += Dl_NeedCellularPermission;

            String archiveName = null;

            #if DEBUG
            String testingArtFile = "/storage/emulated/0/InternalRelease.dat";
            if (File.Exists(testingArtFile))
            {
                archiveName = testingArtFile;
            }
            else
            {
            #endif
                
            if (dl.AreExpansionFilesDelivered(SucceedIfEmpty))
            {
                archiveName = findExpansionFile();
            }

            #if DEBUG
            }
            Logging.Log.Debug("Archive Name {0}", archiveName);
            #endif
            
            anomalousController = new AnomalousController()
            {
                    PrimaryArchive = archiveName
            };
            anomalousController.OnInitCompleted += HandleOnInitCompleted;
            anomalousController.DataFileMissing += HandleDataFileMissing;
            anomalousController.run();
        }

        protected override void OnDestroy()
        {
            dl.cancelDownloads();
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            dl.connectDownloadService();
            base.OnResume();
        }

        protected override void OnStop()
        {
            dl.disconnectDownloadService();
            base.OnStop();
        }

        void HandleOnInitCompleted(AnomalousController anomalousController, StandaloneController controller)
        {
            setInputHandler(controller.MedicalController.InputHandler);
            printRuntimeInfo();
        }

        void openUrl(String url)
        {
            RunOnUiThread(() =>
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                this.StartActivity(intent);
            });
        }

        void HandleDataFileMissing(AnomalousController anomalousController, StandaloneController controller)
        {
            dl.GetExpansionFiles();
        }

        void Dl_DownloadFailed()
        {
            MessageBox.show(String.Format("Error downloading resource archive.\nReason:\n{0}", dl.LastStateMessage), "Resource Archive Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok, r =>
            {
                anomalousController.StandaloneController.exit();
            });
        }

        void Dl_DownloadSucceeded()
        {
            //Reassign primary archive
            anomalousController.PrimaryArchive = findExpansionFile();
            //Run splash screen again.
            anomalousController.rerunSplashScreen();
        }

        void Dl_DownloadProgressUpdated(string message, int current, int total)
        {
            anomalousController.splashShowDownloadProgress(message, current, total);
        }

        void Dl_NeedCellularPermission()
        {
            MessageBox.show(String.Format("Anomalous Medical needs to download additional files.\nThese files total {0}.\nDo you wish to download these files over your cellular connection?\nAdditional carrier charges may apply.\nClick No to cancel the download and try again later over wifi.", Prettify.GetSizeReadable(dl.TotalDownloadSize)), "Resource Archive Error", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, r =>
            {
                if (r == MessageBoxStyle.Yes)
                {
                    dl.resumeOnCellData();
                }
                else
                {
                    anomalousController.StandaloneController.exit();
                }
            });
        }

        private String findExpansionFile()
        {
            try
            {
                String obbWildcard = String.Format("main.*.{0}.obb", BaseContext.ApplicationInfo.PackageName.ToString());
                var files = Directory.EnumerateFiles(Application.Context.ObbDir.AbsolutePath, obbWildcard, SearchOption.AllDirectories);
                if (files.Count() > 1)
                {
                    //Find the file with the highest version number, only does main files for now.
                    String largestVersion = null;
                    long version = 0;
                    foreach (var file in files)
                    {
                        String fileName = Path.GetFileName(file);
                        if (fileName.StartsWith("main.", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int trailingDot = fileName.IndexOf(".", 5);
                            if (trailingDot > 0)
                            {
                                long testVersion;
                                if (long.TryParse(fileName.Substring(5, trailingDot - 5), out testVersion) && testVersion > version)
                                {
                                    version = testVersion;
                                    largestVersion = file;
                                }
                            }
                        }
                    }
                    return largestVersion;
                }
                else
                {
                    //Only one matching file, just return it
                    return files.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} looking for resource archive. Message: {1}", ex.GetType().ToString(), ex.Message);
                return null;
            }
        }
    }
}


