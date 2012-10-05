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

        public Stream openWriteStream(String filename)
        {
            return new FileStream(Path.Combine(parentPath, filename), FileMode.Create, FileAccess.Write);
        }

        public void addFile(string path, string targetDirectory)
        {
            String filename = Path.GetFileName(path);
            File.Copy(path, Path.Combine(parentPath, targetDirectory, filename), true);
        }

        public void delete(String filename)
        {
            File.Delete(Path.Combine(parentPath, filename));
        }

        public IEnumerable<String> listFiles(string pattern)
        {
            foreach(String file in Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly))
            {
                yield return file.Remove(0, parentPathLength);
            }
        }

        public IEnumerable<String> listFiles(String pattern, String directory, bool recursive)
        {
            foreach(String file in Directory.GetFiles(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                yield return file.Remove(0, parentPathLength);
            }
        }

        public IEnumerable<String> listDirectories(String pattern, String directory, bool recursive)
        {
            foreach(String dir in Directory.GetDirectories(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                yield return dir.Remove(0, parentPathLength);
            }
        }

        public bool directoryHasEntries(String path)
        {
            return Directory.EnumerateFileSystemEntries(Path.Combine(parentPath, path), "*", SearchOption.AllDirectories).Any();
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

        public void createDirectory(string path, string directoryName)
        {
            Directory.CreateDirectory(Path.Combine(parentPath, path, directoryName));
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
