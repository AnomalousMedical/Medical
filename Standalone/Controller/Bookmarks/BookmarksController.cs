using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.Threading;
using MyGUIPlugin;
using System.IO;
using Engine.Saving;
using Engine.Threads;
using Anomalous.GuiFramework.Cameras;

namespace Medical.Controller
{
    public partial class BookmarksController : IDisposable
    {
        public delegate void BookmarkDelegate(Bookmark bookmark);
        public delegate void BookmarkPathDelegate(BookmarkPath path);

        private const int MaxFileNameTries = 200;
        private const String TrashFolderName = "Trash";
        private const string TrashFolderNameLower = "trash";

        private static XmlSaver xmlSaver = new XmlSaver();

        /// <summary>
        /// Called when a bookmark is added.
        /// </summary>
        public event BookmarkDelegate BookmarkAdded;

        /// <summary>
        /// Called when a bookmark is removed.
        /// </summary>
        public event BookmarkDelegate BookmarkRemoved;

        /// <summary>
        /// Called when a bookmark path is added.
        /// </summary>
        public event BookmarkPathDelegate BookmarkPathAdded;

        /// <summary>
        /// Called when a bookmark path is removed.
        /// </summary>
        public event BookmarkPathDelegate BookmarkPathRemoved;

        /// <summary>
        /// Called when the currently loaded bookmarks are cleared.
        /// </summary>
        public event Action BookmarksCleared;

        /// <summary>
        /// Called when the currently loaded bookmark paths are cleared.
        /// </summary>
        public event Action BookmarkPathsCleared;

        /// <summary>
        /// Called when premium features are activated.
        /// </summary>
        public event Action<BookmarksController> PremiumBookmarksChanged;

        /// <summary>
        /// Called when the current path changes.
        /// </summary>
        public event Action<BookmarkPath> CurrentPathChanged;

        private StandaloneController standaloneController;
        private ResourceProvider bookmarksResourceProvider;
        private BookmarkPath currentPath;

        private bool premiumBookmarks;
        private LoadBookmarksBgTask loadBookmarks;

        public BookmarksController(StandaloneController standaloneController, int width, int height, bool premiumBookmarks)
        {
            this.standaloneController = standaloneController;
            this.premiumBookmarks = premiumBookmarks;
            this.loadBookmarks = new LoadBookmarksBgTask(this);
            SceneViewControllerExtensions.BookmarksController = this;
        }

        public void Dispose()
        {
            //Ensure any background threads are no longer running.
            loadBookmarks.cancel();
        }

        public Bookmark createBookmark(String name)
        {
            LayerState layerState = new LayerState();
            layerState.captureState();
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            Bookmark bookmark = new Bookmark(name, window.Translation, window.LookAt, layerState);

            saveBookmark(bookmark, currentPath, bookmarksResourceProvider);

            fireBookmarkAdded(bookmark);

            return bookmark;
        }

