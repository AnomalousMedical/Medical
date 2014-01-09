using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    public interface ProjectType
    {
        /// <summary>
        /// Delete the project from the system.
        /// </summary>
        /// <param name="name"></param>
        void deleteProject(String name);

        /// <summary>
        /// Create the projects base files if they do not exist.
        /// </summary>
        /// <param name="name"></param>
        void ensureProjectExists(String name);

        /// <summary>
        /// Open a project, returning an appropriate resource provider. ResourceProviderClosed will be called
        /// when this resource provider is no longer needed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ResourceProvider openResourceProvider(String name);

        /// <summary>
        /// The extension for this project type.
        /// </summary>
        String Extension { get; }

        /// <summary>
        /// Return true if the project exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool doesProjectExist(string name);

        /// <summary>
        /// Get the base path for the project given a file in it.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string getProjectBasePath(string name);

        /// <summary>
        /// Called when a resource provider given by this class closes and is no longer being used. Can dispose here if needed.
        /// </summary>
        /// <param name="resourceProvider"></param>
        void resourceProviderClosed(ResourceProvider resourceProvider);
    }
}
