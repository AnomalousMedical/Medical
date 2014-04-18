using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    public class TextureSceneView : SceneViewWindow
    {
        private TexturePtr texture;
        private HardwarePixelBufferSharedPtr pixelBuffer;
        private RenderTexture renderTexture;
        private RendererWindow rendererWindow;

        private int currentTextureWidth = 800;
        private int currentTextureHeight = 600;
        private OgreWrapper.PixelFormat ogreTextureFormat = OgreWrapper.PixelFormat.PF_X8R8G8B8;

        public TextureSceneView(SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name, BackgroundScene background, int zIndexStart)
            :base(controller, mainTimer, cameraMover, name, background, zIndexStart)
        {
            texture = TextureManager.getInstance().createManual(name, "Rocket", TextureType.TEX_TYPE_2D, (uint)currentTextureWidth, (uint)currentTextureHeight, 1, 1, ogreTextureFormat, TextureUsage.TU_RENDERTARGET, false, 0);

            pixelBuffer = texture.Value.getBuffer();
            renderTexture = pixelBuffer.Value.getRenderTarget();

            rendererWindow = new ManualWindow(renderTexture);
            this.RendererWindow = rendererWindow;
            this.createBackground(renderTexture, true);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (pixelBuffer != null)
            {
                pixelBuffer.Dispose();
            }
            if (texture != null)
            {
                texture.Dispose();
            }
        }

        public override void close()
        {

        }

        public Texture Texture
        {
            get
            {
                return texture.Value;
            }
        }
    }
}
