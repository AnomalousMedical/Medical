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
    class VirtualTextureManager
    {
        FeedbackBuffer[] feedbackBuffers = new FeedbackBuffer[2];
        int frameCount = 0;
        int updateBufferFrame = 5;
        bool readbackThisFrame = false;
        int currentRenderTexture = 0;
        int currentReadbackTexture = 0;
        bool allowImageRender = true;

        public VirtualTextureManager(SceneViewWindow window)
        {
            window.CameraCreated += window_CameraCreated;
            window.CameraDestroyed += window_CameraDestroyed;
            if (!OgreResourceGroupManager.getInstance().resourceGroupExists("FeedbackBufferGroup"))
            {
                OgreResourceGroupManager.getInstance().createResourceGroup("FeedbackBufferGroup");
            }

            for (int i = 0; i < feedbackBuffers.Length; ++i)
            {
                feedbackBuffers[i] = new FeedbackBuffer(window);
            }

            currentRenderTexture = feedbackBuffers.Length - 1; //Separate rendering from readback
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

                    if (readbackThisFrame)
                    {
                        PerformanceMonitor.start("FeedbackBuffer Copy");
                        feedbackBuffers[currentReadbackTexture].copyFromGpu();
                        currentReadbackTexture = (currentReadbackTexture + 1) % feedbackBuffers.Length;
                        PerformanceMonitor.stop("FeedbackBuffer Copy");
                        readbackThisFrame = false;
                    }
                    else
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
    }
}
