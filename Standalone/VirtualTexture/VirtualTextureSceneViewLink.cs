using Anomalous.GuiFramework.Cameras;
using Engine.ObjectManagement;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class VirtualTextureSceneViewLink
    {
        private Dictionary<SceneViewWindow, VirtualTextureManager> virtualTextures = new Dictionary<SceneViewWindow, VirtualTextureManager>(); //Need to dispose this
        private SceneViewController sceneViewController;

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            this.sceneViewController.WindowDestroyed += sceneViewController_WindowDestroyed;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
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

        void standaloneController_SceneLoaded(SimScene scene)
        {
            //Dumb but easy way to detect virtual textures, iterate over everything in the material manager
            foreach(Material material in MaterialManager.getInstance().Iterator)
            {
                int numTechniques = material.getNumTechniques();
                Technique technique = material.getTechnique((ushort)(numTechniques - 1));
                if (technique != null && technique.getSchemeName() == FeedbackBuffer.Scheme)
                {
                    String name = material.getName();
                    if (name.EndsWith("Alpha"))
                    {
                        name = name.Substring(0, name.Length - 5);
                    }
                    if (name.Contains("_HWSkin"))
                    {
                        name = name.Substring(0, name.LastIndexOf('_'));
                    }
                    virtualTextures.Values.First().processMaterialAdded(name, material.getTechnique(0));
                }
            }
        }
    }
}
