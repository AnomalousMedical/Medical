using Medical.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class EditorFilesystemWatcher : IDisposable
    {
        private FileSystemWatcher fileWatcher;
        private FileSystemWatcher directoryWatcher;

        public event ResourceProviderFileEvent FileCreated;
        public event ResourceProviderFileEvent FileChanged;
        public event ResourceProviderFileDeletedEvent FileDeleted;
        public event ResourceProviderFileRenamedEvent FileRenamed;

        private EditorResourceProvider editorResourceProvider;

        public EditorFilesystemWatcher(EditorResourceProvider editorResourceProvider)
        {
            this.editorResourceProvider = editorResourceProvider;

            fileWatcher = new FileSystemWatcher(editorResourceProvider.BackingLocation);
            fileWatcher.SynchronizingObject = new ThreadManagerSynchronizeInvoke();
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            fileWatcher.Created += new FileSystemEventHandler(fileWatcher_Created);
            fileWatcher.Deleted += new FileSystemEventHandler(fileWatcher_Deleted);
            fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
            fileWatcher.Renamed += new RenamedEventHandler(fileWatcher_Renamed);
            fileWatcher.EnableRaisingEvents = true;

            directoryWatcher = new FileSystemWatcher(editorResourceProvider.BackingLocation);
            directoryWatcher.SynchronizingObject = new ThreadManagerSynchronizeInvoke();
            directoryWatcher.IncludeSubdirectories = true;
            directoryWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
            directoryWatcher.Created += new FileSystemEventHandler(directoryWatcher_Created);
            directoryWatcher.Deleted += new FileSystemEventHandler(directoryWatcher_Deleted);
            directoryWatcher.Changed += new FileSystemEventHandler(directoryWatcher_Changed);
            directoryWatcher.Renamed += new RenamedEventHandler(directoryWatcher_Renamed);
            directoryWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            fileWatcher.Dispose();
            directoryWatcher.Dispose();
        }

        void fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            editorResourceProvider.ResourceCache.forceCloseResourceFile(e.OldName);
            if (FileRenamed != null)
            {
                FileRenamed.Invoke(e.Name, e.OldName, false);
            }
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                FileChanged.Invoke(e.Name, false);
            }
        }

        void fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            editorResourceProvider.ResourceCache.forceCloseResourceFile(e.FullPath);
            if (FileDeleted != null)
            {
                FileDeleted.Invoke(e.Name);
            }
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                FileCreated.Invoke(e.Name, false);
            }
        }

        void directoryWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            editorResourceProvider.ResourceCache.forceCloseResourcesInDirectroy(e.OldName);
            if (FileRenamed != null)
            {
                FileRenamed.Invoke(e.Name, e.OldName, true);
            }
        }

        void directoryWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (FileChanged != null)
            {
                FileChanged.Invoke(e.Name, true);
            }
        }

        void directoryWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            editorResourceProvider.ResourceCache.forceCloseResourcesInDirectroy(e.FullPath);
            if (FileDeleted != null)
            {
                FileDeleted.Invoke(e.Name);
            }
        }

        void directoryWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (FileCreated != null)
            {
                FileCreated.Invoke(e.Name, true);
            }
        }
    }
}