        public void deleteBookmark(Bookmark bookmark)
        {
            fireBookmarkRemoved(bookmark);
            if (bookmarksResourceProvider.CanWrite)
            {
                try
                {
                    bookmarksResourceProvider.delete(bookmark.BackingFile);
                }
                catch (Exception)
                {
                    MessageBox.show(String.Format("Could not delete bookmark '{0}'", bookmark.Name), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            }
        }

        public void moveBookmark(BookmarkPath path, Bookmark bookmark)
        {
            if(path != currentPath)
            {
                Bookmark copy = CopySaver.Default.copy(bookmark);
                copy.BookmarkPath = path;
                saveBookmark(copy, path, bookmarksResourceProvider);
                deleteBookmark(bookmark);
            }
        }

        public void addFolder(String name)
        {
            if (bookmarksResourceProvider.CanWrite && name.ToLowerInvariant() != TrashFolderNameLower)
            {
                String path = name;
                if (currentPath != null)
                {
                    path = Path.Combine(currentPath.Path, path);
                }
                if (!bookmarksResourceProvider.directoryExists(path))
                {
                    bookmarksResourceProvider.createDirectory(path);

                    fireBookmarkPathAdded(new BookmarkPath()
                    {
                        DisplayName = name,
                        Parent = currentPath,
                        Path = path
                    });
                }
            }
        }

        public void removeFolder(BookmarkPath path)
        {
            if (bookmarksResourceProvider.CanWrite && path.Parent != null)
            {
                if (bookmarksResourceProvider.directoryExists(path.Path))
                {
                    bookmarksResourceProvider.delete(path.Path);

                    if (path == CurrentPath)
                    {
                        CurrentPath = path.Parent;
                    }

                    fireBookmarkPathRemoved(path);
                }
            }
        }

        public void emptyTrash()
        {
            if (bookmarksResourceProvider.CanWrite)
            {
                bookmarksResourceProvider.delete(TrashFolderName);
                bookmarksResourceProvider.createDirectory(TrashFolderName);
                if(CurrentPath.IsTrash)
                {
                    if(BookmarksCleared != null)
                    {
                        BookmarksCleared.Invoke();
                    }
                }
            }
        }

        public void applyBookmark(Bookmark bookmark)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            LayerState undoLayers = LayerState.CreateAndCapture();
            CameraPosition undoCameraPosition = window.createCameraPosition();
            window.setPosition(bookmark.CameraPosition, MedicalConfig.CameraTransitionTime);
            bookmark.Layers.timedApply(MedicalConfig.CameraTransitionTime);
            standaloneController.LayerController.pushUndoState(undoLayers);
            window.pushUndoState(undoCameraPosition);
        }

        public void loadSavedBookmarks(ResourceProvider resourceProvider)
        {
            bookmarksResourceProvider = resourceProvider;
            ThreadPool.QueueUserWorkItem(state =>
                {
                    loadBookmarksFoldersBgThread(new BookmarkPath()
                    {
                        Path = "",
                        DisplayName = "Bookmarks",
                        Parent = null
                    });
                    if(bookmarksResourceProvider.CanWrite)
                    {
                        loadBookmarksFoldersBgThread(new BookmarkPath()
                        {
                            Path = TrashFolderName,
                            DisplayName = TrashFolderName,
                            Parent = null,
                            IsTrash = true
                        });
                    }
                });
        }

        public void clearBookmarks()
        {
            if (BookmarksCleared != null)
            {
                BookmarksCleared.Invoke();
            }
        }

        public void clearBookmarkPaths()
        {
            currentPath = null;
            if (BookmarkPathsCleared != null)
            {
                BookmarkPathsCleared.Invoke();
            }
        }

        public Bookmark loadBookmark(String bookmarkPath)
        {
            try
            {
                if (bookmarksResourceProvider != null && bookmarksResourceProvider.fileExists(bookmarkPath))
                {
                    using (var stream = bookmarksResourceProvider.openFile(bookmarkPath))
                    {
                        return SharedXmlSaver.Load<Bookmark>(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} loading bookmark '{1}'. Message: {2}", ex.GetType().Name, bookmarkPath, ex.Message);
            }
            return null;
        }

        public BookmarkPath CurrentPath
        {
            get
            {
                return currentPath;
            }
            set
            {
                if (currentPath != value)
                {
                    currentPath = value;
                    if (CurrentPathChanged != null)
                    {
                        CurrentPathChanged.Invoke(currentPath);
                    }
                    if (currentPath != null)
                    {
                        loadBookmarks.loadBookmarks(currentPath);
                    }
                }
            }
        }

        private void loadBookmarksFoldersBgThread(BookmarkPath path)
        {
            if (bookmarksResourceProvider.CanWrite && !bookmarksResourceProvider.directoryExists(path.Path))
            {
                bookmarksResourceProvider.createDirectory(path.Path);
            }

            ThreadManager.invokeAndWait(() => fireBookmarkPathAdded(path));
            if (currentPath == null)
            {
                CurrentPath = path;
            }
            foreach (String directory in bookmarksResourceProvider.listDirectories("*", path.Path, false))
            {
                if (directory.ToLowerInvariant() != TrashFolderNameLower)
                {
                    loadBookmarksFoldersBgThread(new BookmarkPath()
                        {
                            Path = directory,
                            DisplayName = Path.GetFileNameWithoutExtension(directory),
                            Parent = path
                        });
                }
            }
        }

        public bool PremiumBookmarks
        {
            get
            {
                return premiumBookmarks;
            }
            set
            {
                if (premiumBookmarks != value)
                {
                    premiumBookmarks = value;
                    if (PremiumBookmarksChanged != null)
                    {
                        PremiumBookmarksChanged.Invoke(this);
                    }
                }
            }
        }

        private void fireBookmarkAdded(Bookmark bookmark)
        {
            if (BookmarkAdded != null)
            {
                BookmarkAdded.Invoke(bookmark);
            }
        }

        private void fireBookmarkRemoved(Bookmark bookmark)
        {
            if (BookmarkRemoved != null)
            {
                BookmarkRemoved.Invoke(bookmark);
            }
        }

        private void fireBookmarkPathAdded(BookmarkPath path)
        {
            if (BookmarkPathAdded != null)
            {
                BookmarkPathAdded.Invoke(path);
            }
        }

        private void fireBookmarkPathRemoved(BookmarkPath path)
        {
            if (BookmarkPathRemoved != null)
            {
                BookmarkPathRemoved.Invoke(path);
            }
        }

        /// <summary>
        /// This function just saves a bookmark to disk, it does not fire the bookmark added event.
        /// </summary>
        /// <param name="bookmark"></param>
        private static void saveBookmark(Bookmark bookmark, BookmarkPath path, ResourceProvider bookmarksResourceProvider)
        {
            if (bookmarksResourceProvider.CanWrite)
            {
                String directory = "";

                if (path != null)
                {
                    directory = path.Path;
                }

                if (!bookmarksResourceProvider.directoryExists(directory))
                {
                    bookmarksResourceProvider.createDirectory(directory);
                }

                String fileFormat = Path.Combine(directory, bookmark.Name + "{0}.bmk");
                String filename = String.Format(fileFormat, "");

                int index = 0;
                int tries = 0;
                while (bookmarksResourceProvider.fileExists(filename) && tries < MaxFileNameTries)
                {
                    filename = String.Format(fileFormat, (++index).ToString());
                }
                if (tries == MaxFileNameTries)
                {
                    filename = Path.Combine(MedicalConfig.BookmarksFolder, Guid.NewGuid().ToString() + ".bmk");
                }

                bookmark.BackingFile = filename;

                using (Stream stream = bookmarksResourceProvider.openWriteStream(filename))
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(stream, Encoding.Unicode))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlSaver.saveObject(bookmark, xmlWriter);
                    }
                }
            }
        }
    }
}
