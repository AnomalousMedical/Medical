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
        }

        public void Dispose()
        {
            destroyCameras();
            foreach (SceneViewWindow window in windows)
            {
                window.Dispose();
            }
        }

        public void createWindow(String name, Vector3 translation, Vector3 lookAt)
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
        }

        public void createCameras(SimScene scene)
        {
            MDIWindow previousWindow = null;
            int i = 0;
            foreach (SceneViewWindow window in windows)
            {
                createCameraForWindow(window, scene);
                //temporary
                MDIWindow childWindow = new MDIWindow("MDIWindow.layout", window.Name);
                childWindow.SuppressLayout = true;
                childWindow.Content = window;
                childWindow.SuppressLayout = false;
                childWindow.Caption = window.Name;
                switch (i++)
                {
                    case 0:
                        mdiLayout.addWindow(childWindow);
                        break;
                    case 1:
                        mdiLayout.addWindow(childWindow, previousWindow, WindowAlignment.Left);
                        break;
                    case 2:
                        mdiLayout.addWindow(childWindow, previousWindow, WindowAlignment.Bottom);
                        break;
                    case 3:
                        mdiLayout.addWindow(childWindow, previousWindow, WindowAlignment.Right);
                        break;
                }
                previousWindow = childWindow;
                //end temp
            }
            camerasCreated = true;
            currentScene = scene;
        }

        private void createCameraForWindow(SceneViewWindow window, SimScene scene)
        {
            window.createSceneView(rendererWindow, scene);
            rm.setActiveViewport(rm.getActiveViewport() + 1);

            
            
            //layoutContainer.addChild(window);
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
                return windows[0];
            }
        }

        public bool AllowRotation { get; set; }

        public bool AllowZoom { get; set; }
    }
}
