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
    public enum UIExtraScale
    {
        Smaller,
        Normal,
        Larger
    }
    
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
        private static String localDataFileFolder;

        private static float cameraTransitionTime;
        private static float transparencyChangeMultiplier;
        private static bool autoOpenAnatomyFinder;

        private static PluginConfig pluginConfig;

#if ALLOW_OVERRIDE
        private static ConfigFile overrideSettings = null;
        private static ConfigSection resources = null;
#endif

        static MedicalConfig()
        {
            SetVirtualTextureMemoryUsageMode(VTMemoryMode.Normal);
            PlatformExtraScaling = 0.0f;
        }

        public enum VTMemoryMode
        {
            Small,
            Normal,
            Large
        }

        public static void SetVirtualTextureMemoryUsageMode(VTMemoryMode mode)
        {
            switch (mode)
            {
                case VTMemoryMode.Small:
                    MaxStagingVirtualTextureUploadsPerFrame = int.MaxValue;
                    VirtualTextureStagingBufferCount = 20;
                    TextureCacheSize = 100 * 1024 * 1024;
                    FeedbackBufferSize = new IntSize2(256, 128);
                    PhysicalTextureSize = new IntSize2(2048, 2048);
                    PageSize = 64;
                    break;

                case VTMemoryMode.Normal:
                    MaxStagingVirtualTextureUploadsPerFrame = int.MaxValue;
                    VirtualTextureStagingBufferCount = 20;
                    TextureCacheSize = 100 * 1024 * 1024;
                    FeedbackBufferSize = new IntSize2(256, 128);
                    PhysicalTextureSize = new IntSize2(4096, 4096);
                    PageSize = 128;
                    break;

                case VTMemoryMode.Large:
                    MaxStagingVirtualTextureUploadsPerFrame = int.MaxValue;
                    VirtualTextureStagingBufferCount = 20;
                    TextureCacheSize = 500 * 1024 * 1024;
                    FeedbackBufferSize = new IntSize2(256, 128);
                    PhysicalTextureSize = new IntSize2(8192, 8192);
                    PageSize = 128;
                    break;
            }
        }

        public MedicalConfig()
        {
            BuildName = null;
//#if ALLOW_OVERRIDE
//            BuildName = "Internal";
//#endif
            WebsiteHostUrl = "https://www.anomalousmedical.com";

            //Setup directories
            MedicalConfig.userAnomalousFolder = FolderFinder.LocalUserDocumentsFolder;
            if (!Directory.Exists(userAnomalousFolder))
            {
                Directory.CreateDirectory(userAnomalousFolder);
            }

#if ALLOW_OVERRIDE
            UnrestrictedEnvironmentOverride = true;

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

                //Look for redirect
                ConfigSection systemOverride = overrideSettings.createOrRetrieveConfigSection("System");
                try
                {
                    var redirectFile = systemOverride.getValue("Redirect", default(String));
                    if (!String.IsNullOrEmpty(redirectFile) && File.Exists(redirectFile))
                    {
                        Log.Info("Redirecting to override file {0}", redirectFile);
                        overrideSettings = new ConfigFile(redirectFile);
                        overrideSettings.loadConfigFile();
                        systemOverride = overrideSettings.createOrRetrieveConfigSection("System");
                    }
                }
                catch (Exception)
                {

                }

                resources = overrideSettings.createOrRetrieveConfigSection("Resources");

                ConfigSection website = overrideSettings.createOrRetrieveConfigSection("Website");
                WebsiteHostUrl = website.getValue("Host", WebsiteHostUrl);

                BuildName = systemOverride.getValue("CustomBuildName", BuildName);
                libRocketPlugin.RocketInterface.Instance.PixelsPerInch = systemOverride.getValue("PixelsPerInch", libRocketPlugin.RocketInterface.DefaultPixelsPerInch);
                PixelScaleOverride = systemOverride.getValue("PixelScaleOverride", -1.0f);
                UnrestrictedEnvironmentOverride = systemOverride.getValue("UnrestrictedEnvironmentOverride", UnrestrictedEnvironmentOverride);
                ThemeFileOverride = systemOverride.getValue("ThemeFileOverride", ThemeFileOverride);
                TrackMemoryLeaks = systemOverride.getValue("TrackMemoryLeaks", TrackMemoryLeaks);
                OpenGLESEmulatorPath = systemOverride.getValue("OpenGLESEmulatorPath", OpenGLESEmulatorPath);
            }
#endif
            //Fix up paths based on the build name
            String buildUrlExtraPath = "";
            String localDataFileFolder = FolderFinder.LocalDataFolder;
            if (!String.IsNullOrEmpty(BuildName))
            {
                buildUrlExtraPath = "/" + BuildName;
                localDataFileFolder = Path.Combine(localDataFileFolder, BuildName);
            }

            //Setup local private folder
            if(!Directory.Exists(FolderFinder.LocalPrivateDataFolder))
            {
                Directory.CreateDirectory(FolderFinder.LocalPrivateDataFolder);
            }

            //Setup local data folder
            MedicalConfig.localDataFileFolder = localDataFileFolder;
            if (!Directory.Exists(localDataFileFolder))
            {
                Directory.CreateDirectory(localDataFileFolder);
            }
            TemporaryFilesPath = Path.Combine(localDataFileFolder, "Temp");
            if (!Directory.Exists(TemporaryFilesPath))
            {
                Directory.CreateDirectory(TemporaryFilesPath);
            }

            //Configure plugins
            pluginConfig = new PluginConfig(Path.Combine(localDataFileFolder, "Plugins"));

