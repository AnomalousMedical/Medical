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

        public ResourceProvider openProject(String name)
        {
            return getInfo(name).openProject(name);
        }

        public bool doesProjectExist(string name)
        {
            return getInfo(name).doesProjectExist(name);
        }

        private ProjectType getInfo(String name)
        {
            try
            {
                ProjectType ret;
                if (infos.TryGetValue(Path.GetExtension(name), out ret))
                {
                    return ret;
                }
            }
            catch (Exception) { }
            return defaultProjectType;
        }
    }
}
