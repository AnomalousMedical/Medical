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

            virtualTextureManager = new VirtualTextureManager(4);

            materialBuilder = new UnifiedMaterialBuilder(virtualTextureManager);
            OgreInterface.Instance.MaterialParser.addMaterialBuilder(materialBuilder);
            materialBuilder.InitializationComplete += materialBuilder_InitializationComplete;
        }

        void materialBuilder_InitializationComplete(UnifiedMaterialBuilder obj)
        {
            if (obj.MaterialCount > 0)
            {
                materialBuilder.InitializationComplete -= materialBuilder_InitializationComplete;
                virtualTextureManager.update();
            }
        }

        public void Dispose()
        {
            //OgreInterface.Instance.MaterialParser.removeMaterialBuilder(materialBuilder); //Don't do this for now, it makes this leaky but need to figure out order
            this.sceneViewController.WindowCreated -= sceneViewController_WindowCreated;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            IDisposableUtil.DisposeIfNotNull(virtualTextureManager);
        }

        void sceneViewController_WindowCreated(SceneViewWindow window) //Only works for the first window
        {
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

            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
        }

        void window_CameraDestroyed(SceneViewWindow window)
        {
            VirtualTextureManager.destroyFeedbackBufferCamera();

            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
        }

        void MedicalController_OnLoopUpdate(Engine.Platform.Clock time)
        {
            virtualTextureManager.update();
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
