using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lecture
{
    static class EmbeddedTemplateNames
    {
        public const String MasterTemplate_trml = "Lecture.Resources.Project.MasterTemplate.trml";
        public const String Wysiwyg_rcss = "Lecture.Resources.Project.Wysiwyg.rcss";

        public static Assembly Assembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }
    }
}
