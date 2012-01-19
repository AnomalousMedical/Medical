using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using System.IO;

namespace LectureBuilder
{
    class LectureBuilderConfig
    {
        static LectureBuilderConfig()
        {
            LectureProjectDirectory = Path.Combine(MedicalConfig.UserDocRoot, "Lecture Companions");
        }

        public static String LectureProjectDirectory { get; set; }

        private LectureBuilderConfig() { }
    }
}
