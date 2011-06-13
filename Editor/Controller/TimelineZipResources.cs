using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZipAccess;
using Logging;
using System.Xml;

namespace Medical
{
    public class TimelineZipResources : TimelineResourceProvider
    {
        private String resourceLocation;
        private ZipFile zipFile;

        public TimelineZipResources(String resourceLocation)
        {
            this.resourceLocation = resourceLocation;
            zipFile = new ZipFile(resourceLocation);
        }

        public void Dispose()
        {
            zipFile.Dispose();
        }

        public Stream openFile(string filename)
        {
            return zipFile.openFile(filename);
        }

        public void addStream(String filename, MemoryStream memoryStream)
        {
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                if (ionicZip.ContainsEntry(filename))
                {
                    ionicZip.UpdateEntry(filename, memoryStream);
                }
                else
                {
                    ionicZip.AddEntry(filename, memoryStream);
                }
                ionicZip.Save();
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public void addFile(String path)
        {
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                if (ionicZip.ContainsEntry(Path.GetFileName(path)))
                {
                    ionicZip.UpdateFile(path, "");
                }
                else
                {
                    ionicZip.AddFile(path, "");
                }
                ionicZip.Save();
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public String[] listFiles(String pattern)
        {
            try
            {
                List<ZipFileInfo> zipFiles = zipFile.listFiles("/", pattern, false);
                String[] ret = new String[zipFiles.Count];
                int i = 0;
                foreach (ZipFileInfo info in zipFiles)
                {
                    ret[i++] = info.FullName;
                }
                return ret;
            }
            catch (Exception ex)
            {
                Log.Error("Could not list files in directory {0}.\nReason: {1}", resourceLocation, ex.Message);
            }
            return new String[0];
        }

        public bool exists(String path)
        {
            return zipFile.exists(path);
        }

        public TimelineResourceProvider clone()
        {
            return new TimelineZipResources(resourceLocation);
        }

        public String BackingLocation
        {
            get
            {
                return resourceLocation;
            }
        }
    }
}
