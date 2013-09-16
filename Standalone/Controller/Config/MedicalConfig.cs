using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Engine.Resources;
using Engine.Platform;
using Logging;
using Medical.GUI;

namespace Medical
{
    public class MedicalConfig
    {
        private static ConfigFile configFile;
        private static ConfigSection program;

        private static String docRoot;
        private static String windowsFile;
        private static String bookmarksFolder;
        private static String recentDocsFile;

        private static String sceneDirectory;

        private static String userAnomalousFolder;
        private static String commonAnomalousFolder;

        private static float cameraTransitionTime;
        private static float transparencyChangeMultiplier;

        private static PluginConfig pluginConfig;

#if ALLOW_OVERRIDE
        private static ConfigFile overrideSettings = null;
        private static ConfigSection resources = null;
#endif

        static MedicalConfig()
        {
            BuildName = null;
#if ALLOW_OVERRIDE
            BuildName = "Internal";
#endif
        }

        public MedicalConfig(String userAnomalousFolder, String commonAnomalousFolder)
        {
            WebsiteHostUrl = "https://www.anomalousmedical.com";

            //Setup directories
            MedicalConfig.userAnomalousFolder = userAnomalousFolder;
            if (!Directory.Exists(userAnomalousFolder))
            {
                Directory.CreateDirectory(userAnomalousFolder);
            }

#if ALLOW_OVERRIDE
            //Override settings
			String overrideFile = Path.Combine(FolderFinder.ExecutableFolder, PlatformConfig.OverrideFileLocation);
            if (!File.Exists(overrideFile))
            {
                overrideFile = Path.Combine(userAnomalousFolder, "override.ini");
            }
            if (File.Exists(overrideFile))
            {
                Log.Info("Using override file {0}", overrideFile);
                overrideSettings = new ConfigFile(overrideFile);
                overrideSettings.loadConfigFile();
                resources = overrideSettings.createOrRetrieveConfigSection("Resources");

                ConfigSection website = overrideSettings.createOrRetrieveConfigSection("Website");
                WebsiteHostUrl = website.getValue("Host", WebsiteHostUrl);

                Cracked = false;
                ConfigSection systemOverride = overrideSettings.createOrRetrieveConfigSection("System");
                Cracked = systemOverride.getValue("Cracked", Cracked);
                BuildName = systemOverride.getValue("CustomBuildName", BuildName);
                libRocketPlugin.RocketInterface.Instance.PixelsPerInch = systemOverride.getValue("PixelsPerInch", libRocketPlugin.RocketInterface.DefaultPixelsPerInch);
                float pixelScale = systemOverride.getValue("PixelScale", 1.0f);
                MyGUIPlugin.MyGUIInterface.ScaleFactor = pixelScale;
                libRocketPlugin.RocketInterface.Instance.PixelScale = pixelScale;
                AllowUnsignedDllPlugins = systemOverride.getValue("AllowUnsignedDllPlugins", false);
                AllowUnsignedDataFilePlugins = systemOverride.getValue("AllowUnsignedDataFilePlugins", false);
                CertificateStoreTrustedRoot = systemOverride.getValue("CertificateStoreTrustedRoot", (String)null);
                CertificateStoreTrustedSignature = systemOverride.getValue("CertificateStoreTrustedSignature", (String)null);
            }
#endif
            //Fix up paths based on the build name
            String buildUrlExtraPath = "";
            if (!String.IsNullOrEmpty(BuildName))
            {
                buildUrlExtraPath = "/" + BuildName;
                commonAnomalousFolder = Path.Combine(commonAnomalousFolder, BuildName);
            }

            //Setup common folder
            MedicalConfig.commonAnomalousFolder = commonAnomalousFolder;
            if (!Directory.Exists(commonAnomalousFolder))
            {
                Directory.CreateDirectory(commonAnomalousFolder);
            }
            TemporaryFilesPath = Path.Combine(commonAnomalousFolder, "Temp");
            if (!Directory.Exists(TemporaryFilesPath))
            {
                Directory.CreateDirectory(TemporaryFilesPath);
            }

            //Configure plugins
            pluginConfig = new PluginConfig(Path.Combine(commonAnomalousFolder, "Plugins"));

#if ALLOW_OVERRIDE
            if (overrideSettings != null)
            {
                pluginConfig.readPlugins(overrideSettings);
            }
#endif

            //User configuration settings
            configFile = new ConfigFile(userAnomalousFolder + "/config.ini");
            configFile.loadConfigFile();

            program = configFile.createOrRetrieveConfigSection("Program");
            sceneDirectory = "Scenes";

            cameraTransitionTime = program.getValue("CameraTransitionTime", 0.5f);
            transparencyChangeMultiplier = program.getValue("TransparencyChangeMultiplier", 2.0f);
            ServerConnection.DefaultTimeout = program.getValue("DefaultTimeout", ServerConnection.DefaultTimeout);

            EngineConfig = new EngineConfig(configFile);

            SafeDownloadFolder = Path.Combine(commonAnomalousFolder, "Downloads");
            if (!Directory.Exists(SafeDownloadFolder))
            {
                Directory.CreateDirectory(SafeDownloadFolder);
            }

            //Certificate store
            MedicalConfig.CertificateStoreFile = Path.Combine(userAnomalousFolder, "CertificateStore.cst");

            //Configure website urls
            MedicalConfig.HelpURL = String.Format("{0}/Help", WebsiteHostUrl);
            MedicalConfig.ForgotPasswordURL = String.Format("{0}/RecoverPassword", WebsiteHostUrl);
            MedicalConfig.RegisterURL = String.Format("{0}/Login", WebsiteHostUrl);
            MedicalConfig.LicenseServerURL = String.Format("{0}/DRM3/License", WebsiteHostUrl);
            MedicalConfig.AnomalousMedicalStoreURL = String.Format("{0}/Store", WebsiteHostUrl);
            MedicalConfig.AnomalousMedicalBlogURL = String.Format("{0}/Blog", WebsiteHostUrl);
            MedicalConfig.ProductPageBaseURL = String.Format("{0}/Product/Plugin/{{0}}", WebsiteHostUrl);
            MedicalConfig.LicenseReaderURL = String.Format("{0}/DRM3/LicenseReader", WebsiteHostUrl);
            MedicalConfig.LicenseImageURL = String.Format("{0}/DRM3/LicenseImage", WebsiteHostUrl);
            MedicalConfig.ImageStoreURL = String.Format("{0}/Store/Image_Licensing", WebsiteHostUrl);
            MedicalConfig.DefaultAdUrl = String.Format("{0}/DRM3/ProgramAd", WebsiteHostUrl);
            MedicalConfig.CertificateStoreUrl = String.Format("{0}/DRM3/CertificateStore", WebsiteHostUrl);

            MedicalConfig.UpdateCheckURL = String.Format("{0}/DRM3/Update{1}", WebsiteHostUrl, buildUrlExtraPath);
            MedicalConfig.PluginInfoURL = String.Format("{0}/DRM3/DownloadInfo{1}", WebsiteHostUrl, buildUrlExtraPath);
            MedicalConfig.PluginDownloadURL = String.Format("{0}/DRM3/FileDownload{1}", WebsiteHostUrl, buildUrlExtraPath);

            //Read command line
            String[] commandLine = Environment.GetCommandLineArgs();
            for (int i = 0; i < commandLine.Length; ++i)
            {
                switch (commandLine[i])
                {
                    case "-startTask":
                        if (i + 1 < commandLine.Length)
                        {
                            StartupTask = commandLine[i + 1];
                        }
                        else
                        {
                            Log.Error("Command line argument '-startTask' requires a task name after it.");
                        }
                        break;
                }
            }
        }

