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
        int id;

        Viewport vp;

        public FeedbackBuffer(VirtualTextureManager virtualTextureManager, int id)
        {
            this.id = id;
            this.virtualTextureManager = virtualTextureManager;
        }

        public void update()
        {
            PerformanceMonitor.start("FeedbackBuffer Render");
            renderTexture.update();
            PerformanceMonitor.stop("FeedbackBuffer Render");
        }

        public void blitToStaging()
        {
            PerformanceMonitor.start("FeedbackBuffer blit to staging");
            pixelBuffer.Value.blitToStaging();
            PerformanceMonitor.stop("FeedbackBuffer blit to staging");
        }

        public void blitStagingToMemory()
        {
            PerformanceMonitor.start("FeedbackBuffer blit staging to memory");
            pixelBuffer.Value.blitStagingToMemory(fullBitmapBox);
            PerformanceMonitor.stop("FeedbackBuffer blit staging to memory");
        }

        public unsafe void analyzeBuffer()
        {
            PerformanceMonitor.start("FeedbackBuffer Analyze");
            float u, v;
            byte m, t;
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
            PerformanceMonitor.stop("FeedbackBuffer Analyze");
        }

        public String TextureName
        {
            get
            {
                return "FeedbackBuffer" + id;
            }
        }

        internal void destroyCamera()
        {
            renderTexture.destroyViewport(vp);
            vp = null;
            renderTexture.Dispose();
            renderTexture = null;
            pixelBuffer.Dispose();
            texture.Dispose();
            fullBitmapBox.Dispose();
            fullBitmap.Dispose();
        }

        internal void createCamera(Camera camera, IntSize2 renderSize)
        {
            texture = TextureManager.getInstance().createManual(TextureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, (uint)renderSize.Width, (uint)renderSize.Height, 1, 0, OgrePlugin.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);

            fullBitmap = new FreeImageBitmap((int)texture.Value.Width, (int)texture.Value.Height, FreeImageAPI.PixelFormat.Format32bppRgb);
            fullBitmapBox = fullBitmap.createPixelBox(OgrePlugin.PixelFormat.PF_A8R8G8B8);

            pixelBuffer = texture.Value.getBuffer();
            pixelBuffer.Value.OptimizeReadback = true;
            renderTexture = pixelBuffer.Value.getRenderTarget();
            renderTexture.setAutoUpdated(false);

            vp = renderTexture.addViewport(camera);
            vp.setMaterialScheme(Scheme);
            vp.setBackgroundColor(new Engine.Color(0.0f, 0.0f, 0.0f, 1.0f));
            vp.clear();
        }
    }
}
