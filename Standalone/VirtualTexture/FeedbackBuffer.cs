using Anomalous.GuiFramework.Cameras;
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
            renderTexture.update();
        }

        public void copyFromGpu()
        {
            pixelBuffer.Value.blitToMemory(fullBitmapBox);
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

            texture = TextureManager.getInstance().createManual("FeedbackBuffer", "FeedbackBufferGroup", TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 0, OgrePlugin.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0);

            fullBitmap = new FreeImageBitmap((int)texture.Value.Width, (int)texture.Value.Height, FreeImageAPI.PixelFormat.Format32bppRgb);
            fullBitmapBox = fullBitmap.createPixelBox(OgrePlugin.PixelFormat.PF_A8R8G8B8);

            pixelBuffer = texture.Value.getBuffer();
            renderTexture = pixelBuffer.Value.getRenderTarget();
            renderTexture.setAutoUpdated(false);

            vp = renderTexture.addViewport(window.Camera);
            vp.setMaterialScheme("FeedbackBuffer");
        }
    }
}
