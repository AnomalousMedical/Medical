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
        private String parentDirectory;
        private VirtualFileSystem virtualFileSystem;

        public VirtualFilesystemResourceProvider(String parentDirectory)
        {
            this.parentDirectory = parentDirectory;
            virtualFileSystem = VirtualFileSystem.Instance;
        }

        public Stream openFile(string filename)
        {
            return virtualFileSystem.openStream(Path.Combine(parentDirectory, filename), Engine.Resources.FileMode.Open);
        }

        public void addStream(string filename, System.IO.MemoryStream memoryStream)
        {
            throw new NotImplementedException("Cannot add files to the VirtualFSResourceProvider");
        }

        public void addFile(string path)
        {
            throw new NotImplementedException("Cannot add files to the VirtualFSResourceProvider");
        }

        public void deleteFile(String filename)
        {
            throw new NotImplementedException("Cannot delete files in the VirtualFSResourceProvider");
        }

        public string[] listFiles(string pattern)
        {
            return virtualFileSystem.listFiles(parentDirectory, pattern, false);
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            return virtualFileSystem.listFiles(Path.Combine(parentDirectory, directory), pattern, recursive);
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            return virtualFileSystem.listDirectories(Path.Combine(parentDirectory, directory), pattern, recursive);
        }

        public bool exists(string path)
        {
            return virtualFileSystem.exists(Path.Combine(parentDirectory, path));
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(parentDirectory, filename);
        }

        public string BackingLocation
        {
            get
            {
                return parentDirectory;
            }
        }
    }
}
