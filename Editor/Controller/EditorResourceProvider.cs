using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public class EditorResourceProvider : ResourceProvider, IDisposable
    {
        private String parentPath;
        private FileSystemWatcher fileWatcher;

        public EditorResourceProvider(String path)
        {
            this.parentPath = path;
            if (Directory.Exists(parentPath))
            {
                fileWatcher = new FileSystemWatcher(parentPath);
                fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                fileWatcher.EnableRaisingEvents = true;
            }
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
        }

        public Stream openFile(string filename)
        {
            return File.OpenRead(Path.Combine(parentPath, filename));
        }

        public void addStream(string filename, MemoryStream memoryStream)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(parentPath, filename), FileMode.Create))
            {
                memoryStream.WriteTo(fileStream);
            }
        }

        public void addFile(string path)
        {
            String filename = Path.GetFileName(path);
            File.Copy(path, Path.Combine(parentPath, filename), true);
        }

        public void deleteFile(String filename)
        {
            File.Delete(Path.Combine(parentPath, filename));
        }

        public string[] listFiles(string pattern)
        {
            String[] files = Directory.GetFiles(parentPath, pattern, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPath.Length + 1);
            }
            return files;
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            String[] files = Directory.GetFiles(Path.Combine(parentPath, directory), pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = files[i].Remove(0, parentPath.Length + 1);
            }
            return files;
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

        public string BackingLocation
        {
            get
            {
                return parentPath;
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //EditorTypeController typeController;
            //String extension = Path.GetExtension(e.FullPath);
            //if (typeControllers.TryGetValue(extension, out typeController))
            //{
            //    typeController.fileChanged(e, extension);
            //}
        }
    }
}
