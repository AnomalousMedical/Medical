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
        /// <summary>
        /// Create a new project. Returns a string to the default file for the project. 
        /// You may also return null to signify that there is no default file and that 
        /// the project directory should be used instead.
        /// </summary>
        /// <param name="resourceProvider">The resource provider to use to make files.</param>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The default file for the project or null to use the project folder.</returns>
        String createProject(EditorResourceProvider resourceProvider, String projectName);
    }
}
