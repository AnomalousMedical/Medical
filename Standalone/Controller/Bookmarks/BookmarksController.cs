using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    public class BookmarksController
    {
        private StandaloneController standaloneController;

        public BookmarksController(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
        }

        public Bookmark createBookmark()
        {
            LayerState layerState = new LayerState("");
            layerState.captureState();
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            return new Bookmark(window.Translation, window.LookAt, layerState);
        }

        public void applyBookmark(Bookmark bookmark)
        {
            SceneViewWindow window = standaloneController.SceneViewController.ActiveWindow;
            window.setPosition(bookmark.CameraTranslation, bookmark.CameraLookAt);
            bookmark.Layers.apply(MedicalConfig.TransparencyChangeMultiplier);
        }
    }
}
