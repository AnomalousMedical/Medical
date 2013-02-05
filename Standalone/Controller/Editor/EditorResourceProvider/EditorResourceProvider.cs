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
    public delegate void ResourceProviderFileEvent(String path, bool isDirectory);
    public delegate void ResourceProviderFileRenamedEvent(String path, String oldPath, bool isDirectory);
    public delegate void ResourceProviderFileDeletedEvent(String path);

    public class EditorResourceProvider : ResourceProvider
    {
        private static XmlSaver xmlSaver = new XmlSaver();
        private String parentPath;
        private int parentPathLength;

        public EditorResourceProvider(String path)
        {
            ResourceCache = new ResourceCache();
            this.parentPath = path.Replace('\\', '/');
            parentPathLength = parentPath.Length;
            if (!parentPath.EndsWith("/"))
            {
                ++parentPathLength;
            }
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
                return File.OpenRead(Path.Combine(parentPath, filename));
            }
        }

        public Stream openWriteStream(String filename)
        {
            return new FileStream(Path.Combine(parentPath, filename), FileMode.Create, FileAccess.Write);
        }

        public void addFile(string path, string targetDirectory)
        {
            String filename = Path.GetFileName(path);
            String destinationPath = Path.Combine(parentPath, targetDirectory, filename);
            File.Copy(path, destinationPath, true);
            CachedResource cachedResource = ResourceCache[destinationPath];
            if (cachedResource != null)
            {
                cachedResource.AllowClose = true;
                ResourceCache.closeResource(path);
            }
        }

        public void delete(String filename)
        {
            String path = Path.Combine(parentPath, filename);
            FileAttributes attrs = File.GetAttributes(path);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
                ResourceCache.forceCloseResourcesInDirectroy(path);
            }
            else
            {
                File.Delete(path);
                ResourceCache.forceCloseResourceFile(filename);
            }
        }

        public void move(String oldPath, String newPath)
        {
            String fullOldPath = Path.Combine(parentPath, oldPath);
            String fullNewPath = Path.Combine(parentPath, newPath);
            FileAttributes attrs = File.GetAttributes(fullOldPath);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Move(fullOldPath, fullNewPath);
                ResourceCache.forceCloseResourcesInDirectroy(oldPath);
            }
            else
            {
                File.Move(fullOldPath, fullNewPath);
                ResourceCache.forceCloseResourceFile(oldPath);
            }
        }

        public void copy(string from, string to)
        {
            String fullFromPath = Path.Combine(parentPath, from);
            String fullToPath = Path.Combine(parentPath, to);
            FileAttributes attrs = File.GetAttributes(fullFromPath);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryExtensions.Copy(fullFromPath, fullToPath, true);
            }
            else
            {
                File.Copy(fullFromPath, fullToPath, true);
            }
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
            return File.Exists(path) || Directory.Exists(path);
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

        public ResourceCache ResourceCache { get; private set; }
    }
}