        public static void setUserDirectory(String username)
        {
            //There is a tiny chance of name collisions here if two users have an invalid char in the same place, but otherwise identical names.
            //I'm not that worried about it.
            String sanatizedUser = PathExtensions.MakeValidFileName(username);
            MedicalConfig.docRoot = Path.Combine(Path.Combine(userAnomalousFolder, "Users"), sanatizedUser);
            windowsFile = docRoot + "/windows.ini";
            bookmarksFolder = docRoot + "/Bookmarks";
            recentDocsFile = docRoot + "/docs.ini";
            ImageOutputFolder = docRoot + "/Images";
            if (!Directory.Exists(docRoot))
            {
                Directory.CreateDirectory(docRoot);
            }
        }

        public static String LogFile
        {
            get
            {
                return Path.Combine(userAnomalousFolder, "Log.log");
            }
        }

        public static String CrashLogDirectory
        {
            get
            {
                return Path.Combine(userAnomalousFolder, "CrashLogs");
            }
        }

        public static String SafeDownloadFolder { get; private set; }

        public static String TemporaryFilesPath { get; private set; }

        public static String UserDocRoot
        {
            get
            {
                return docRoot;
            }
        }

        public static ConfigFile ConfigFile
        {
            get
            {
                return configFile;
            }
        }

        public static String WindowsFile
        {
            get
            {
                return windowsFile;
            }
        }

