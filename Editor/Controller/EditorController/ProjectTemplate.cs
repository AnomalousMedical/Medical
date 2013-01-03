using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Reflection;
using System.IO;

namespace Medical
{
    public interface ProjectTemplate
    {
        void createProject(EditorResourceProvider resourceProvider, String projectName);

        String getDefaultFileName(String projectName);
    }
}
