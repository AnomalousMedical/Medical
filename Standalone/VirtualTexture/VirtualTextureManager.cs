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

            //fullBitmap = new FreeImageBitmap[3];
            //fullBitmapBox = new PixelBox[fullBitmap.Length];

            //width = 80;// mainWindow.WindowWidth / 10;
            //height = 60;// mainWindow.WindowHeight / 10;
            //OgreResourceGroupManager.getInstance().createResourceGroup("FeedbackBufferGroup");
            //texture = TextureManager.getInstance().createManual("FeedbackBuffer", "FeedbackBufferGroup", TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 0, OgrePlugin.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);

            //for (int i = 0; i < fullBitmap.Length; ++i)
            //{
            //    fullBitmap[i] = new FreeImageBitmap((int)texture.Value.Width, (int)texture.Value.Height, FreeImageAPI.PixelFormat.Format32bppRgb);
            //    fullBitmapBox[i] = fullBitmap[i].createPixelBox(OgrePlugin.PixelFormat.PF_A8R8G8B8);
            //}

            //pixelBuffer = texture.Value.getBuffer();
            //renderTexture = pixelBuffer.Value.getRenderTarget();
            //renderTexture.setAutoUpdated(false);
            //Viewport vp = renderTexture.addViewport(window.Camera);
            //vp.setMaterialScheme("FeedbackBuffer");
            //renderTexture.PostRenderTargetUpdate += () => Logging.Log.Debug("Finished rendering feedback buffer");
            //window.SchemeName = "FeedbackBuffer";


            //MaterialManager.getInstance().HandleSchemeNotFound += StandaloneController_HandleSchemeNotFound;
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


                    //PerformanceMonitor.start("FeedbackBuffer Copy");
                    //feedbackBuffers[currentReadbackTexture].copyFromGpu();
                    //currentReadbackTexture = (currentReadbackTexture + 1) % feedbackBuffers.Length;
                    //PerformanceMonitor.stop("FeedbackBuffer Copy");

                    //PerformanceMonitor.start("FeedbackBuffer Render");
                    //feedbackBuffers[currentRenderTexture].update();
                    //currentReadbackTexture = (currentRenderTexture + 1) % feedbackBuffers.Length;
                    //PerformanceMonitor.stop("FeedbackBuffer Render");

                    //if (liveThumbs == null)
                    //{
                    //    liveThumbs = new LiveThumbnailController("TestRender", new IntSize2(mainWindow.WindowWidth / 10, mainWindow.WindowHeight / 10), sceneViewController);
                    //    thumbHost = new SillyThumbHost();
                    //    liveThumbs.addThumbnailHost(thumbHost);
                    //    thumbHost.Layers = LayerState.CreateAndCapture();
                    //    thumbHost.LookAt = window.LookAt;
                    //    thumbHost.Translation = window.Translation;
                    //    liveThumbs.setVisibility(thumbHost, true);

                    //    //window.SchemeName = "FeedbackBuffer";

                    //    MaterialManager.getInstance().HandleSchemeNotFound += StandaloneController_HandleSchemeNotFound;
                    //}
                    //thumbHost.Layers = LayerState.CreateAndCapture();
                    //thumbHost.LookAt = window.LookAt;
                    //thumbHost.Translation = window.Translation;
                    //liveThumbs.updateCameraAndLayers(thumbHost);
                    //liveThumbs.updateAllThumbs();

                    allowImageRender = true;

                });
            }
            frameCount = (frameCount + 1) % updateBufferFrame;
        }
    }
}
