using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;
using MyGUIPlugin;
using Logging;
using Medical.Controller;

namespace Medical
{
    public class EditorResourceProvider : ResourceProvider
    {
        private ResourceProvider backingProvider;

        public EditorResourceProvider(ResourceProvider backingProvider)
        {
            ResourceCache = new ResourceCache();
            this.backingProvider = backingProvider;
        }

        public Stream openFile(string filename)
        {
            CachedResource cachedResource = ResourceCache[filename];
            if (cachedResource != null)
            {
                return cachedResource.openStream();
            }
            else
            {
                return backingProvider.openFile(filename);
            }
        }

        public String readFileAsString(String filename)
        {
            if (fileExists(filename))
            {
                using (StreamReader stringReader = new StreamReader(openFile(filename)))
                {
                    return stringReader.ReadToEnd();
                }
            }
            return null;
        }

        public Stream openWriteStream(String filename)
        {
            return backingProvider.openWriteStream(filename);
        }

        public void addFile(string path, string targetDirectory)
        {
            String filename = Path.GetFileName(path);
            String destinationPath = Path.Combine(targetDirectory, filename);
            backingProvider.addFile(path, targetDirectory);
            CachedResource cachedResource = ResourceCache[destinationPath];
            if (cachedResource != null)
            {
                cachedResource.AllowClose = true;
                ResourceCache.closeResource(path);
            }
        }

        public void delete(String filename)
        {
            bool wasDir = backingProvider.isDirectory(filename);
            backingProvider.delete(filename);
            if (wasDir)
            {
                ResourceCache.forceCloseResourcesInDirectroy(filename);
            }
            else
            {
                ResourceCache.forceCloseResourceFile(filename);
            }
        }

        public void move(String oldPath, String newPath)
        {
            bool wasDir = backingProvider.isDirectory(oldPath);
            backingProvider.move(oldPath, newPath);
            if (wasDir)
            {
                copyCachedResourcesToBackingProvider(ResourceCache.getResourcesInDirectory(oldPath), oldPath, newPath);
                ResourceCache.forceCloseResourcesInDirectroy(oldPath);
            }
            else
            {
                var cachedResource = ResourceCache[oldPath];
                if (cachedResource != null)
                {
                    using (Stream writeStream = backingProvider.openWriteStream(newPath))
                    {
                        using (Stream readStream = cachedResource.openStream())
                        {
                            readStream.CopyTo(writeStream);
                        }
                    }
                }
                ResourceCache.forceCloseResourceFile(oldPath);
            }
        }

        public void copyFile(string from, string to)
        {
            var cachedResource = ResourceCache[from];
            if (cachedResource == null)
            {
                backingProvider.copyFile(from, to);
            }
            else
            {
                using (Stream writeStream = backingProvider.openWriteStream(to))
                {
                    using (Stream readStream = cachedResource.openStream())
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }

        public void copyDirectory(string from, string to)
        {
            backingProvider.copyDirectory(from, to);
            copyCachedResourcesToBackingProvider(ResourceCache.getResourcesInDirectory(from), from, to);
        }

        public IEnumerable<String> listFiles(string pattern)
        {
            return backingProvider.listFiles(pattern);
        }

        public IEnumerable<String> listFiles(String pattern, String directory, bool recursive)
        {
            return backingProvider.listFiles(pattern, directory, recursive);
        }

        public IEnumerable<String> listDirectories(String pattern, String directory, bool recursive)
        {
            return backingProvider.listDirectories(pattern, directory, recursive);
        }

        public bool directoryHasEntries(String path)
        {
            return backingProvider.directoryHasEntries(path);
        }

        public bool fileExists(string path)
        {
            return ResourceCache[path] != null || backingProvider.fileExists(path);
        }

        public bool directoryExists(string path)
        {
            return backingProvider.directoryExists(path);
        }

        public bool exists(String path)
        {
            return ResourceCache[path] != null || backingProvider.exists(path);
        }

        public String getFullFilePath(String filename)
        {
            return backingProvider.getFullFilePath(filename);
        }

        public void createDirectory(string path, string directoryName)
        {
            backingProvider.createDirectory(path, directoryName);
        }

        public bool isDirectory(String path)
        {
            return backingProvider.isDirectory(path);
        }

        public void cloneProviderTo(String destination)
        {
            backingProvider.cloneProviderTo(destination);
        }

        public void saveAllCachedResources()
        {
            foreach (CachedResource resource in ResourceCache.Resources)
            {
                resource.save();
            }
        }

        public string BackingLocation
        {
            get
            {
                return backingProvider.BackingLocation;
            }
        }

        public ResourceProvider BackingProvider
        {
            get
            {
                return backingProvider;
            }
            internal set
            {
                backingProvider = value;
            }
        }

        public ResourceCache ResourceCache { get; private set; }

        /// <summary>
        /// Given a list of files copy any files in the cache from that list to the destination directory.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="destinationDir"></param>
        private void copyCachedResourcesToBackingProvider(IEnumerable<CachedResource> files, String baseFromPath, string destinationDir)
        {
            int basePathLength = baseFromPath.Length;
            if (!(baseFromPath.EndsWith("/") || baseFromPath.EndsWith("\\")))
            {
                ++basePathLength;
            }
            foreach (var cachedResource in files)
            {
                if (cachedResource != null)
                {
                    String toFile = Path.Combine(destinationDir, cachedResource.File.Substring(basePathLength));
                    using (Stream writeStream = backingProvider.openWriteStream(toFile))
                    {
                        using (Stream readStream = cachedResource.openStream())
                        {
                            readStream.CopyTo(writeStream);
                        }
                    }
                }
            }
        }
    }
}