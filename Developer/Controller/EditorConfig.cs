﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class DeveloperConfig
    {
        private static ConfigSection editorSection;

        static DeveloperConfig()
        {
            editorSection = MedicalConfig.ConfigFile.createOrRetrieveConfigSection("Editor");
        }

        public static String TimelineProjectDirectory
        {
            get
            {
                return editorSection.getValue("TimelineProjectDir", MedicalConfig.UserDocRoot + "/Timeline Projects");
            }
            set
            {
                editorSection.setValue("TimelineProjectDir", value);
            }
        }
    }
}