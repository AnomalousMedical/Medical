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
    public class VirtualTextureSceneViewLink : IDisposable, FeedbackCameraPositioner
    {
        private VirtualTextureManager virtualTextureManager;
        private SceneViewController sceneViewController;
        private StandaloneController standaloneController;
        private UnifiedMaterialBuilder materialBuilder;

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.sceneViewController = standaloneController.SceneViewController;
            this.standaloneController = standaloneController;

            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            CompressedTextureSupport textureFormat = OgreInterface.Instance.SelectedTextureFormat;
            int padding;
            switch(textureFormat)
            {
                case CompressedTextureSupport.DXT:
                    padding = 4;
                    break;
                default:
                    padding = 1;
                    break;
            }

            virtualTextureManager = new VirtualTextureManager(4, new IntSize2(4096, 4096), 128, textureFormat, padding, 10);

            materialBuilder = new UnifiedMaterialBuilder(virtualTextureManager, OgreInterface.Instance.SelectedTextureFormat);
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
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            IDisposableUtil.DisposeIfNotNull(virtualTextureManager);
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

        void standaloneController_SceneUnloading(SimScene scene)
        {
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            virtualTextureManager.destroyFeedbackBufferCamera(scene);
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            virtualTextureManager.createFeedbackBufferCamera(scene, this, new IntSize2(128, 128));
            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
        }

        public Vector3 Translation
        {
            get
            {
                return sceneViewController.ActiveWindow.Translation;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return sceneViewController.ActiveWindow.LookAt;
            }
        }
    }
}
