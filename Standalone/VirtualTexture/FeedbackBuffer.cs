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

        SceneViewWindow window;
        Viewport vp;

        public FeedbackBuffer(SceneViewWindow window)
        {
            this.window = window;
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

        private unsafe void analyzeBuffer()
        {
            PerformanceMonitor.start("FeedbackBuffer Analyze");
            int x, y, m, t;
            for (int slId = 0; slId < fullBitmap.Height; ++slId)
            {
                var scanline = fullBitmap.GetScanline<RGBQUAD>(slId);
                RGBQUAD[] slData = scanline.Data;
                for (int pxId = 0; pxId < scanline.Count; ++pxId)
                {
                    var px = slData[pxId];
                    byte* p = (byte*)&px;
                    if (px.rgbBlue != 0 || px.rgbGreen != 0 || px.rgbRed != 0 || (px.rgbReserved != 255 && px.rgbReserved != 0))
                    {
//#if READ_RGBA
                          x = p[0] + ((p[2] & 0x0f) << 8);   
                          y = p[1] + ((p[2] & 0xf0) << 4);
                          m = p[3] & 15;
                          t = p[3] & ~15;
                          Logging.Log.Debug("{0}, {1} m:{2} t:{3}", x, y, m, t);
//#else
//                        x = p[3] + ((p[1] & 0x0f) << 8);
//                        y = p[2] + ((p[1] & 0xf0) << 4);
//                        m = p[0] & 15;
//                        t = p[0] & ~15;
//#endif
                    }
                }
            }
            PerformanceMonitor.stop("FeedbackBuffer Analyze");
        }

        public FreeImageBitmap MainMemoryBuffer
        {
            get
            {
                return fullBitmap;
            }
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
        }
    }
}
