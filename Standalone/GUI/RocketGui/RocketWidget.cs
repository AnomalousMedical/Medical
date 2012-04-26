using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using libRocketPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class RocketWidget : IDisposable
    {
        private SceneManager sceneManager;
        private Camera camera;
        private Viewport vp;
        private TexturePtr texture;
        private HardwarePixelBufferSharedPtr pixelBuffer;

        private Context context;

        private ImageBox imageBox;

        //temp
        private ContextUpdater contextUpdater;
        //end temp

        public RocketWidget(String name, EventManager eventManager, UpdateTimer mainTimer)
        {
            sceneManager = Root.getSingleton().createSceneManager(SceneType.ST_GENERIC, "libRocketScene");
            //ogreWindow = pluginManager.RendererPlugin.PrimaryWindow as OgreWindow;

            //Create camera and viewport
            camera = sceneManager.createCamera("libRocketCamera");

            texture = TextureManager.getInstance().createManual("__RocketRTT", "Rocket", TextureType.TEX_TYPE_2D, (uint)2048, (uint)2048, 1, 1, OgreWrapper.PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, false, 0);

            //vp = ogreWindow.OgreRenderWindow.addViewport(camera, 2000000, 0.0f, 0.0f, 1.0f, 1.0f);
            pixelBuffer = texture.Value.getBuffer();
            vp = pixelBuffer.Value.getRenderTarget().addViewport(camera);
            vp.setBackgroundColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
            vp.setOverlaysEnabled(false);
            //vp.setClearEveryFrame(false);
            vp.clear();

            context = Core.CreateContext(name, new Vector2i(2048, 2048));

            using (ElementDocument document = context.LoadDocument("assets/demo.rml"))
            {
                if (document != null)
                {
                    document.Show();
                }
            }

            sceneManager.addRenderQueueListener(new RocketRenderQueueListener(context, (RenderInterfaceOgre3D)Core.GetRenderInterface()));
            contextUpdater = new ContextUpdater(context, eventManager);
            mainTimer.addFixedUpdateListener(contextUpdater);

            imageBox = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, 2048, 2048, MyGUIPlugin.Align.Default, "Overlapped", name);
            imageBox.setImageTexture("__RocketRTT");
            //imagebox.setImageCoord(new MyGUIPlugin.IntCoord(600, 600, 2048, 2048));
            //imagebox.setima
        }

        public void Dispose()
        {
            if (imageBox != null)
            {
                Gui.Instance.destroyWidget(imageBox);
            }
            if (contextUpdater != null)
            {
                contextUpdater.Dispose();
            }
            if (context != null)
            {
                context.Dispose();
            }
            if (vp != null)
            {
                pixelBuffer.Value.getRenderTarget().destroyViewport(vp);
            }
            if (pixelBuffer != null)
            {
                pixelBuffer.Dispose();
            }
            if (texture != null)
            {
                texture.Dispose();
            }
            if (camera != null)
            {
                sceneManager.destroyCamera(camera);
            }
            if (sceneManager != null)
            {
                Root.getSingleton().destroySceneManager(sceneManager);
            }
        }

    }
}