        public static String BookmarksFolder
        {
            get
            {
                return bookmarksFolder;
            }
        }

        public static String SaveDirectory
        {
            get
            {
                return program.getValue("SaveDirectory", userAnomalousFolder + "/SavedFiles");
            }
            set
            {
                program.setValue("SaveDirectory", value);
            }
        }

        public static bool EnableMultitouch
        {
            get
            {
                return program.getValue("EnableMultitouch", PlatformConfig.DefaultEnableMultitouch);
            }
            set
            {
                program.setValue("EnableMultitouch", value);
            }
        }

        public static bool StoreCredentials
        {
            get
            {
                return program.getValue("StoreCredentials", false);
            }
            set
            {
                program.setValue("StoreCredentials", value);
            }
        }

        public static bool FirstRun
        {
            get
            {
                return program.getValue("FirstRun", true);
            }
            set
            {
                program.setValue("FirstRun", value);
            }
        }

        public static String LicenseServerURL { get; private set; }

        public static String HelpURL { get; private set; }

        public static String BuildName { get; private set; }

#if ALLOW_OVERRIDE
        public static String WorkingResourceDirectory
        {
            get
            {
                if (overrideSettings != null)
                {
                    return resources.getValue("WorkingResourceDirectory", "");
                }
                return "";
            }
        }

        public static bool Cracked { get; private set; }

        public static bool AllowUnsignedDllPlugins { get; private set; }

        public static bool AllowUnsignedDataFilePlugins { get; private set; }

        public static String CertificateStoreTrustedRoot { get; private set; }

        public static String CertificateStoreTrustedSignature { get; private set; }

        public static bool OverrideCertificateStore
        {
            get
            {
                return !String.IsNullOrEmpty(CertificateStoreTrustedRoot) && !String.IsNullOrEmpty(CertificateStoreTrustedSignature);
            }
        }
#endif

        public static String DefaultScene
        {
            get
            {
                return program.getValue("DefaultScene", "Female.sim.xml");
            }
            set
            {
                program.setValue("DefaultScene", value);
            }
        }

        /// <summary>
        /// This is the scene to use if the default scene does not exist.
        /// </summary>
        public static String FallbackScene
        {
            get
            {
                return "Female.sim.xml";
            }
        }

        public static EngineConfig EngineConfig { get; private set; }

        public static void save()
        {
            configFile.writeConfigFile();
        }

        public static String SceneDirectory
        {
            get
            {
                return sceneDirectory;
            }
        }

        public static float CameraTransitionTime
        {
            get
            {
                return cameraTransitionTime;
            }
            set
            {
                cameraTransitionTime = value;
                program.setValue("CameraTransitionTime", value);
            }
        }

        public static MouseButtonCode CameraMouseButton
        {
            get
            {
                MouseButtonCode buttonCode = PlatformConfig.DefaultCameraMouseButton;

                String mouseButton = program.getValue("CameraMouseButton", buttonCode.ToString());
                try
                {
                    buttonCode = (MouseButtonCode)Enum.Parse(typeof(MouseButtonCode), mouseButton);
                }
                catch (Exception)
                {
                    Log.Warning("Could not parse the mouse button code {0}. Using default.", mouseButton);
                }
                return buttonCode;
            }
            set
            {
                program.setValue("CameraMouseButton", value.ToString());
            }
        }

        public static String RecentDocsFile
        {
            get
            {
                return recentDocsFile;
            }
        }

        public static PluginConfig PluginConfig
        {
            get
            {
                return pluginConfig;
            }
        }

        public static string LicenseFile
        {
            get
            {
                return Path.Combine(userAnomalousFolder, "License.lic");
            }
        }

        public static String ForgotPasswordURL { get; private set; }

        public static String RegisterURL { get; private set; }

        public static String PluginInfoURL { get; private set; }

        public static String PluginDownloadURL { get; private set; }

        public static String AnomalousMedicalStoreURL { get; private set; }

        public static String ProductPageBaseURL { get; private set; }

        public static String UpdateCheckURL { get; private set; }

        public static String LicenseReaderURL { get; private set; }

        public static String ImageOutputFolder { get; private set; }

        public static String LicenseImageURL { get; private set; }

        public static String ImageStoreURL { get; private set; }

        public static String DefaultAdUrl { get; private set; }

        public static String AnomalousMedicalBlogURL { get; private set; }

        public static String StartupTask { get; private set; }

        public static String CertificateStoreFile { get; private set; }

        public static String CertificateStoreUrl { get; private set; }

        public static String WebsiteHostUrl { get; private set; }
    }
}