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
                ResourceCache.forceCloseResourcesInDirectroy(oldPath);
            }
            else
            {
                ResourceCache.forceCloseResourceFile(oldPath);
            }
        }

        public void copy(string from, string to)
        {
            backingProvider.copy(from, to);
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

        public bool exists(string path)
        {
            return backingProvider.exists(path);
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

        public string BackingLocation
        {
            get
            {
                return backingProvider.BackingLocation;
            }
        }

        public ResourceCache ResourceCache { get; private set; }
    }
}