using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Renderer;
using Engine.ObjectManagement;
using MyGUIPlugin;

namespace Medical.Controller
{
    class SceneViewController : IDisposable
    {
        private SceneViewLayoutContainer layoutContainer = new SceneViewLayoutContainer();
        private SceneViewWindow window;
        private EventManager eventManager;
        private UpdateTimer mainTimer;
        private RendererWindow rendererWindow;

        public SceneViewController(EventManager eventManager, UpdateTimer mainTimer, RendererWindow rendererWindow)
        {
            this.eventManager = eventManager;
            this.mainTimer = mainTimer;
            this.rendererWindow = rendererWindow;
            window = new SceneViewWindow(new OrbitCameraController(new Vector3(0, -5, 150), new Vector3(0, -5, 0), null, eventManager), "Default");
        }

        public void Dispose()
        {
            destroyCameras();
        }

        public void createCameras(SimScene scene)
        {
            window.createSceneView(rendererWindow, mainTimer, scene);
            layoutContainer.setWindow(window);

            MyGUIInterface myGui = PluginManager.Instance.getPlugin("MyGUIPlugin") as MyGUIInterface;
            OgreRenderManager rm = myGui.OgrePlatform.getRenderManager();
            rm.setActiveViewport(1);
        }

        public void destroyCameras()
        {
            window.destroySceneView();
        }

        public ScreenLayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }
    }
}
