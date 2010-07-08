using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using OgrePlugin;

namespace Medical.Controller
{
    class BackgroundController
    {
        private ViewportBackground sourceBackground;
        private Dictionary<SceneViewWindow, ViewportBackground> backgrounds = new Dictionary<SceneViewWindow, ViewportBackground>();
        private OgreSceneManager currentSceneManager;

        public BackgroundController(ViewportBackground sourceBackground, SceneViewController sceneViewController)
        {
            this.sourceBackground = sourceBackground;

            sceneViewController.WindowCreated += new SceneViewWindowEvent(sceneViewController_WindowCreated);
            sceneViewController.WindowDestroyed += new SceneViewWindowEvent(sceneViewController_WindowDestroyed);
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

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            ViewportBackground newBackground = sourceBackground.clone(window.Name + "Background");
            backgrounds.Add(window, newBackground);
            window.FindVisibleObjects += newBackground.preRenderCallback;
        }

        void sceneViewController_WindowDestroyed(SceneViewWindow window)
        {
            ViewportBackground background = backgrounds[window];
            window.FindVisibleObjects -= background.preRenderCallback;
            backgrounds.Remove(window);
        }
    }
}
