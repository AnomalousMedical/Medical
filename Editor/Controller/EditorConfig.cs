using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class EditorConfig
    {
        private static ConfigSection editorSection;

        static EditorConfig()
        {
            editorSection = MedicalConfig.ConfigFile.createOrRetrieveConfigSection("Editor");
        }

        public static String TimelineProjectDirectory
        {
            get
            {
                return editorSection.getValue("TimelineProjectDir", MedicalConfig.DocRoot + "/Timeline Projects");
            }
            set
            {
                editorSection.setValue("TimelineProjectDir", value);
            }
        }
    }
}
