using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class EmptyProjectTemplate : ProjectTemplate
    {
        public void createProject(EditorResourceProvider resourceProvider, String projectName)
        {
            
        }

        public String getDefaultFileName(String projectName)
        {
            return null;
        }
    }
}
