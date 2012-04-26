using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using OgreWrapper;
using OgrePlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class RocketGuiManager : IDisposable
    {
        private SceneManager sceneManager;
        //private OgreWindow ogreWindow;
        private Camera camera;
        private Viewport vp;
        private TexturePtr texture;
        HardwarePixelBufferSharedPtr pixelBuffer;

        private Context context;
        private ContextUpdater contextUpdater;
        private EventListenerInstancer eventListenerInstancer;
        private ContextWindowListener windowListener;

        public RocketGuiManager()
        {

        }

        public void Dispose()
        {
            if (windowListener != null)
            {
                MainWindow.Instance.removeListener(windowListener);
            }
            if (contextUpdater != null)
            {
                contextUpdater.Dispose();
            }
            if (context != null)
            {
                context.Dispose();
            }
            if (eventListenerInstancer != null)
            {
                eventListenerInstancer.Dispose();
            }

            if (vp != null)
            {
                pixelBuffer.Value.getRenderTarget().destroyViewport(vp);
                //ogreWindow.OgreRenderWindow.destroyViewport(vp);
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

        public void initialize(PluginManager pluginManager, EventManager eventManager, UpdateTimer mainTimer)
        {
            //Create a rocket group in ogre
            OgreResourceGroupManager.getInstance().createResourceGroup("Rocket");

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

            eventListenerInstancer = new TestEventListenerInstancer();
            Factory.RegisterEventListenerInstancer(eventListenerInstancer);

            String sample_path = "S:/Junk/librocket/playing/";//"S:/dependencies/libRocket/src/Samples/";
            VirtualFileSystem.Instance.addArchive(sample_path);
            OgreResourceGroupManager.getInstance().addResourceLocation("assets", "EngineArchive", "Rocket", false);

            FontDatabase.LoadFontFace("assets/Delicious-Roman.otf");
            FontDatabase.LoadFontFace("assets/Delicious-Bold.otf");
            FontDatabase.LoadFontFace("assets/Delicious-Italic.otf");
            FontDatabase.LoadFontFace("assets/Delicious-BoldItalic.otf");

            //context = Core.CreateContext("main", new Vector2i((int)ogreWindow.OgreRenderWindow.getWidth(), (int)ogreWindow.OgreRenderWindow.getHeight()));
            context = Core.CreateContext("main", new Vector2i(2048, 2048));
            Debugger.Initialise(context);

            //windowListener = new ContextWindowListener(context);
            //MainWindow.Instance.addListener(windowListener);

            //using (ElementDocument cursor = context.LoadMouseCursor(sample_path + "assets/cursor.rml"))
            //{

            //}

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

            MyGUIPlugin.ImageBox imagebox = (MyGUIPlugin.ImageBox)MyGUIPlugin.Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, 2048, 2048, MyGUIPlugin.Align.Default, "Overlapped", "RocketTestImage");
            imagebox.setImageTexture("__RocketRTT");
            //imagebox.setImageCoord(new MyGUIPlugin.IntCoord(600, 600, 2048, 2048));
            //imagebox.setima
        }
    }
}
