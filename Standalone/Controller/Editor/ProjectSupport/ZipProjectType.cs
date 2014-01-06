using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    public class ZipProjectType : ProjectType
    {
        public ZipProjectType(String extension)
        {
            this.Extension = extension;
        }

        public void deleteProject(string name)
        {
            File.Delete(name);
        }

        public void ensureProjectExists(string name)
        {
            if (!File.Exists(name))
            {
                using (ZipFile zf = new ZipFile(name))
                {
                    zf.Save();
                }
            }
        }

        public ResourceProvider openProject(string name)
        {
            return new ZipResourceProvider(name);
        }

        public bool doesProjectExist(string name)
        {
            return File.Exists(name);
        }

        public string getProjectBasePath(string name)
        {
            return name;
        }

        public void resourceProviderClosed(ResourceProvider resourceProvider)
        {
            ((ZipResourceProvider)resourceProvider).Dispose();
        }

        public string Extension { get; set; }
    }
}
