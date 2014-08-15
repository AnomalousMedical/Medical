using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;
using System.Threading;
using MyGUIPlugin;

namespace Medical.Controller
{
    public delegate void BookmarkDelegate(Bookmark bookmark);

    public class BookmarksController : IDisposable
    {
        private const int MaxFileNameTries = 200;

        private static XmlSaver xmlSaver = new XmlSaver();

        public event BookmarkDelegate BookmarkAdded;
        public event BookmarkDelegate BookmarkRemoved;
        public event Action<BookmarksController> PremiumBookmarksChanged;

        private StandaloneController standaloneController;
        private BookmarkDelegate mainThreadCallback;

        private bool cancelBackgroundLoading = false;
        private bool premiumBookmarks;

        public BookmarksController(StandaloneController standaloneController, int width, int height, bool premiumBookmarks)
        {
            this.standaloneController = standaloneController;
            mainThreadCallback = fireBookmarkAdded;
            this.premiumBookmarks = premiumBookmarks;
        }

        public void Dispose()
        {
            //Ensure any background threads are no longer running.
            cancelBackgroundLoading = true;
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
            ensureBookmarksFolderExists();

            String fileFormat = Path.Combine(MedicalConfig.BookmarksFolder, bookmark.Name + "{0}.bmk");
            String filename = String.Format(fileFormat, "");

            int index = 0;
            int tries = 0;
            while (File.Exists(filename) && tries < MaxFileNameTries)
            {
                filename = String.Format(fileFormat, (++index).ToString());
            }
            if(tries == MaxFileNameTries)
            {
                filename = Path.Combine(MedicalConfig.BookmarksFolder, Guid.NewGuid().ToString() + ".bmk");
            }

            bookmark.BackingFile = filename;

            using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.Unicode))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(bookmark, xmlWriter);
            }
        }

        public void removeBookmark(Bookmark bookmark)
        {
            fireBookmarkRemoved(bookmark);
            if (bookmark.BackingFile != null)
            {
                try
                {
                    File.Delete(bookmark.BackingFile);
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
            window.setPosition(bookmark.CameraPosition, MedicalConfig.CameraTransitionTime);
            bookmark.Layers.timedApply(MedicalConfig.CameraTransitionTime);
        }

        public void loadSavedBookmarks()
        {
            ThreadPool.QueueUserWorkItem(state =>
                {
                    Thread.Sleep(1000);
                    if (premiumBookmarks)
                    {
                        ensureBookmarksFolderExists();
                        readBookmarkFilesBgThread(new FilesystemResourceProvider(MedicalConfig.BookmarksFolder), true);
                    }
                    else if(NonPremiumBookmarksResourceProvider != null)
                    {
                        readBookmarkFilesBgThread(NonPremiumBookmarksResourceProvider, false);
                    }
                });
        }

        private void readBookmarkFilesBgThread(ResourceProvider bookmarkResourceProvider, bool saveFilePath)
        {
            foreach (String file in bookmarkResourceProvider.listFiles("*.bmk"))
            {
                Bookmark bookmark;
                using (XmlTextReader xmlReader = new XmlTextReader(bookmarkResourceProvider.openFile(file)))
                {
                    try
                    {
                        bookmark = xmlSaver.restoreObject(xmlReader) as Bookmark;
                    }
                    catch(Exception ex)
                    {
                        bookmark = null;
                        Logging.Log.Error("{0} loading bookmark '{1}'. Skipping this bookmark. Message: {2}", ex.GetType().Name, file, ex.Message);
                    }
                }
                if (bookmark != null)
                {
                    if (saveFilePath)
                    {
                        bookmark.BackingFile = bookmarkResourceProvider.getFullFilePath(file);
                    }
                    ThreadManager.invokeAndWait(mainThreadCallback, bookmark);
                    Thread.Sleep(50);
                }
                if (cancelBackgroundLoading)
                {
                    return;
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

        private void ensureBookmarksFolderExists()
        {
            if (!Directory.Exists(MedicalConfig.BookmarksFolder))
            {
                Directory.CreateDirectory(MedicalConfig.BookmarksFolder);
            }
        }
    }
}
