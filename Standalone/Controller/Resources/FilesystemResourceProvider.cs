using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public class FilesystemResourceProvider : ResourceProvider
    {
        private String parentPath;
        private int parentPathLength;

        public FilesystemResourceProvider(String path)
        {
            this.parentPath = path.Replace('\\', '/');
            parentPathLength = parentPath.Length;
            if (!parentPath.EndsWith("/"))
            {
                ++parentPathLength;
            }
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

        public void deleteFile(String filename)
        {
            File.Delete(Path.Combine(parentPath, filename));
        }

        public string[] listFiles(string pattern)
        {
            String[] files = Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            String[] files = Directory.GetFiles(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            String[] directories = Directory.GetDirectories(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < directories.Length; ++i)
            {
                directories[i] = directories[i].Remove(0, parentPathLength);
            }
            return directories;
        }

        public bool exists(string path)
        {
            if (!path.StartsWith(parentPath))
            {
                path = Path.Combine(parentPath, path);
            }
            return File.Exists(path);
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(parentPath, filename);
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
