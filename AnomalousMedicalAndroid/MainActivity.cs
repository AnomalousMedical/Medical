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
using System.Net.Http;
using DentalSim;
using Android;

#if ALLOW_DATA_FILE
using Medical.Movement;
using Developer;
#endif

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
            Java.Lang.JavaSystem.LoadLibrary("FreeImage");
            Java.Lang.JavaSystem.LoadLibrary("openal");
        }

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
            //This works to get write permissions, but the app will crash
            //the first time you run it on a new phone.
            CheckAppPermissions();

            ActivityManager actManager = GetSystemService(ActivityService) as ActivityManager;
            var memoryInfo = new ActivityManager.MemoryInfo();
            actManager.GetMemoryInfo(memoryInfo);

            if (memoryInfo.TotalMem < 1536000000)
            {
                MedicalConfig.SetVirtualTextureMemoryUsageMode(MedicalConfig.VTMemoryMode.Small);
            }

            MedicalConfig.PlatformExtraScaling = 0.25f;

            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;
            OgrePlugin.OgreInterface.InitialClearColor = new Color(0.156f, 0.156f, 0.156f);

            #if DEBUG
            Logging.Log.Default.addLogListener(new Logging.LogConsoleListener());
            #endif

            OtherProcessManager.OpenUrlInBrowserOverride = openUrl;

            String archiveName = null;

            #if ALLOW_DATA_FILE
            String testingArtFile = "/storage/emulated/0/AnomalousMedical.dat";
            if (File.Exists(testingArtFile))
            {
                archiveName = testingArtFile;
            }
            else
            {
            #endif
                
            archiveName = findExpansionFile();

            #if ALLOW_DATA_FILE
            }
            Logging.Log.Debug("Archive Name {0}", archiveName);
            #endif
            
            anomalousController = new AnomalousController()
            {
                    PrimaryArchive = archiveName
            };
            anomalousController.OnInitCompleted += HandleOnInitCompleted;
            //anomalousController.DataFileMissing += HandleDataFileMissing;
            anomalousController.AddAdditionalPlugins += HandleAddAdditionalPlugins;
            anomalousController.run();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            anomalousController.Dispose();
            this.killAppProcess();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        void HandleOnInitCompleted(AnomalousController anomalousController, StandaloneController controller)
        {
            setInputHandler(controller.MedicalController.InputHandler);
            printRuntimeInfo();
        }

        static void HandleAddAdditionalPlugins(AnomalousController anomalousController, StandaloneController controller)
        {
            controller.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(controller)
                {
                    AllowUninstall = false
                });

            controller.AtlasPluginManager.addPlugin(new DentalSimPlugin()
                {
                    AllowUninstall = false
                });

            #if ALLOW_DATA_FILE
            controller.AtlasPluginManager.addPlugin(new MovementBodyAtlasPlugin()
            {
            AllowUninstall = false
            });
            controller.AtlasPluginManager.addPlugin(new DeveloperAtlasPlugin(controller)
            {
            AllowUninstall = false
            });
            #endif
        }

        void openUrl(String url)
        {
            RunOnUiThread(() =>
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                this.StartActivity(intent);
            });
        }

        //void HandleDataFileMissing(AnomalousController anomalousController, StandaloneController controller)
        //{
            
        //}

        void Dl_DownloadSucceeded()
        {
            //Reassign primary archive
            anomalousController.PrimaryArchive = findExpansionFile();
            //Run splash screen again.
            anomalousController.rerunSplashScreen();
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
                    return files.FirstOrDefault() ?? "CannotFindArchive.dat"; //Returning null here will make the program close right away, let it open with no archive
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} looking for resource archive. Message: {1}", ex.GetType().ToString(), ex.Message);
                return null;
            }
        }

        private void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                    RequestPermissions(permissions, 1);
                }
            }
        }
    }
}


