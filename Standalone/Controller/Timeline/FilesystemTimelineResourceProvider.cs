using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public class FilesystemTimelineResourceProvider : TimelineResourceProvider
    {
        private String parentPath;

        public FilesystemTimelineResourceProvider(String path)
        {
            this.parentPath = path;
        }

        public void Dispose()
        {
            
        }

        public Stream openFile(string filename)
        {
            return File.OpenRead(Path.Combine(parentPath, filename));
        }

        public void addStream(string filename, MemoryStream memoryStream)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(parentPath, filename), FileMode.Create))
            {
                memoryStream.WriteTo(fileStream);
            }
        }

        public void addFile(string path)
        {
            String filename = Path.GetFileName(path);
            File.Copy(path, Path.Combine(parentPath, filename), true);
        }

        public string[] listFiles(string pattern)
        {
            String[] files = Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPath.Length + 1);
            }
            return files;
        }

        public bool exists(string path)
        {
            return File.Exists(path);
        }

        public string BackingLocation
        {
            get
            {
                return parentPath;
            }
        }
    }
}
