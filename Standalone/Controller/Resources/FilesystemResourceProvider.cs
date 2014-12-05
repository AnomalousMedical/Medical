﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public class FilesystemResourceProvider : ResourceProvider
    {
        private String parentPath;
        private int parentPathLength;

        public FilesystemResourceProvider(String path)
        {
            this.parentPath = path.Replace('\\', '/');
            parentPathLength = parentPath.Length;
            if (!parentPath.EndsWith("/"))
            {
                ++parentPathLength;
            }
        }

        public Stream openFile(string filename)
        {
            return File.OpenRead(Path.Combine(parentPath, filename));
        }

        public Stream openWriteStream(String filename)
        {
            return new FileStream(Path.Combine(parentPath, filename), FileMode.Create, FileAccess.Write);
        }

        public void addFile(string path, string targetDirectory)
        {
            String filename = Path.GetFileName(path);
            File.Copy(path, Path.Combine(parentPath, targetDirectory, filename), true);
        }

        public void delete(String filename)
        {
            String path = Path.Combine(parentPath, filename);
            FileAttributes attrs = File.GetAttributes(path);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
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

        public bool fileExists(string path)
        {
            if (!path.StartsWith(parentPath))
            {
                path = Path.Combine(parentPath, path);
            }
            return File.Exists(path);
        }

        public bool directoryExists(string path)
        {
            if (!path.StartsWith(parentPath))
            {
                path = Path.Combine(parentPath, path);
            }
            return Directory.Exists(path);
        }

        public bool exists(String path)
        {
            return fileExists(path) || directoryExists(path);
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(parentPath, filename);
        }

        public void createDirectory(String fullPath)
        {
            Directory.CreateDirectory(Path.Combine(parentPath, fullPath));
        }

        public void createDirectory(string path, string directoryName)
        {
            createDirectory(Path.Combine(path, directoryName));
        }

        public bool isDirectory(String path)
        {
            path = Path.Combine(parentPath, path);
            FileAttributes attrs = File.GetAttributes(path);
            return (attrs & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public string BackingLocation
        {
            get
            {
                return parentPath;
            }
        }

        public void move(string oldPath, string newPath)
        {
            String fullOldPath = Path.Combine(parentPath, oldPath);
            String fullNewPath = Path.Combine(parentPath, newPath);
            FileAttributes attrs = File.GetAttributes(fullOldPath);
            if ((attrs & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Move(fullOldPath, fullNewPath);
            }
            else
            {
                File.Move(fullOldPath, fullNewPath);
            }
        }

        public void copyFile(string from, string to)
        {
            String fullFromPath = Path.Combine(parentPath, from);
            String fullToPath = Path.Combine(parentPath, to);
            File.Copy(fullFromPath, fullToPath, true);
        }

        public void copyDirectory(string from, string to)
        {
            String fullFromPath = Path.Combine(parentPath, from);
            String fullToPath = Path.Combine(parentPath, to);
            DirectoryExtensions.Copy(fullFromPath, fullToPath, true);
        }

        public void cloneProviderTo(String destination)
        {
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
            DirectoryExtensions.Copy(BackingLocation, destination, true);
        }

        public bool CanWrite
        {
            get
            {
                return true;
            }
        }
    }
}
