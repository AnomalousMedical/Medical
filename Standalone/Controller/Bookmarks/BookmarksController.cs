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

namespace Medical.Controller
{
    public delegate void BookmarkDelegate(Bookmark bookmark);

    public class BookmarksController : IDisposable
    {
        public event BookmarkDelegate BookmarkAdded;

        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);
        private static XmlSaver xmlSaver = new XmlSaver();

        private StandaloneController standaloneController;
        private BookmarkDelegate mainThreadCallback;

        private bool cancelBackgroundLoading = false;

        public BookmarksController(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            mainThreadCallback = fireBookmarkAdded;
        }

        public void Dispose()
        {
            //Ensure any background threads are no longer running.
            cancelBackgroundLoading = true;
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

            using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.Default))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlSaver.saveObject(bookmark, xmlWriter);
            }

            if (BookmarkAdded != null)
            {
                BookmarkAdded.Invoke(bookmark);
            }

            return bookmark;
        }

        public void applyBookmark(Bookmark bookmark)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            window.setPosition(bookmark.CameraTranslation, bookmark.CameraLookAt);
            bookmark.Layers.apply(MedicalConfig.TransparencyChangeMultiplier);
        }

        public Bitmap createThumbnail(Bookmark bookmark)
        {
            ImageRenderer imageRenderer = standaloneController.ImageRenderer;
            ImageRendererProperties imageProperties = new ImageRendererProperties();
            imageProperties.Width = 50;
            imageProperties.Height = 50;
            imageProperties.UseWindowBackgroundColor = false;
            imageProperties.CustomBackgroundColor = BACK_COLOR;
            imageProperties.AntiAliasingMode = 2;
            imageProperties.TransparentBackground = true;
            imageProperties.UseActiveViewportLocation = false;
            imageProperties.UseCustomPosition = true;
            imageProperties.CameraLookAt = bookmark.CameraLookAt;
            imageProperties.CameraPosition = bookmark.CameraTranslation;
            imageProperties.OverrideLayers = true;
            imageProperties.LayerState = bookmark.Layers;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;

            return imageRenderer.renderImage(imageProperties);
        }

        public void loadSavedBookmarks()
        {
            Thread backgroundLoaderThread = new Thread(delegate()
                {
                    ensureBookmarksFolderExists();
                    String[] bookmarkFiles = Directory.GetFiles(MedicalConfig.BookmarksFolder);
                    foreach (String file in bookmarkFiles)
                    {
                        Bookmark bookmark;
                        using (XmlTextReader xmlReader = new XmlTextReader(file))
                        {
                            bookmark = (Bookmark)xmlSaver.restoreObject(xmlReader);
                        }
                        ThreadManager.invokeAndWait(mainThreadCallback, bookmark);
                        if (cancelBackgroundLoading)
                        {
                            return;
                        }
                    }
                });
            backgroundLoaderThread.Start();
        }

        private void fireBookmarkAdded(Bookmark bookmark)
        {

            if (BookmarkAdded != null)
            {
                BookmarkAdded.Invoke(bookmark);
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
