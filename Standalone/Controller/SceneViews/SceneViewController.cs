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
        private bool camerasCreated = false;
        private SimScene currentScene = null;

        public SceneViewController(EventManager eventManager, UpdateTimer mainTimer, RendererWindow rendererWindow)
        {
            this.eventManager = eventManager;
            this.mainTimer = mainTimer;
            this.rendererWindow = rendererWindow;
            AllowRotation = true;
            AllowZoom = true;
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
            orbitCamera.AllowRotation = AllowRotation;
            orbitCamera.AllowZoom = AllowZoom;
            window = new SceneViewWindow(mainTimer, orbitCamera, name);
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(window);
            }
            if (camerasCreated)
            {
                createCameras(currentScene);
            }
        }

        public void createCameras(SimScene scene)
        {
            if (window != null)
            {
                window.createSceneView(rendererWindow, scene);
                layoutContainer.setWindow(window);

                MyGUIInterface myGui = PluginManager.Instance.getPlugin("MyGUIPlugin") as MyGUIInterface;
                OgreRenderManager rm = myGui.OgrePlatform.getRenderManager();
                rm.setActiveViewport(1);
            }
            camerasCreated = true;
            currentScene = scene;
        }

        public void destroyCameras()
        {
            if (window != null)
            {
                window.destroySceneView();
            }
            camerasCreated = false;
            currentScene = null;
        }

        public void resetAllCameraPositions()
        {
            if (window != null)
            {
                window.resetToStartPosition();
            }
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

        public bool AllowRotation { get; set; }

        public bool AllowZoom { get; set; }
    }
}
