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

        private MDILayoutManager mdiLayout;
        private EventManager eventManager;
        private UpdateTimer mainTimer;
        private RendererWindow rendererWindow;
        private bool camerasCreated = false;
        private SimScene currentScene = null;
        private OgreRenderManager rm;
        private SceneViewWindow activeWindow = null;

        private List<SceneViewWindow> windows = new List<SceneViewWindow>();

        public SceneViewController(MDILayoutManager mdiLayout, EventManager eventManager, UpdateTimer mainTimer, RendererWindow rendererWindow, OgreRenderManager renderManager)
        {
            this.eventManager = eventManager;
            this.mainTimer = mainTimer;
            this.rendererWindow = rendererWindow;
            this.mdiLayout = mdiLayout;
            AllowRotation = true;
            AllowZoom = true;

            rm = renderManager;
            mdiLayout.ActiveWindowChanged += new EventHandler(mdiLayout_ActiveWindowChanged);
        }

        public void Dispose()
        {
            destroyCameras();
            foreach (SceneViewWindow window in windows)
            {
                window.Dispose();
            }
        }

        public SceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt)
        {
            SceneViewWindow window = doCreateWindow(name, ref translation, ref lookAt);
            mdiLayout.addWindow(window._getMDIWindow());
            return window;
        }

        public SceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt, SceneViewWindow previous, WindowAlignment alignment)
        {
            SceneViewWindow window = doCreateWindow(name, ref translation, ref lookAt);
            mdiLayout.addWindow(window._getMDIWindow(), previous._getMDIWindow(), alignment);
            return window;
        }

        public void createCameras(SimScene scene)
        {
            foreach (SceneViewWindow window in windows)
            {
                createCameraForWindow(window, scene);
            }
            camerasCreated = true;
            currentScene = scene;
        }

        private void createCameraForWindow(SceneViewWindow window, SimScene scene)
        {
            window.createSceneView(rendererWindow, scene);
            rm.setActiveViewport(rm.getActiveViewport() + 1);
        }

        public void destroyCameras()
        {
            foreach (SceneViewWindow window in windows)
            {
                window.destroySceneView();
            }
            rm.setActiveViewport(0);
            camerasCreated = false;
            currentScene = null;
        }

        public void resetAllCameraPositions()
        {
            foreach (SceneViewWindow window in windows)
            {
                window.resetToStartPosition();
            }
        }

        public SceneViewWindow ActiveWindow
        {
            get
            {
                if (activeWindow == null)
                {
                    return windows[0];
                }
                return activeWindow;
            }
        }

        public bool AllowRotation { get; set; }

        public bool AllowZoom { get; set; }

        private void mdiLayout_ActiveWindowChanged(object sender, EventArgs e)
        {
            //Check to see if the active window is one of the SceneViewWindow's MDIWindow
            MDIWindow activeMDIWindow = mdiLayout.ActiveWindow;
            foreach (SceneViewWindow window in windows)
            {
                if (window._getMDIWindow() == activeMDIWindow)
                {
                    activeWindow = window;
                    if (ActiveWindowChanged != null)
                    {
                        ActiveWindowChanged.Invoke(window);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// This method will create the actual window.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="translation"></param>
        /// <param name="lookAt"></param>
        /// <returns></returns>
        private SceneViewWindow doCreateWindow(String name, ref Vector3 translation, ref Vector3 lookAt)
        {
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, null, eventManager);
            orbitCamera.AllowRotation = AllowRotation;
            orbitCamera.AllowZoom = AllowZoom;
            SceneViewWindow window = new SceneViewWindow(mainTimer, orbitCamera, name);
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(window);
            }
            if (camerasCreated)
            {
                createCameraForWindow(window, currentScene);
            }
            windows.Add(window);
            return window;
        }
    }
}
