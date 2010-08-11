using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Renderer;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using OgrePlugin;

namespace Medical.Controller
{
    public delegate void SceneViewWindowEvent(SceneViewWindow window);

    public class SceneViewController : IDisposable
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
        private PopupSceneViewWindow cloneWindow = null;

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

        public MDISceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt)
        {
            MDISceneViewWindow window = doCreateWindow(name, ref translation, ref lookAt);
            mdiLayout.showWindow(window._getMDIWindow());
            return window;
        }

        public MDISceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt, MDISceneViewWindow previous, WindowAlignment alignment)
        {
            MDISceneViewWindow window = doCreateWindow(name, ref translation, ref lookAt);
            mdiLayout.showWindow(window._getMDIWindow(), previous._getMDIWindow(), alignment);
            return window;
        }

        public void destroyWindow(SceneViewWindow window)
        {
            if (WindowDestroyed != null)
            {
                WindowDestroyed.Invoke(window);
            }
            if (camerasCreated)
            {
                window.destroySceneView();
            }
            windows.Remove(window);
            window.Dispose();
        }

        public void createCameras(SimScene scene)
        {
            foreach (SceneViewWindow window in windows)
            {
                window.createSceneView(rendererWindow, scene);
            }
            camerasCreated = true;
            currentScene = scene;
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

        public void changeRendererWindow(RendererWindow rendererWindow)
        {
            this.rendererWindow = rendererWindow;
        }

        public void createCloneWindow(WindowInfo windowInfo)
        {
            if (cloneWindow == null)
            {
                CloneCamera cloneCamera = new CloneCamera(this);
                RendererWindow window = OgreInterface.Instance.createRendererWindow(windowInfo);
                ((OgreWindow)window).OgreRenderWindow.DeactivateOnFocusChange = false;
                cloneWindow = new PopupSceneViewWindow(window, this, mainTimer, cloneCamera, "Clone");
                if (WindowCreated != null)
                {
                    WindowCreated.Invoke(cloneWindow);
                }
                if (camerasCreated)
                {
                    cloneWindow.createSceneView(window, currentScene);
                }
                windows.Add(cloneWindow);
            }
            else
            {
                destroyWindow(cloneWindow);
                OgreInterface.Instance.destroyRendererWindow(cloneWindow.RendererWindow);
                cloneWindow = null;
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
            bool foundWindow = false;
            MDIWindow activeMDIWindow = mdiLayout.ActiveWindow;
            foreach (SceneViewWindow window in windows)
            {
                MDISceneViewWindow mdiSceneWindow = window as MDISceneViewWindow;
                if (mdiSceneWindow != null && mdiSceneWindow._getMDIWindow() == activeMDIWindow)
                {
                    activeWindow = window;
                    if (ActiveWindowChanged != null)
                    {
                        ActiveWindowChanged.Invoke(window);
                    }
                    foundWindow = true;
                    break;
                }
            }
            //If we did not find the window specified, use the first window as the current window
            if (!foundWindow && windows.Count > 0)
            {
                activeWindow = windows[0];
                if (ActiveWindowChanged != null)
                {
                    ActiveWindowChanged.Invoke(activeWindow);
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
        private MDISceneViewWindow doCreateWindow(String name, ref Vector3 translation, ref Vector3 lookAt)
        {
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, null, eventManager);
            orbitCamera.AllowRotation = AllowRotation;
            orbitCamera.AllowZoom = AllowZoom;
            MDISceneViewWindow window = new MDISceneViewWindow(rm, this, mainTimer, orbitCamera, name);
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(window);
            }
            if (camerasCreated)
            {
                window.createSceneView(rendererWindow, currentScene);
            }
            windows.Add(window);
            return window;
        }
    }
}
