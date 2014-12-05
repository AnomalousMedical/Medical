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

namespace Medical.Controller
{
    public partial class BookmarksController : IDisposable
    {
        public delegate void BookmarkDelegate(Bookmark bookmark);
        public delegate void BookmarkPathDelegate(BookmarkPath path);

        private const int MaxFileNameTries = 200;

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
        /// Called when premium features are activated.
        /// </summary>
        public event Action<BookmarksController> PremiumBookmarksChanged;

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

            saveBookmark(bookmark);

            fireBookmarkAdded(bookmark);

            return bookmark;
        }

        /// <summary>
        /// This function just saves a bookmark to disk, it does not fire the bookmark added event.
        /// </summary>
        /// <param name="bookmark"></param>
        public void saveBookmark(Bookmark bookmark)
        {
            if (bookmarksResourceProvider.CanWrite)
            {
                String directory = "";
                
                if (currentPath != null)
                {
                    directory = currentPath.Path;
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

        public void loadSavedBookmarks()
        {
            ThreadPool.QueueUserWorkItem(state =>
                {
                    if (premiumBookmarks)
                    {
                        if(!Directory.Exists(MedicalConfig.BookmarksFolder))
                        {
                            Directory.CreateDirectory(MedicalConfig.BookmarksFolder);
                        }
                        bookmarksResourceProvider = new FilesystemResourceProvider(MedicalConfig.BookmarksFolder);
                        loadBookmarksFoldersBgThread(new BookmarkPath()
                        {
                            Path = "",
                            DisplayName = "Bookmarks",
                            Parent = null
                        });
                    }
                    else if(NonPremiumBookmarksResourceProvider != null)
                    {
                        bookmarksResourceProvider = NonPremiumBookmarksResourceProvider;
                        loadBookmarksFoldersBgThread(new BookmarkPath()
                        {
                            Path = "",
                            DisplayName = "Bookmarks",
                            Parent = null
                        });
                    }
                });
        }

        public void clearBookmarks()
        {
            if(BookmarksCleared != null)
            {
                BookmarksCleared.Invoke();
            }
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
                    loadBookmarks.loadBookmarks(currentPath);
                }
            }
        }

        private void loadBookmarksFoldersBgThread(BookmarkPath path)
        {
            ThreadManager.invokeAndWait(() => fireBookmarkPathAdded(path));
            if(currentPath == null)
            {
                CurrentPath = path;
            }
            foreach(String directory in bookmarksResourceProvider.listDirectories("*", path.Path, false))
            {
                loadBookmarksFoldersBgThread(new BookmarkPath()
                    {
                        Path = directory,
                        DisplayName = Path.GetFileNameWithoutExtension(directory),
                        Parent = path
                    });
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
                if(premiumBookmarks != value)
                {
                    premiumBookmarks = value;
                    if(PremiumBookmarksChanged != null)
                    {
                        PremiumBookmarksChanged.Invoke(this);
                    }
                }
            }
        }

        public ResourceProvider NonPremiumBookmarksResourceProvider { get; set; }

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
    }
}
