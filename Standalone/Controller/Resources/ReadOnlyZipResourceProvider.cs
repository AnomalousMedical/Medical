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
    /// <summary>
    /// This resource provider will read files from a zipped file. Don't forget to dispose it.
    /// </summary>
    public class ReadOnlyZipResourceProvider : ResourceProvider, IDisposable
    {
        private String resourceLocation;
        private ZipFile zipFile;

        public ReadOnlyZipResourceProvider(String resourceLocation)
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

        public Stream openWriteStream(String filename)
        {
            throw new NotImplementedException("stream writing not supported by this resource provider.");
        }

        public void addFile(String path, string targetDirectory)
        {
            throw new NotImplementedException("addFile not supported by this resource provider.");
        }

        public void delete(String filename)
        {
            throw new NotImplementedException("deleteFile not supported by this resource provider.");
        }

        public IEnumerable<String> listFiles(String pattern)
        {
            IEnumerable<ZipFileInfo> zipFiles = null;
            try
            {
                zipFiles = zipFile.listFiles("/", pattern, false);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list files in directory {0}.\nReason: {1}", resourceLocation, ex.Message);
            }
            if (zipFiles != null)
            {
                foreach (ZipFileInfo info in zipFiles)
                {
                    yield return info.FullName;
                }
            }
        }

        public IEnumerable<String> listFiles(String pattern, String directory, bool recursive)
        {
            IEnumerable<ZipFileInfo> zipFiles = null;
            try
            {
                zipFiles = zipFile.listFiles(Path.Combine(resourceLocation, directory), pattern, recursive);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list files in directory {0}.\nReason: {1}", Path.Combine(resourceLocation, directory), ex.Message);
            }
            if (zipFiles != null)
            {
                foreach (ZipFileInfo info in zipFiles)
                {
                    yield return info.FullName;
                }
            }
        }

        public IEnumerable<String> listDirectories(String pattern, String directory, bool recursive)
        {
            IEnumerable<ZipFileInfo> zipDirs = null;
            try
            {
                zipDirs = zipFile.listDirectories(Path.Combine(resourceLocation, directory), pattern, recursive);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list directories in directory {0}.\nReason: {1}", Path.Combine(resourceLocation, directory), ex.Message);
            }
            if (zipDirs != null)
            {
                foreach (ZipFileInfo info in zipDirs)
                {
                    yield return info.FullName;
                }
            }
        }

        public bool directoryHasEntries(String path)
        {
            return listFiles("*", path, true).Any() || listDirectories("*", path, true).Any();
        }

        public bool exists(String path)
        {
            return zipFile.exists(path);
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(resourceLocation, filename);
        }

        public void createDirectory(string path, string directoryName)
        {
            throw new NotImplementedException("Cannot create directories in the ReadOnlyZipResourceProvider");
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
