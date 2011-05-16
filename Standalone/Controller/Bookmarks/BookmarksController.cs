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

        private ImageRendererProperties imageProperties;
        private ImageAtlas imageAtlas = new ImageAtlas("Bookmarks", new Size2(50, 50), new Size2(512, 512));

        private StandaloneController standaloneController;
        private BookmarkDelegate mainThreadCallback;

        private bool cancelBackgroundLoading = false;

        public BookmarksController(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            mainThreadCallback = fireBookmarkAdded;

            imageProperties = new ImageRendererProperties();
            imageProperties.Width = 50;
            imageProperties.Height = 50;
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

        public String createThumbnail(Bookmark bookmark)
        {
            if (imageAtlas.containsImage(bookmark))
            {
                return imageAtlas.getImageId(bookmark);
            }
            else
            {
                ImageRenderer imageRenderer = standaloneController.ImageRenderer;

                imageProperties.CameraLookAt = bookmark.CameraLookAt;
                imageProperties.CameraPosition = bookmark.CameraTranslation;
                imageProperties.LayerState = bookmark.Layers;

                using (Bitmap thumb = imageRenderer.renderImage(imageProperties))
                {
                    return imageAtlas.addImage(bookmark, thumb);
                }
            }
        }

        public void loadSavedBookmarks()
        {
            Thread backgroundLoaderThread = new Thread(delegate()
                {
                    ensureBookmarksFolderExists();
                    String[] bookmarkFiles = Directory.GetFiles(MedicalConfig.BookmarksFolder, "*.bmk");
                    foreach (String file in bookmarkFiles)
                    {
                        Bookmark bookmark;
                        using (XmlTextReader xmlReader = new XmlTextReader(file))
                        {
                            bookmark = (Bookmark)xmlSaver.restoreObject(xmlReader);
                        }
                        if (bookmark != null)
                        {
                            ThreadManager.invokeAndWait(mainThreadCallback, bookmark);
                        }
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
