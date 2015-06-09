using Anomalous.GuiFramework.Cameras;
using Engine;
using Engine.Threads;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class VirtualTextureManager : IDisposable
    {
        public const String ResourceGroup = "VirtualTextureGroup";

        FeedbackBuffer[] feedbackBuffers = new FeedbackBuffer[2];
        int frameCount = 0;
        int updateBufferFrame = 1;
        bool readbackThisFrame = false;
        int currentRenderTexture = 0;
        int currentReadbackTexture = 0;
        bool allowImageRender = true;

        Dictionary<String, PhysicalTexture> physicalTextures = new Dictionary<string, PhysicalTexture>();
        Dictionary<String, IndirectionTexture> indirectionTextures = new Dictionary<string, IndirectionTexture>();

        public VirtualTextureManager(SceneViewWindow window)
        {
            window.CameraCreated += window_CameraCreated;
            window.CameraDestroyed += window_CameraDestroyed;
            if (!OgreResourceGroupManager.getInstance().resourceGroupExists(VirtualTextureManager.ResourceGroup))
            {
                OgreResourceGroupManager.getInstance().createResourceGroup(VirtualTextureManager.ResourceGroup);
            }

            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i] = new FeedbackBuffer(window);
            }

            currentRenderTexture = feedbackBuffers.Length - 1; //Separate rendering from readback

            //Create physical textures
            physicalTextures.Add("NormalMap", new PhysicalTexture("NormalMap", new IntSize2(4096, 4096)));
            physicalTextures.Add("Diffuse", new PhysicalTexture("Diffuse", new IntSize2(4096, 4096)));
            physicalTextures.Add("Specular", new PhysicalTexture("Specular", new IntSize2(4096, 4096)));
            physicalTextures.Add("Opacity", new PhysicalTexture("Opacity", new IntSize2(4096, 4096)));
        }

        public void Dispose()
        {
            foreach(var physicalTexture in physicalTextures.Values)
            {
                physicalTexture.Dispose();
            }
            foreach (var indirectionTexture in indirectionTextures.Values)
            {
                indirectionTexture.Dispose();
            }
        }

        void window_CameraDestroyed(SceneViewWindow window)
        {
            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i].cameraDestroyed();
            }
        }

        void window_CameraCreated(SceneViewWindow window)
        {
            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i].cameraCreated();
            }
        }

        public void update(SceneViewWindow window, bool currentCameraRender)
        {
            if (allowImageRender && frameCount == 0)
            {
                ThreadManager.invoke(() =>
                {
                    allowImageRender = false;

                    //if (readbackThisFrame)
                    {
                        PerformanceMonitor.start("FeedbackBuffer Copy");
                        feedbackBuffers[currentReadbackTexture].copyFromGpu();
                        currentReadbackTexture = (currentReadbackTexture + 1) % feedbackBuffers.Length;
                        PerformanceMonitor.stop("FeedbackBuffer Copy");
                        readbackThisFrame = false;
                    }
                    //else
                    {
                        PerformanceMonitor.start("FeedbackBuffer Render");
                        feedbackBuffers[currentRenderTexture].update();
                        currentRenderTexture = (currentRenderTexture + 1) % feedbackBuffers.Length;
                        PerformanceMonitor.stop("FeedbackBuffer Render");
                        readbackThisFrame = true;
                    }

                    allowImageRender = true;

                });
            }
            frameCount = (frameCount + 1) % updateBufferFrame;
        }

        public void processMaterialAdded(String materialSetKey, Technique technique)
        {
            IndirectionTexture indirectionTex;
            if (!indirectionTextures.TryGetValue(materialSetKey, out indirectionTex))
            {
                indirectionTex = new IndirectionTexture(new IntSize2(2048, 2048), 128, this); //Don't hardcode size
                indirectionTextures.Add(materialSetKey, indirectionTex);
            }
            indirectionTex.reconfigureTechnique(technique);
        }

        public void processMaterialRemoved(Object materialSetKey)
        {

        }

        internal PhysicalTexture getPhysicalTexture(string name)
        {
            return physicalTextures[name];
        }
    }
}
