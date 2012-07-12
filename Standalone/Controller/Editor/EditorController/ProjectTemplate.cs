using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public interface ProjectTemplate
    {
        void createProject(EditorResourceProvider resourceProvider, String projectName);

        String getDefaultFileName(String projectName);
    }
}
