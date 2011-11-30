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

        private static String updateURL;
        private static String helpURL;
        private static String userAnomalousFolder;
        private static String commonAnomalousFolder;

        private static float cameraTransitionTime;
        private static float transparencyChangeMultiplier;

        private static PluginConfig pluginConfig;

#if ALLOW_OVERRIDE
        private static ConfigFile overrideSettings = null;
        private static ConfigSection resources = null;
#endif

        public MedicalConfig(String userAnomalousFolder, String commonAnomalousFolder)
        {
            //Setup directories
            MedicalConfig.userAnomalousFolder = userAnomalousFolder;
            if (!Directory.Exists(userAnomalousFolder))
            {
                Directory.CreateDirectory(userAnomalousFolder);
            }

            MedicalConfig.commonAnomalousFolder = commonAnomalousFolder;
            if (!Directory.Exists(commonAnomalousFolder))
            {
                Directory.CreateDirectory(commonAnomalousFolder);
            }

            //Configure plugins
            pluginConfig = new PluginConfig(Path.Combine(commonAnomalousFolder, "Plugins"));

            //Configure website urls
            MedicalConfig.updateURL = "http://www.AnomalousMedical.com/DRM/UpdateChecker.aspx";
            MedicalConfig.helpURL = "http://www.AnomalousMedical.com/HelpIndex.aspx?user={0}";
            MedicalConfig.ForgotPasswordURL = "https://www.anomalousmedical.com/RecoverPassword.aspx";
            MedicalConfig.RegisterURL = "https://www.anomalousmedical.com/Register.aspx";
            MedicalConfig.LicenseServerURL = "https://www.anomalousmedical.com/DRM/LicenseServer.aspx";
            MedicalConfig.PluginInfoURL = "https://www.anomalousmedical.com/DRM/DownloadInfo.aspx";
            MedicalConfig.PluginDownloadURL = "https://www.anomalousmedical.com/DRM/FileDownloader.aspx";
            MedicalConfig.AnomalousMedicalStoreURL = "https://www.anomalousmedical.com/Store.aspx";
            MedicalConfig.WebsiteImagesBaseURL = "http://www.anomalousmedical.com";
            MedicalConfig.ProductPageBaseURL = "http://www.anomalousmedical.com/ProductInfo.aspx?ProductId={0}";
            MedicalConfig.UpdateCheckURL = "https://www.anomalousmedical.com/DRM/UpdateCheck.aspx";
            MedicalConfig.LicenseReaderURL = "https://www.anomalousmedical.com/DRM/LicenseReader.aspx";
            MedicalConfig.LicenseImageURL = "https://www.anomalousmedical.com/DRM/LicenseImage.aspx";
            
            //User configuration settings
            configFile = new ConfigFile(userAnomalousFolder + "/config.ini");
            configFile.loadConfigFile();
            
            program = configFile.createOrRetrieveConfigSection("Program");
            sceneDirectory = "Scenes";

            cameraTransitionTime = program.getValue("CameraTransitionTime", 0.5f);
            transparencyChangeMultiplier = program.getValue("TransparencyChangeMultiplier", 2.0f);

            EngineConfig = new EngineConfig(configFile);

            SafeDownloadFolder = Path.Combine(commonAnomalousFolder, "Downloads");
            if (!Directory.Exists(SafeDownloadFolder))
            {
                Directory.CreateDirectory(SafeDownloadFolder);
            }

#if ALLOW_OVERRIDE
            //Override settings
			String overrideFile = Path.Combine(FolderFinder.ExecutableFolder, PlatformConfig.OverrideFileLocation);
            if (File.Exists(overrideFile))
            {				
                overrideSettings = new ConfigFile(overrideFile);
                overrideSettings.loadConfigFile();
                resources = overrideSettings.createOrRetrieveConfigSection("Resources");

                ConfigSection updates = overrideSettings.createOrRetrieveConfigSection("Updates");
                updateURL = updates.getValue("UpdateURL", updateURL);
                LicenseServerURL = updates.getValue("LicenseServerURL", LicenseServerURL);
                helpURL = updates.getValue("HelpURL", helpURL);
                ForgotPasswordURL = updates.getValue("ForgotPasswordURL", ForgotPasswordURL);
                RegisterURL = updates.getValue("RegisterURL", RegisterURL);
                PluginInfoURL = updates.getValue("PluginInfoURL", PluginInfoURL);
                PluginDownloadURL = updates.getValue("PluginDownloadURL", PluginDownloadURL);
                AnomalousMedicalStoreURL = updates.getValue("AnomalousMedicalStoreURL", AnomalousMedicalStoreURL);
                WebsiteImagesBaseURL = updates.getValue("WebsiteImagesBaseURL", WebsiteImagesBaseURL);
                ProductPageBaseURL = updates.getValue("ProductPageBaseURL", ProductPageBaseURL);
                UpdateCheckURL = updates.getValue("UpdateCheckURL", UpdateCheckURL);
                LicenseReaderURL = updates.getValue("LicenseReaderURL", LicenseReaderURL);
                LicenseImageURL = updates.getValue("LicenseImageURL", LicenseImageURL);

                pluginConfig.readPlugins(overrideSettings);

                Cracked = false;
                ConfigSection systemOverride = overrideSettings.createOrRetrieveConfigSection("System");
                Cracked = systemOverride.getValue("Cracked", Cracked);
            }
#endif
        }

        public static void setUser(String username)
        {
            MedicalConfig.docRoot = Path.Combine(Path.Combine(userAnomalousFolder, "Users"), username);
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
                return program.getValue("EnableMultitouch", true);
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

        public static String LicenseServerURL { get; private set; }

        public static String getHelpURL(String username)
        {
            return String.Format(helpURL, username);
        }

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
#endif

        public static String DefaultScene
        {
            get
            {
#if ALLOW_OVERRIDE
                if (overrideSettings != null)
                {
                    return resources.getValue("DefaultScene", SceneDirectory + "/Female.sim.xml");
                }
                else
                {
                    return SceneDirectory + "/Female.sim.xml";
                }
#else
                return SceneDirectory + "/Female.sim.xml";
#endif
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

        public static String UpdateURL
        {
            get
            {
                return updateURL;
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

        public static string ServerPublicKey
        {
            get
            {
                return "<RSAKeyValue><Modulus>rwW3rMupoggrRkZHE8h3YVnovrLaP7+t2DJYN9ZK0e7Ytn2XGf9SwKcGfjAGWfYwBi/F+oSrD0Tgb0rFDYj1uyG6yOLtga8DZxPCgcKz8WsbDkMMF+W2NpeNQW/51HYeBnHM7cL+DQDRdhscwUKqBcN87HIwaTeok0CDtknFJ2k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            }
        }

        public static String ForgotPasswordURL { get; private set; }

        public static String RegisterURL { get; private set; }

        public static String PluginInfoURL { get; private set; }

        public static String PluginDownloadURL { get; private set; }

        public static String AnomalousMedicalStoreURL { get; private set; }

        public static String ProductPageBaseURL { get; private set; }

        public static String WebsiteImagesBaseURL { get; private set; }

        public static String UpdateCheckURL { get; private set; }

        public static String LicenseReaderURL { get; private set; }

        public static String ImageOutputFolder { get; private set; }

        public static String LicenseImageURL { get; private set; }
    }
}