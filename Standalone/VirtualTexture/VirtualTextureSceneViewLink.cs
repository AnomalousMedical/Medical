using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.ObjectManagement;
using Engine.Platform;
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

            virtualTextureManager = new VirtualTextureManager(UnifiedMaterialBuilder.GetNumCompressedTexturesNeeded(textureFormat), MedicalConfig.PhysicalTextureSize, MedicalConfig.PageSize, 4096, textureFormat, MedicalConfig.VirtualTextureStagingBufferCount, MedicalConfig.FeedbackBufferSize, MedicalConfig.TextureCacheSize, UnifiedMaterialBuilder.AreTexturesPagedOnDisk(textureFormat));
            virtualTextureManager.MaxStagingUploadPerFrame = MedicalConfig.MaxStagingVirtualTextureUploadsPerFrame;
            virtualTextureManager.TransparentFeedbackBufferVisibilityMask = TransparencyController.TransparentVisibilityMask;
            virtualTextureManager.OpaqueFeedbackBufferVisibilityMask = TransparencyController.OpaqueVisibilityMask;
            virtualTextureManager.MipSampleBias = -3;
            virtualTextureManager.AutoAdjustMipLevel = false;

            materialBuilder = new UnifiedMaterialBuilder(virtualTextureManager, OgreInterface.Instance.SelectedTextureFormat, standaloneController.MedicalController.PluginManager.createLiveResourceManager("UnifiedShaders"));
            OgreInterface.Instance.MaterialParser.addMaterialBuilder(materialBuilder);
            TransparencyController.initialize(materialBuilder);

            standaloneController.MainWindow.DestroyInternalResources += MainWindow_DestroyInternalResources;
            standaloneController.MainWindow.CreateInternalResources += MainWindow_CreateInternalResources;
        }

        public void Dispose()
        {
            standaloneController.MainWindow.DestroyInternalResources -= MainWindow_DestroyInternalResources;
            standaloneController.MainWindow.CreateInternalResources -= MainWindow_CreateInternalResources;
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

        private void MainWindow_DestroyInternalResources(OSWindow window, InternalResourceType resourceType)
        {
            if ((resourceType & InternalResourceType.Graphics) == InternalResourceType.Graphics)
            {
                virtualTextureManager.suspend();
            }
        }

        private void MainWindow_CreateInternalResources(OSWindow window, InternalResourceType resourceType)
        {
            if ((resourceType & InternalResourceType.Graphics) == InternalResourceType.Graphics)
            {
                virtualTextureManager.resume();
            }
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
