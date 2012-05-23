using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

namespace Medical
{
    public class VirtualFilesystemResourceProvider : ResourceProvider
    {
        private String parentPath;
        private int parentPathLength;
        private VirtualFileSystem virtualFileSystem;

        public VirtualFilesystemResourceProvider(String path)
        {
            this.parentPath = path.Replace('\\', '/');
            parentPathLength = parentPath.Length;
            if (!parentPath.EndsWith("/"))
            {
                ++parentPathLength;
            }
            virtualFileSystem = VirtualFileSystem.Instance;
        }

        public Stream openFile(string filename)
        {
            return virtualFileSystem.openStream(Path.Combine(parentPath, filename), Engine.Resources.FileMode.Open);
        }

        public Stream openWriteStream(String filename)
        {
            throw new NotImplementedException("stream writing not supported by this resource provider.");
        }

        public void addFile(string path, string targetDirectory)
        {
            throw new NotImplementedException("Cannot add files to the VirtualFSResourceProvider");
        }

        public void delete(String filename)
        {
            throw new NotImplementedException("Cannot delete files in the VirtualFSResourceProvider");
        }

        public string[] listFiles(string pattern)
        {
            String[] files = virtualFileSystem.listFiles(parentPath, pattern, false);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            String[] files = virtualFileSystem.listFiles(Path.Combine(parentPath, directory), pattern, recursive);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            String[] files = virtualFileSystem.listDirectories(Path.Combine(parentPath, directory), pattern, recursive);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public bool exists(string path)
        {
            return virtualFileSystem.exists(Path.Combine(parentPath, path));
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
