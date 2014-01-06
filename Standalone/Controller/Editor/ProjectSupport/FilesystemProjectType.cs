using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    class FilesystemProjectType : ProjectType
    {
        public void deleteProject(string name)
        {
            Directory.Delete(name, true);
        }

        public void ensureProjectExists(string name)
        {
            if (!Directory.Exists(name))
            {
                Directory.CreateDirectory(name);
            }
        }

        public ResourceProvider openProject(string name)
        {
            return new FilesystemResourceProvider(name);
        }

        public bool doesProjectExist(string name)
        {
            return Directory.Exists(name);
        }

        public string Extension
        {
            get
            {
                return "";
            }
        }
    }
}
