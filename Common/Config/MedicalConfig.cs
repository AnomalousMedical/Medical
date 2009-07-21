using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

namespace Medical
{
    public class MedicalConfig
    {
        private static ConfigFile configFile;
        private static String docRoot;
        private static String windowsFile;
        private static String camerasFile;
        private static CameraSection cameraSection;
        private static ConfigSection program;

        public MedicalConfig(String docRoot)
        {
            MedicalConfig.docRoot = docRoot;
            windowsFile = docRoot + "/windows.ini";
            camerasFile = docRoot + "/cameras.ini";
            if (!Directory.Exists(docRoot))
            {
                Directory.CreateDirectory(docRoot);
            }
            configFile = new ConfigFile(docRoot + "/config.ini");
            configFile.loadConfigFile();
            cameraSection = new CameraSection(configFile);
            EngineConfig = new EngineConfig(configFile);
            program = configFile.createOrRetrieveConfigSection("Program");
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

        public static CameraSection CameraSection
        {
            get
            {
                return cameraSection;
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
                return program.getValue("SaveDirectory", docRoot + "/Files");
            }
            set
            {
                program.setValue("SaveDirectory", value);
            }
        }

        public static EngineConfig EngineConfig { get; private set; }

        public static void save()
        {
            configFile.writeConfigFile();
        }

    }
}
