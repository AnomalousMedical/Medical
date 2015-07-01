using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class VirtualTextureSceneViewLink : IDisposable
    {
        private VirtualTextureManager virtualTextureManager;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;
        private UnifiedMaterialBuilder materialBuilder;

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.sceneViewController.WindowCreated += sceneViewController_WindowCreated;
            this.standaloneController = standaloneController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;

            virtualTextureManager = new VirtualTextureManager();

            materialBuilder = new UnifiedMaterialBuilder(virtualTextureManager);
            OgreInterface.Instance.MaterialParser.addMaterialBuilder(materialBuilder);
        }

        public void Dispose()
        {
            //OgreInterface.Instance.MaterialParser.removeMaterialBuilder(materialBuilder); //Don't do this for now, it makes this leaky but need to figure out order
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            IDisposableUtil.DisposeIfNotNull(virtualTextureManager);
        }

        void sceneViewController_WindowCreated(SceneViewWindow window) //Only works for the first window
        {
            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
            window.CameraCreated += window_CameraCreated;
            window.CameraDestroyed += window_CameraDestroyed;
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
        }

        void window_CameraCreated(SceneViewWindow window)
        {
            int width = window.RenderWidth / 10;
            if (width < 10)
            {
                width = 10;
            }
            int height = window.RenderHeight / 10;
            if (height < 10)
            {
                height = 10;
            }

            VirtualTextureManager.createFeedbackBufferCamera(window.Camera, new IntSize2(width, height));
        }

        void window_CameraDestroyed(SceneViewWindow window)
        {
            VirtualTextureManager.destroyFeedbackBufferCamera();
        }

        void MedicalController_OnLoopUpdate(Engine.Platform.Clock time)
        {
            virtualTextureManager.update();
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            //Dumb but easy way to detect virtual textures, iterate over everything in the material manager
            foreach(Material material in MaterialManager.getInstance().Iterator.Where(m => !materialBuilder.isCreator(m)))
            {
                if(material.Name == "SkinMaterial_HWSkin")
                {
                    Logging.Log.Debug("");
                }

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
                    int techniqueCount = material.getNumTechniques();
                    if(techniqueCount > 0)
                    {
                        Technique feedbackTechnique;
                        for (int i = techniqueCount; i > 0; --i )
                        {
                            feedbackTechnique = material.getTechnique((ushort)(i - 1)); //The last one is the most likely feedbackTechnique
                            if(feedbackTechnique.SchemeName == FeedbackBuffer.Scheme)
                            {
                                virtualTextureManager.processMaterialAdded(name, material.getTechnique(0), feedbackTechnique);
                            }
                        }
                    }
                }
            }
        }

        public VirtualTextureManager VirtualTextureManager
        {
            get
            {
                return virtualTextureManager;
            }
        }
    }
}
