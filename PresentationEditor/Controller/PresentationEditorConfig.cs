using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical;

namespace PresentationEditor
{
    class PresentationEditorConfig
    {
        static PresentationEditorConfig()
        {
            
        }

        public static String ProjectDirectory
        {
            get
            {
                return MedicalConfig.UserDocRoot + "/Presentations";
            }
        }
    }
}
