using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;

namespace Medical.Controller
{
    public class BookmarksController
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        private StandaloneController standaloneController;

        public BookmarksController(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
        }

        public Bookmark createBookmark(String name)
        {
            LayerState layerState = new LayerState("");
            layerState.captureState();
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            return new Bookmark(name, window.Translation, window.LookAt, layerState);
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
            imageProperties.LayerState = bookmark.Layers;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;

            return imageRenderer.renderImage(imageProperties);
        }
    }
}
