using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using Engine.Resources;

namespace Medical
{
    public class MedicalConfig
    {
        private static ConfigFile configFile;
        private static String docRoot;
        private static String windowsFile;
        private static String camerasFile;
        private static ConfigSection program;

        private static ConfigFile internalSettings = null;
        private static ConfigSection resources = null;

        private static String programDirectory;
        private static String autoResourceRoot;

        private static String sceneDirectory;

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
            RecentDocuments = new RecentDocuments(configFile);
            configFile.loadConfigFile();
            EngineConfig = new EngineConfig(configFile);
            program = configFile.createOrRetrieveConfigSection("Program");
            sceneDirectory = "/Scenes";

            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                programDirectory = Path.GetDirectoryName(args[0]);
            }
            else
            {
                programDirectory = ".";
            }
#if ALLOW_OVERRIDE
            if (File.Exists(programDirectory + "/PiperJBO.dat"))
            {
                autoResourceRoot = programDirectory + "/PiperJBO.dat";
            }
            else if (File.Exists(programDirectory + "/PiperJBO.zip"))
            {
                autoResourceRoot = programDirectory + "/PiperJBO.zip";
            }
            else
            {
                autoResourceRoot = ".";
            }
            if (File.Exists(programDirectory + "/override.ini"))
            {
                internalSettings = new ConfigFile(programDirectory + "/override.ini");
                internalSettings.loadConfigFile();
                resources = internalSettings.createOrRetrieveConfigSection("Resources");
            }
#else
            autoResourceRoot = programDirectory + "/PiperJBO.dat";
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
                return program.getValue("SaveDirectory", docRoot + "/Files");
            }
            set
            {
                program.setValue("SaveDirectory", value);
            }
        }

        public static String ResourceRoot
        {
            get
            {
#if ALLOW_OVERRIDE
                if (internalSettings != null)
                {
                    return resources.getValue("ResourceRoot", autoResourceRoot);
                }
                else
                {
                    return autoResourceRoot;
                }
#else
                return autoResourceRoot;
#endif
            }
        }

        public static String DefaultScene
        {
            get
            {
#if ALLOW_OVERRIDE
                if (internalSettings != null)
                {
                    return resources.getValue("DefaultScene", SceneDirectory + "/Male.sim.xml");
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

        public static RecentDocuments RecentDocuments { get; private set; }

        public static void save()
        {
            configFile.writeConfigFile();
        }

        public static String SceneDirectory
        {
            get
            {
                return Resource.ResourceRoot + sceneDirectory;
            }
        }

        public static String ProgramDirectory
        {
            get
            {
                return programDirectory;
            }
        }

    }
}
