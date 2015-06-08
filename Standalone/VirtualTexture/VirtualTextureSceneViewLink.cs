using Anomalous.GuiFramework.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class VirtualTextureSceneViewLink
    {
        private Dictionary<SceneViewWindow, VirtualTextureManager> virtualTextures = new Dictionary<SceneViewWindow, VirtualTextureManager>();
        private SceneViewController sceneViewController;

        public VirtualTextureSceneViewLink(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            this.sceneViewController.WindowDestroyed += sceneViewController_WindowDestroyed;
        }

        void sceneViewController_WindowDestroyed(SceneViewWindow window)
        {
            virtualTextures.Remove(window);
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            window.RenderingStarted += window_RenderingStarted;
            virtualTextures.Add(window, new VirtualTextureManager(window));
        }

        void window_RenderingStarted(SceneViewWindow window, bool currentCameraRender)
        {
            virtualTextures[window].update(window, currentCameraRender);
        }
    }
}
