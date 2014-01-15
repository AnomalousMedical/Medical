using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public static class EditorConfig
    {
        private static ConfigFile configFile;

        static EditorConfig()
        {
            configFile = new ConfigFile(MedicalConfig.UserDocRoot + "/editor.ini");
            configFile.loadConfigFile();
        }

        public static void save()
        {
            configFile.writeConfigFile();
        }

        public static String ProjectDirectory
        {
            get
            {
                return MedicalConfig.UserDocRoot + "/Editor Projects";
            }
        }

        public static ConfigSection getConfigSection(String name)
        {
            return configFile.createOrRetrieveConfigSection(name);
        }

        public static String readConfigHexColor(ConfigSection section, String name, String defaultColor)
        {
            Color testColor;
            String readColor = section.getValue(name, defaultColor);
            if (Color.TryFromRGBAString(readColor, out testColor))
            {
                return readColor;
            }
            return defaultColor;
        }
    }
}
