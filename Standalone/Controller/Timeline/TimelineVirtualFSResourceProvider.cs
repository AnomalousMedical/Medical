﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

namespace Medical
{
    public class TimelineVirtualFSResourceProvider : TimelineResourceProvider
    {
        private String parentDirectory;
        private VirtualFileSystem virtualFileSystem;

        public TimelineVirtualFSResourceProvider(String parentDirectory)
        {
            this.parentDirectory = parentDirectory;
            virtualFileSystem = VirtualFileSystem.Instance;
        }

        public void Dispose()
        {
            
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

        public bool exists(string path)
        {
            return virtualFileSystem.exists(Path.Combine(parentDirectory, path));
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(parentDirectory, filename);
        }

        public TimelineResourceProvider clone()
        {
            return new TimelineVirtualFSResourceProvider(parentDirectory);
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
