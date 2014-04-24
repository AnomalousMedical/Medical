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

        private OgreWrapper.PixelFormat ogreTextureFormat = OgreWrapper.PixelFormat.PF_X8R8G8B8;

        private bool renderOneFrame = false;
        private bool alwaysRender = true;
        private bool renderingEnabled = true;

        public TextureSceneView(SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name, BackgroundScene background, int zIndexStart, int width, int height)
            :base(controller, mainTimer, cameraMover, name, background, zIndexStart)
        {
            this.TextureName = name;
            texture = TextureManager.getInstance().createManual(name, "MyGUI", TextureType.TEX_TYPE_2D, (uint)width, (uint)height, 1, 1, ogreTextureFormat, TextureUsage.TU_RENDERTARGET, false, 0);

            pixelBuffer = texture.Value.getBuffer();
            renderTexture = pixelBuffer.Value.getRenderTarget();
            this.RenderingEnded += TextureSceneView_RenderingEnded;

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

        public bool RenderingEnabled
        {
            get
            {
                return renderingEnabled;
            }
            set
            {
                if(renderingEnabled != value)
                {
                    renderingEnabled = value;
                    determineRenderingActive();
                }
            }
        }

        public bool AlwaysRender
        {
            get
            {
                return alwaysRender;
            }
            set
            {
                if(alwaysRender != value)
                {
                    alwaysRender = value;
                    determineRenderingActive();
                }
            }
        }

        public bool RenderOneFrame
        {
            get
            {
                return renderOneFrame;
            }
            set
            {
                if(renderOneFrame != value)
                {
                    renderOneFrame = value;
                    determineRenderingActive();
                }
            }
        }

        public uint Width
        {
            get
            {
                return renderTexture.getWidth();
            }
        }

        public uint Height
        {
            get
            {
                return renderTexture.getHeight();
            }
        }

        public String TextureName { get; private set; }

        void determineRenderingActive()
        {
            renderTexture.setAutoUpdated(renderingEnabled && (alwaysRender || renderOneFrame));
        }

        void TextureSceneView_RenderingEnded(SceneViewWindow window, bool currentCameraRender)
        {
            RenderOneFrame = false;
        }
    }
}
