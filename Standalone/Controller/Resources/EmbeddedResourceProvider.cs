using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Medical
{
    public class EmbeddedResourceProvider : ResourceProvider
    {
        private Assembly assembly;
        private String baseResourceString;
        private List<String> fileList = new List<string>();

        public EmbeddedResourceProvider(Assembly assembly, String baseResourceString)
        {
            this.assembly = assembly;
            this.baseResourceString = baseResourceString;

            String[] fileList = assembly.GetManifestResourceNames();
            foreach (String file in fileList)
            {
                if (file.StartsWith(baseResourceString))
                {
                    this.fileList.Add(file.Remove(0, baseResourceString.Length));
                }
            }
        }

        public Stream openFile(string filename)
        {
            return assembly.GetManifestResourceStream(baseResourceString + filename);
        }

        public void addStream(String filename, MemoryStream memoryStream)
        {
            throw new NotImplementedException("addStream not supported by this resource provider.");
        }

        public void addFile(String path)
        {
            throw new NotImplementedException("addFile not supported by this resource provider.");
        }

        public void deleteFile(String filename)
        {
            throw new NotImplementedException("deleteFile not supported by this resource provider.");
        }

        public string[] listFiles(string pattern)
        {
            return fileList.ToArray();
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            List<String> files = new List<string>();
            directory = directory.Replace('\\', '/').Replace('/', '.');
            foreach (String file in fileList)
            {
                if (file.StartsWith(directory))
                {
                    files.Add(file);
                }
            }
            return files.ToArray();
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            return new String[0];
        }

        public bool exists(string path)
        {
            return fileList.Contains(path);
        }

        public String getFullFilePath(String filename)
        {
            return baseResourceString + filename;
        }

        public string BackingLocation
        {
            get
            {
                return baseResourceString;
            }
        }
    }
}
