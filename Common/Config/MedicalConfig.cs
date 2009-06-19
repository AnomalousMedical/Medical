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
        private static String docRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Atlas";
        private static String windowsFile = docRoot + "/windows.ini";
        private static String camerasFile = docRoot + "/cameras.ini";
        private static CameraSection cameraSection;

        static MedicalConfig()
        {
            if (!Directory.Exists(docRoot))
            {
                Directory.CreateDirectory(docRoot);
            }
            configFile = new ConfigFile(docRoot + "/config.ini");
            configFile.loadConfigFile();
            cameraSection = new CameraSection(configFile);
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

        public static void save()
        {
            configFile.writeConfigFile();
        }

    }
}
