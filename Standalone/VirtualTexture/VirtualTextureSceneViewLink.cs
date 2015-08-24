using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using OgrePlugin.VirtualTexture;
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
        private StandaloneController standaloneController;
        private UnifiedMaterialBuilder materialBuilder;
        private CameraLink cameraLink;

        public VirtualTextureSceneViewLink(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            cameraLink = new CameraLink(standaloneController.SceneViewController);

            CompressedTextureSupport textureFormat = OgreInterface.Instance.SelectedTextureFormat;

            virtualTextureManager = new VirtualTextureManager(4, MedicalConfig.PhysicalTextureSize, MedicalConfig.PageSize, textureFormat, MedicalConfig.VirtualTextureStagingBufferCount, MedicalConfig.FeedbackBufferSize, MedicalConfig.TextureCacheSize);
            virtualTextureManager.MaxStagingUploadPerFrame = MedicalConfig.MaxStagingVirtualTextureUploadsPerFrame;
            virtualTextureManager.TransparentFeedbackBufferVisibilityMask = TransparencyController.TransparentVisibilityMask;
            virtualTextureManager.OpaqueFeedbackBufferVisibilityMask = TransparencyController.OpaqueVisibilityMask;
            virtualTextureManager.MipSampleBias = -3;

            materialBuilder = new UnifiedMaterialBuilder(virtualTextureManager, OgreInterface.Instance.SelectedTextureFormat, standaloneController.MedicalController.PluginManager.createLiveResourceManager("UnifiedShaders"));
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
            OgreInterface.Instance.MaterialParser.removeMaterialBuilder(materialBuilder);
            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
            standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            IDisposableUtil.DisposeIfNotNull(virtualTextureManager);
            materialBuilder.Dispose();
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
            virtualTextureManager.createFeedbackBufferCamera(scene, cameraLink);
            standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
        }

        class CameraLink : FeedbackCameraPositioner
        {
            SceneViewController sceneViewController;

            public CameraLink(SceneViewController sceneViewController)
            {
                this.sceneViewController = sceneViewController;
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

            public void preRender()
            {
                TransparencyController.applyTransparencyState(sceneViewController.ActiveWindow.CurrentTransparencyState);
            }
        }
    }
}
