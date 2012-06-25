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

namespace Medical
{
    public delegate void ResourceProviderFileEvent(String path, bool isDirectory);
    public delegate void ResourceProviderFileRenamedEvent(String path, String oldPath, bool isDirectory);
    public delegate void ResourceProviderFileDeletedEvent(String path);

    public class EditorResourceProvider : ResourceProvider, IDisposable
    {
        private static XmlSaver xmlSaver = new XmlSaver();
        private String parentPath;
        private int parentPathLength;
        private FileSystemWatcher fileWatcher;

        public event ResourceProviderFileEvent FileCreated;
        public event ResourceProviderFileEvent FileChanged;
        public event ResourceProviderFileDeletedEvent FileDeleted;
        public event ResourceProviderFileRenamedEvent FileRenamed;

        public EditorResourceProvider(String path)
        {
            ResourceCache = new ResourceCache();
            this.parentPath = path.Replace('\\', '/');
            parentPathLength = parentPath.Length;
            if (!parentPath.EndsWith("/"))
            {
                ++parentPathLength;
            }
            if (Directory.Exists(parentPath))
            {
                fileWatcher = new FileSystemWatcher(parentPath);
                fileWatcher.IncludeSubdirectories = true;
                fileWatcher.Created += new FileSystemEventHandler(fileWatcher_Created);
                fileWatcher.Deleted += new FileSystemEventHandler(fileWatcher_Deleted);
                fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                fileWatcher.Renamed += new RenamedEventHandler(fileWatcher_Renamed);
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
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

        public string[] listFiles(string pattern)
        {
            String[] files = Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            String[] files = Directory.GetFiles(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPathLength);
            }
            return files;
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            String[] directories = Directory.GetDirectories(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < directories.Length; ++i)
            {
                directories[i] = directories[i].Remove(0, parentPathLength);
            }
            return directories;
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

        public ResourceCache ResourceCache { get; private set; }

        void fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (FileRenamed != null)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(e.FullPath);
                    FileRenamed.Invoke(e.Name, e.OldName, (attr & FileAttributes.Directory) == FileAttributes.Directory);
                }
                catch (Exception) { }
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(e.FullPath);
                    FileChanged.Invoke(e.Name, (attr & FileAttributes.Directory) == FileAttributes.Directory);
                }
                catch (Exception) { }
            }
        }

        void fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (FileDeleted != null)
            {
                try
                {
                    FileDeleted.Invoke(e.Name);
                }
                catch (Exception) { }
            }
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(e.FullPath);
                    FileCreated.Invoke(e.Name, (attr & FileAttributes.Directory) == FileAttributes.Directory);
                }
                catch (Exception) { }
            }
        }
    }
}