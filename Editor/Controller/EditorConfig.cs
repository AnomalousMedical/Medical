using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    static class EditorConfig
    {
        static EditorConfig()
        {
            
        }

        public static String ProjectDirectory
        {
            get
            {
                return MedicalConfig.UserDocRoot + "/Editor Projects";
            }
        }
    }
}
