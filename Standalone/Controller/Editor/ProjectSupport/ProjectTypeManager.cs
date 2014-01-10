using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    public class ProjectTypeManager
    {
        private static readonly FilesystemProjectType defaultProjectType = new FilesystemProjectType();

        private Dictionary<String, ProjectType> infos = new Dictionary<string, ProjectType>();

        public void addInfo(ProjectType info)
        {
            infos.Add(info.Extension, info);
        }

        public void deleteProject(String name)
        {
            getInfo(name).deleteProject(name);
        }

        public void ensureProjectExists(String name)
        {
            getInfo(name).ensureProjectExists(name);
        }

        public ResourceProvider openResourceProvider(String name)
        {
            return getInfo(name).openResourceProvider(name);
        }

        public bool doesProjectExist(string name)
        {
            return getInfo(name).doesProjectExist(name);
        }

        public string getProjectBasePath(string name)
        {
            return getInfo(name).getProjectBasePath(name);
        }

        public void resourceProviderClosed(ResourceProvider resourceProvider)
        {
            getInfo(resourceProvider.BackingLocation).resourceProviderClosed(resourceProvider);
        }

        private ProjectType getInfo(String name)
        {
            try
            {
                ProjectType ret;
                String ext = Path.GetExtension(name);
                if (!String.IsNullOrEmpty(ext))
                {
                    if (infos.TryGetValue(ext.ToLowerInvariant(), out ret))
                    {
                        return ret;
                    }
                }
            }
            catch (Exception) { }
            return defaultProjectType;
        }

        public bool areSameProjectType(String location1, string location2)
        {
            return getInfo(location1) == getInfo(location2);
        }
    }
}
