﻿using System;
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
        private static String docRoot;
        private static String windowsFile;
        private static String camerasFile;
        private static ConfigSection program;
        private static TaskbarAlignment taskbarAlignment = TaskbarAlignment.Top;
        private static String recentDocsFile;

        private static ConfigFile internalSettings = null;
        private static ConfigSection resources = null;

        private static String programDirectory;

        private static String sceneDirectory;

        private static String updateURL;
        private static String anomalousFolder;

        private static float cameraTransitionTime;
        private static float transparencyChangeMultiplier;

        public MedicalConfig(String anomalousFolder, String programFolder)
        {
            MedicalConfig.anomalousFolder = anomalousFolder;
            MedicalConfig.docRoot = Path.Combine(anomalousFolder, programFolder);
            MedicalConfig.updateURL = "http://www.AnomalousMedical.com/DRM/UpdateChecker.aspx";
            windowsFile = docRoot + "/windows.ini";
            camerasFile = docRoot + "/cameras.ini";
            recentDocsFile = docRoot + "/docs.ini";
            if (!Directory.Exists(docRoot))
            {
                Directory.CreateDirectory(docRoot);
            }
            configFile = new ConfigFile(anomalousFolder + "/config.ini");
            configFile.loadConfigFile();
            EngineConfig = new EngineConfig(configFile);
            program = configFile.createOrRetrieveConfigSection("Program");
            sceneDirectory = "Scenes";

            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                programDirectory = Path.GetDirectoryName(args[0]);
            }
            else
            {
                programDirectory = ".";
            }
            cameraTransitionTime = program.getValue("CameraTransitionTime", 0.5f);
            transparencyChangeMultiplier = program.getValue("TransparencyChangeMultiplier", 2.0f);

            String taskbarAlignmentString = program.getValue("TaskbarAlignment", taskbarAlignment.ToString());
            try
            {
                taskbarAlignment = (TaskbarAlignment)Enum.Parse(typeof(TaskbarAlignment), taskbarAlignmentString);
            }
            catch (Exception)
            {
                Log.Warning("Could not parse the taskbar alignment {0}. Using default.", taskbarAlignmentString);
            }

            LicenseServerURL = "https://www.anomalousmedical.com/DRM/LicenseServer.aspx";

#if ALLOW_OVERRIDE
            if (File.Exists(programDirectory + "/override.ini"))
            {
                internalSettings = new ConfigFile(programDirectory + "/override.ini");
                internalSettings.loadConfigFile();
                resources = internalSettings.createOrRetrieveConfigSection("Resources");

                ConfigSection updates = internalSettings.createOrRetrieveConfigSection("Updates");
                updateURL = updates.getValue("UpdateURL", updateURL);
                LicenseServerURL = updates.getValue("LicenseServerURL", LicenseServerURL);
            }
#endif
        }

        public static String DocRoot
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

        public static String CamerasFile
        {
            get
            {
                return camerasFile;
            }
        }

        public static String SaveDirectory
        {
            get
            {
                return program.getValue("SaveDirectory", anomalousFolder + "/SavedFiles");
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

        public static String LicenseServerURL { get; private set; }

#if ALLOW_OVERRIDE
        public static String WorkingResourceDirectory
        {
            get
            {
                if (internalSettings != null)
                {
                    return resources.getValue("WorkingResourceDirectory", "");
                }
                return "";
            }
        }
#endif

        public static String DefaultScene
        {
            get
            {
#if ALLOW_OVERRIDE
                if (internalSettings != null)
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

        public static String ProgramDirectory
        {
            get
            {
                return programDirectory;
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

        public static float TransparencyChangeMultiplier
        {
            get
            {
                return transparencyChangeMultiplier;
            }
            set
            {
                transparencyChangeMultiplier = value;
                program.setValue("TransparencyChangeMultiplier", value);
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

        public static TaskbarAlignment TaskbarAlignment
        {
            get
            {
                return taskbarAlignment;
            }
            set
            {
                taskbarAlignment = value;
                program.setValue("TaskbarAlignment", taskbarAlignment.ToString());
            }
        }

        public static String RecentDocsFile
        {
            get
            {
                return recentDocsFile;
            }
        }
    }
}
