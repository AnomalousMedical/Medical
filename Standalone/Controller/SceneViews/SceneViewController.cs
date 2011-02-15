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
using System.Drawing;
using Medical.GUI;

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
        private bool allowRotation = true;
        private bool allowZoom = true;

        private SingleViewCloneWindow cloneWindow = null;
        private List<MDISceneViewWindow> mdiWindows = new List<MDISceneViewWindow>();

        public SceneViewController(MDILayoutManager mdiLayout, EventManager eventManager, UpdateTimer mainTimer, RendererWindow rendererWindow, OgreRenderManager renderManager)
        {
            this.eventManager = eventManager;
            this.mainTimer = mainTimer;
            this.rendererWindow = rendererWindow;
            this.mdiLayout = mdiLayout;

            rm = renderManager;
            mdiLayout.ActiveWindowChanged += new EventHandler(mdiLayout_ActiveWindowChanged);

            MedicalConfig.EngineConfig.ShowStatsToggled += new EventHandler(EngineConfig_ShowStatsToggled);
        }

        public void Dispose()
        {
            destroyCameras();
            foreach (SceneViewWindow window in mdiWindows)
            {
                window.Dispose();
            }
        }

        public MDISceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt)
        {
            return createWindow(name, translation, lookAt, null, WindowAlignment.Left);
        }

        public MDISceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt, MDISceneViewWindow previous, WindowAlignment alignment)
        {
            MDISceneViewWindow window = doCreateWindow(name, ref translation, ref lookAt);
            if (previous != null)
            {
                mdiLayout.showWindow(window._getMDIWindow(), previous._getMDIWindow(), alignment);
            }
            else
            {
                mdiLayout.showWindow(window._getMDIWindow());
            }
            return window;
        }

        public MDISceneViewWindow findWindow(String name)
        {
            foreach (SceneViewWindow window in mdiWindows)
            {
                if (window.Name == name)
                {
                    return window as MDISceneViewWindow;
                }
            }
            return null;
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
            if (window == cloneWindow)
            {
                cloneWindow = null;
            }
            else
            {
                mdiWindows.Remove((MDISceneViewWindow)window);
                //On the last window, disable closing it.
                if (mdiWindows.Count == 1)
                {
                    mdiWindows[0].AllowClose = false;
                }
            }
            window.Dispose();
        }

        public void createCameras(SimScene scene)
        {
            foreach (SceneViewWindow window in mdiWindows)
            {
                window.createSceneView(rendererWindow, scene);
            }
            if (cloneWindow != null)
            {
                cloneWindow.createSceneView(rendererWindow, scene);
            }
            camerasCreated = true;
            currentScene = scene;
        }

        public void destroyCameras()
        {
            foreach (SceneViewWindow window in mdiWindows)
            {
                window.destroySceneView();
            }
            if (cloneWindow != null)
            {
                cloneWindow.destroySceneView();
            }
            rm.setActiveViewport(0);
            camerasCreated = false;
            currentScene = null;
        }

        public void resetAllCameraPositions()
        {
            foreach (SceneViewWindow window in mdiWindows)
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
                cloneWindow = new SingleViewCloneWindow(windowInfo, this, mainTimer, cloneCamera, "Clone");
                cloneWindow.Closed += new EventHandler(cloneWindow_Closed);
                if (WindowCreated != null)
                {
                    WindowCreated.Invoke(cloneWindow);
                }
                if (camerasCreated)
                {
                    cloneWindow.createSceneView(null, currentScene);
                }
            }
        }

        public void destroyCloneWindow()
        {
            cloneWindow.close();
        }

        void cloneWindow_Closed(object sender, EventArgs e)
        {
            cloneWindow = null;
        }

        public void closeAllWindows()
        {
            List<MDISceneViewWindow> windowListCopy = new List<MDISceneViewWindow>(mdiWindows);
            foreach (MDISceneViewWindow window in windowListCopy)
            {
                window.close();
            }
        }

        public void createFromPresets(SceneViewWindowPresetSet presets)
        {
            //StatusController.SetStatus("Recreating viewports...");
            closeAllWindows();
            SceneViewWindow camera;
            foreach (SceneViewWindowPreset preset in presets.getPresetEnum())
            {
                camera = createWindow(preset.Name, preset.Position, preset.LookAt, findWindow(preset.ParentWindow), preset.WindowPosition);
            }
            //StatusController.TaskCompleted();
        }

        public bool HasCloneWindow
        {
            get
            {
                return cloneWindow != null;
            }
        }

        public SceneViewWindow ActiveWindow
        {
            get
            {
                if (activeWindow == null)
                {
                    return mdiWindows[0];
                }
                return activeWindow;
            }
        }

        public bool AllowRotation
        {
            get
            {
                return allowRotation;
            }
            set
            {
                allowRotation = value;
                foreach (SceneViewWindow window in mdiWindows)
                {
                    OrbitCameraController orbitCamera = window.CameraMover as OrbitCameraController;
                    if (orbitCamera != null)
                    {
                        orbitCamera.AllowRotation = value;
                    }
                }
            }
        }

        public bool AllowZoom
        {
            get
            {
                return allowZoom;
            }
            set
            {
                allowZoom = value;
                foreach (SceneViewWindow window in mdiWindows)
                {
                    OrbitCameraController orbitCamera = window.CameraMover as OrbitCameraController;
                    if (orbitCamera != null)
                    {
                        orbitCamera.AllowZoom = value;
                    }
                }
            }
        }

        private void mdiLayout_ActiveWindowChanged(object sender, EventArgs e)
        {
            //Check to see if the active window is one of the SceneViewWindow's MDIWindow
            bool foundWindow = false;
            MDIWindow activeMDIWindow = mdiLayout.ActiveWindow;
            foreach (SceneViewWindow window in mdiWindows)
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
            if (!foundWindow && mdiWindows.Count > 0)
            {
                activeWindow = mdiWindows[0];
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
            //Count is 0, disable close button on first window
            if (mdiWindows.Count == 0)
            {
                window.AllowClose = false;
            }
            //Count is 1, enable the close button on the first window
            if (mdiWindows.Count == 1)
            {
                mdiWindows[0].AllowClose = true;
            }
            mdiWindows.Add(window);
            return window;
        }

        private void EngineConfig_ShowStatsToggled(object sender, EventArgs e)
        {
            bool showStats = MedicalConfig.EngineConfig.ShowStatistics;
            foreach (MDISceneViewWindow mdiWindow in mdiWindows)
            {
                mdiWindow.showSceneStats(showStats);
            }
            if (cloneWindow != null)
            {
                cloneWindow.showSceneStats(showStats);
            }
        }
    }
}
