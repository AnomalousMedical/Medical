using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;
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
        private static XmlSaver xmlSaver = new XmlSaver();

        public event BookmarkDelegate BookmarkAdded;
        public event BookmarkDelegate BookmarkRemoved;
        public event Action<BookmarksController> PremiumBookmarksChanged;

        private ImageRendererProperties imageProperties;
        private ImageAtlas imageAtlas;

        private StandaloneController standaloneController;
        private BookmarkDelegate mainThreadCallback;

        private bool cancelBackgroundLoading = false;
        private bool premiumBookmarks;

        public BookmarksController(StandaloneController standaloneController, int width, int height, bool premiumBookmarks)
        {
            this.standaloneController = standaloneController;
            mainThreadCallback = fireBookmarkAdded;
            this.premiumBookmarks = premiumBookmarks;

            imageAtlas = new ImageAtlas("Bookmarks", new IntSize2(width, height));

            imageProperties = new ImageRendererProperties();
            imageProperties.Width = width;
            imageProperties.Height = height;
            imageProperties.UseWindowBackgroundColor = false;
            imageProperties.CustomBackgroundColor = new Engine.Color(.94f, .94f, .94f);
            imageProperties.AntiAliasingMode = 2;
            imageProperties.TransparentBackground = true;
            imageProperties.UseActiveViewportLocation = false;
            imageProperties.OverrideLayers = true;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;
        }

        public void Dispose()
        {
            //Ensure any background threads are no longer running.
            cancelBackgroundLoading = true;
            imageAtlas.Dispose();
        }

        public Bookmark createBookmark(String name)
        {
            LayerState layerState = new LayerState("");
            layerState.captureState();
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            Bookmark bookmark = new Bookmark(name, window.Translation, window.LookAt, layerState);

            ensureBookmarksFolderExists();

            String fileFormat = Path.Combine(MedicalConfig.BookmarksFolder, name + "{0}.bmk");
            String filename = String.Format(fileFormat, "");

            int index = 0;
            while (File.Exists(filename))
            {
                filename = String.Format(fileFormat, (++index).ToString());
            }

            bookmark.BackingFile = filename;

            using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.Unicode))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(bookmark, xmlWriter);
            }

            fireBookmarkAdded(bookmark);

            return bookmark;
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

        public String createThumbnail(Bookmark bookmark)
        {
            if (imageAtlas.containsImage(bookmark))
            {
                return imageAtlas.getImageId(bookmark);
            }
            else
            {
                ImageRenderer imageRenderer = standaloneController.ImageRenderer;

                imageProperties.CameraLookAt = bookmark.CameraPosition.LookAt;
                imageProperties.CameraPosition = bookmark.CameraPosition.Translation;
                imageProperties.LayerState = bookmark.Layers;

                using (Bitmap thumb = imageRenderer.renderImage(imageProperties))
                {
                    return imageAtlas.addImage(bookmark, thumb);
                }
            }
        }

        public void loadSavedBookmarks()
        {
            ThreadPool.QueueUserWorkItem(state =>
                {
                    Thread.Sleep(1000);
                    if (premiumBookmarks)
                    {
                        ensureBookmarksFolderExists();
                        readBookmarkFilesBgThread(new FilesystemResourceProvider(MedicalConfig.BookmarksFolder));
                    }
                    else if(NonPremiumBookmarksResourceProvider != null)
                    {
                        readBookmarkFilesBgThread(NonPremiumBookmarksResourceProvider);
                    }
                });
        }

        private void readBookmarkFilesBgThread(ResourceProvider bookmarkResourceProvider)
        {
            foreach (String file in bookmarkResourceProvider.listFiles("*.bmk"))
            {
                Bookmark bookmark;
                using (XmlTextReader xmlReader = new XmlTextReader(bookmarkResourceProvider.openFile(file)))
                {
                    bookmark = (Bookmark)xmlSaver.restoreObject(xmlReader);
                    bookmark.BackingFile = file;
                }
                if (bookmark != null)
                {
                    ThreadManager.invokeAndWait(mainThreadCallback, bookmark);
                    Thread.Sleep(50);
                }
                if (cancelBackgroundLoading)
                {
                    return;
                }
            }
        }

        public int BookmarkWidth
        {
            get
            {
                return imageProperties.Width;
            }
        }

        public int BookmarkHeight
        {
            get
            {
                return imageProperties.Height;
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
