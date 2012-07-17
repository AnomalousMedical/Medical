using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical;

namespace Developer
{
    static class DeveloperConfig
    {
        static ConfigSection developerSection;

        static DeveloperConfig()
        {
            developerSection = MedicalConfig.ConfigFile.createOrRetrieveConfigSection("Developer");
        }

        public static String LastPluginKey
        {
            get
            {
                return developerSection.getValue("LastPluginKey", "");
            }
            set
            {
                developerSection.setValue("LastPluginKey", value);
            }
        }

        public static String LastPluginExportDirectory
        {
            get
            {
                return developerSection.getValue("LastPluginExportDirectory", "");
            }
            set
            {
                developerSection.setValue("LastPluginExportDirectory", value);
            }
        }
    }
}
