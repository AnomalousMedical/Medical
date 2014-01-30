using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lecture
{
    static class EmbeddedTemplateNames
    {
        public const String SimpleSlide_rml = "Lecture.Resources.Project.SimpleSlide.rml";
        public const String MasterTemplate_trml = "Lecture.Resources.Project.MasterTemplate.trml";
        public const String SlideMasterStyles_rcss = "Lecture.Resources.Project.SlideMasterStyles.rcss";

        public static Assembly Assembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }
    }
}
