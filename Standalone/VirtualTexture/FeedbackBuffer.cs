using Anomalous.GuiFramework.Cameras;
using Engine;
using FreeImageAPI;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class FeedbackBuffer
    {
        public const String Scheme = "FeedbackBuffer";

        FreeImageBitmap fullBitmap;
        PixelBox fullBitmapBox;
        TexturePtr texture;
        RenderTexture renderTexture;
        HardwarePixelBufferSharedPtr pixelBuffer;
        VirtualTextureManager virtualTextureManager;

        SceneViewWindow window;
        Viewport vp;

        public FeedbackBuffer(SceneViewWindow window, VirtualTextureManager virtualTextureManager)
        {
            this.window = window;
            this.virtualTextureManager = virtualTextureManager;
        }

        public void update()
        {
            PerformanceMonitor.start("FeedbackBuffer Render");
            renderTexture.update();
            PerformanceMonitor.stop("FeedbackBuffer Render");
        }

        public void copyFromGpu()
        {
            PerformanceMonitor.start("FeedbackBuffer Copy");
            pixelBuffer.Value.blitToMemory(fullBitmapBox);
            PerformanceMonitor.stop("FeedbackBuffer Copy");

            analyzeBuffer(); //Doing this on main thread for now
        }

        public float Frac(float value)
        {
            return value - (float)Math.Truncate(value);
        }

        private unsafe void analyzeBuffer()
        {
            PerformanceMonitor.start("FeedbackBuffer Analyze");
            float u, v;
            int m, t;
            virtualTextureManager.beginPageUpdate();
            for (int slId = 0; slId < fullBitmap.Height; ++slId)
            {
                var scanline = fullBitmap.GetScanline<RGBQUAD>(slId);
                RGBQUAD[] slData = scanline.Data;
                for (int pxId = 0; pxId < scanline.Count; ++pxId)
                {
                    var px = slData[pxId];
                    t = px.rgbReserved; //There is a change of this changing to the wrong texture id, but who knows if would happen (precision issues)

                    IndirectionTexture indirectionTexture;
                    if(t != 255 && virtualTextureManager.getIndirectionTexture(t, out indirectionTexture))
                    {
                        //Here the uvs are crushed to 8 bit, but should be ok since we are just detecting pages 
                        //with this number this allows 255 pages to be decenly processsed above that data might be lost.
                        u = px.rgbRed / 255.0f;
                        v = px.rgbGreen / 255.0f;
                        m = px.rgbBlue;
                        indirectionTexture.processPage(u, v, m);
                    }
                }
            }
            virtualTextureManager.finishPageUpdate();
            PerformanceMonitor.stop("FeedbackBuffer Analyze");
        }

        internal void cameraDestroyed()
        {
            renderTexture.destroyViewport(vp);
            vp = null;
            renderTexture.Dispose();
            pixelBuffer.Dispose();
            texture.Dispose();
            fullBitmapBox.Dispose();
            fullBitmap.Dispose();
        }

        internal void cameraCreated()
        {
            int width = window.RenderWidth / 10;
            if(width < 10)
            {
                width = 10;
            }
            int height = window.RenderHeight / 10;
            if(height < 10)
            {
                height = 10;
            }

            texture = TextureManager.getInstance().createManual("FeedbackBuffer", VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 0, OgrePlugin.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);

            fullBitmap = new FreeImageBitmap((int)texture.Value.Width, (int)texture.Value.Height, FreeImageAPI.PixelFormat.Format32bppRgb);
            fullBitmapBox = fullBitmap.createPixelBox(OgrePlugin.PixelFormat.PF_A8R8G8B8);

            pixelBuffer = texture.Value.getBuffer();
            renderTexture = pixelBuffer.Value.getRenderTarget();
            renderTexture.setAutoUpdated(false);

            vp = renderTexture.addViewport(window.Camera);
            vp.setMaterialScheme(Scheme);
            vp.setBackgroundColor(new Engine.Color(0.0f, 0.0f, 0.0f, 1.0f));
        }
    }
}