#if ALLOW_OVERRIDE
            if (overrideSettings != null)
            {
                pluginConfig.findConfiguredPlugins(overrideSettings);
            }
#endif

            //User configuration settings
            configFile = new ConfigFile(ConfigFilePath);
            configFile.loadConfigFile();

            program = configFile.createOrRetrieveConfigSection("Program");
            sceneDirectory = "Scenes";

            cameraTransitionTime = program.getValue("CameraTransitionTime", 0.5f);
            transparencyChangeMultiplier = program.getValue("TransparencyChangeMultiplier", 2.0f);
            autoOpenAnatomyFinder = program.getValue("AutoOpenAnatomyFinder", true);

            EngineConfig = new EngineConfig(configFile);

            SafeDownloadFolder = Path.Combine(localDataFileFolder, "Downloads");
            if (!Directory.Exists(SafeDownloadFolder))
            {
                Directory.CreateDirectory(SafeDownloadFolder);
            }

            //Configure website urls
            MedicalConfig.HelpURL = String.Format("{0}/LearningCenter", WebsiteHostUrl);

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

#if ALLOW_OVERRIDE
        public static void saveOverride()
        {
            overrideSettings.writeConfigFile();
        }

        public static String OverrideBackingFile
        {
            get
            {
                return overrideSettings.BackingFile;
            }
            set
            {
                overrideSettings.BackingFile = value;
            }
        }
#endif

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

        public static string ConfigFilePath
        {
            get
            {
                return Path.Combine(userAnomalousFolder, "config.ini");
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

        public static String PatientSaveDirectory
        {
            get
            {
				return program.getValue("PatientSaveDirectory", DefaultPatientSaveDirectory);
            }
            set
            {
                program.setValue("PatientSaveDirectory", value);
            }
        }

		public static String DefaultPatientSaveDirectory
		{
			get 
			{
				return userAnomalousFolder + "/SavedFiles";
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

        public static String LastShaderVersion
        {
            get
            {
                return program.getValue("LastShaderVersion", "");
            }
            set
            {
                program.setValue("LastShaderVersion", value);
            }
        }

        /// <summary>
        /// Set some extra scaling for a particular platform, added when the main pixel scale is calculated.
        /// </summary>
        /// <value>The platform extra scaling.</value>
        public static float PlatformExtraScaling { get; set; }

        public static UIExtraScale ExtraScaling
        {
            get
            {
                UIExtraScale extraScale;
                if (!Enum.TryParse<UIExtraScale>(program.getValue("ExtraScaling", () => UIExtraScale.Normal.ToString()), out extraScale))
                {
                    extraScale = UIExtraScale.Normal;
                }
                return extraScale;
            }
            set
            {
                program.setValue("ExtraScaling", value.ToString());
            }
        }

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

        public static float PixelScaleOverride { get; private set; }

        /// <summary>
        /// This will allow an override to simulate a restricted environment. This will only work if the platform
        /// is unrestricted since it is anded with the platform's actual value.
        /// </summary>
        public static bool UnrestrictedEnvironmentOverride { get; private set; }

        public static bool HasThemeFileOverride
        {
            get
            {
                return !String.IsNullOrEmpty(ThemeFileOverride);
            }
        }

        public static String ThemeFileOverride { get; private set; }

        public static String OpenGLESEmulatorPath { get; private set; }
#endif

        /// <summary>
        /// Set the number of virtual texture staging buffer uploads per frame. Can reduce
        /// stuttering while still allowing efficient background loading of textures. This
        /// can be higher than the total number of staging buffers which implies unlimited
        /// upload per frame.
        /// </summary>
        public static int MaxStagingVirtualTextureUploadsPerFrame { get; set; }

        /// <summary>
        /// Set the number of staging buffers for the virtual texture. The more of these
        /// there are the more textures can be loaded in the background without needing
        /// to stop the background thread.
        /// </summary>
        public static int VirtualTextureStagingBufferCount { get; set; }

        public static ulong TextureCacheSize { get; set; }

        public static IntSize2 FeedbackBufferSize { get; set; }

        public static IntSize2 PhysicalTextureSize { get; set; }

        public static int PageSize { get; set; }

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
                return Path.Combine(FolderFinder.LocalPrivateDataFolder, "License.lic");
            }
        }

        public static String ImageOutputFolder { get; private set; }

        public static String StartupTask { get; private set; }

        public static String WebsiteHostUrl { get; private set; }

        public static bool TrackMemoryLeaks { get; private set; }
    }
}