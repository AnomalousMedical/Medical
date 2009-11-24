using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using OgrePlugin;

namespace Medical
{
    public class BackgroundController
    {
        private ViewportBackground sourceBackground;
        private Dictionary<DrawingWindow, ViewportBackground> backgrounds = new Dictionary<DrawingWindow, ViewportBackground>();
        private OgreSceneManager currentSceneManager;

        public BackgroundController(ViewportBackground sourceBackground, DrawingWindowController drawingWindowController)
        {
            this.sourceBackground = sourceBackground;

            drawingWindowController.WindowCreated += new DrawingWindowEvent(drawingWindowController_WindowCreated);
            drawingWindowController.WindowDestroyed += new DrawingWindowEvent(drawingWindowController_WindowDestroyed);
        }

        public void sceneLoaded(OgreSceneManager sceneManager)
        {
            currentSceneManager = sceneManager;
            foreach (ViewportBackground background in backgrounds.Values)
            {
                background.createBackground(sceneManager);
            }
        }

        public void sceneUnloading()
        {
            foreach (ViewportBackground background in backgrounds.Values)
            {
                background.destroyBackground();
            }
            currentSceneManager = null;
        }

        void drawingWindowController_WindowCreated(DrawingWindow window)
        {
            ViewportBackground newBackground = sourceBackground.clone(window.CameraName + "Background");
            backgrounds.Add(window, newBackground);
            newBackground.createBackground(currentSceneManager);
            window.PreFindVisibleObjects += newBackground.preRenderCallback;
        }

        void drawingWindowController_WindowDestroyed(DrawingWindow window)
        {
            ViewportBackground background = backgrounds[window];
            background.destroyBackground();
            window.PreFindVisibleObjects -= background.preRenderCallback;
            backgrounds.Remove(window);
        }
    }
}
