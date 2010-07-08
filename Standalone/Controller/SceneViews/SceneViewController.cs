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
    public delegate void SceneViewWindowEvent(SceneViewWindow window);

    class SceneViewController : IDisposable
    {
        public event SceneViewWindowEvent WindowCreated;
        public event SceneViewWindowEvent WindowDestroyed;
        public event SceneViewWindowEvent ActiveWindowChanged;

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
        }

        public void Dispose()
        {
            destroyCameras();
            window.Dispose();
        }

        public void createWindow(String name, Vector3 translation, Vector3 lookAt)
        {
            //temp, will soon have more than one window variable
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, null, eventManager);
            window = new SceneViewWindow(mainTimer, orbitCamera, name);
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(window);
            }
        }

        public void createCameras(SimScene scene)
        {
            window.createSceneView(rendererWindow, scene);
            layoutContainer.setWindow(window);

            MyGUIInterface myGui = PluginManager.Instance.getPlugin("MyGUIPlugin") as MyGUIInterface;
            OgreRenderManager rm = myGui.OgrePlatform.getRenderManager();
            rm.setActiveViewport(1);
        }

        public void destroyCameras()
        {
            window.destroySceneView();
        }

        public void resetAllCameraPositions()
        {
            window.resetToStartPosition();
        }

        public LayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        public SceneViewWindow ActiveWindow
        {
            get
            {
                return window;
            }
        }
    }
}
