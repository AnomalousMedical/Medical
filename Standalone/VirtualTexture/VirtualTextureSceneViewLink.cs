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
        VirtualTextureManager virtualTexture;
        private SceneViewController sceneViewController;

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            virtualTexture = new VirtualTextureManager(window);
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
        }

        void window_RenderingStarted(SceneViewWindow window, bool currentCameraRender)
        {
            virtualTexture.update(window, currentCameraRender);
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
                    virtualTexture.processMaterialAdded(name, material.getTechnique(0));
                }
            }
        }
    }
}
