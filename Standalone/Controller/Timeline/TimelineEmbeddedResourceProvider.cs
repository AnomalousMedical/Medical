﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Medical
{
    public class TimelineEmbeddedResourceProvider : TimelineResourceProvider
    {
        private Assembly assembly;
        private String baseResourceString;
        private List<String> fileList = new List<string>();

        public TimelineEmbeddedResourceProvider(Assembly assembly, String baseResourceString)
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

        public void Dispose()
        {
            fileList.Clear();
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

        public string[] listFiles(string pattern)
        {
            return fileList.ToArray();
        }

        public bool exists(string path)
        {
            return fileList.Contains(path);
        }

        public TimelineResourceProvider clone()
        {
            return new TimelineEmbeddedResourceProvider(assembly, baseResourceString);
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
