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
        private bool autoAspectRatio = true;
        private float aspectRatio = 4f / 3f;
        private BackgroundScene background;

        private SingleViewCloneWindow cloneWindow = null;
        private List<MDISceneViewWindow> mdiWindows = new List<MDISceneViewWindow>();
        private List<TextureSceneView> textureWindows = new List<TextureSceneView>();

        public SceneViewController(MDILayoutManager mdiLayout, EventManager eventManager, UpdateTimer mainTimer, RendererWindow rendererWindow, OgreRenderManager renderManager, BackgroundScene background)
        {
            this.background = background;
            this.eventManager = eventManager;
            this.mainTimer = mainTimer;
            this.rendererWindow = rendererWindow;
            this.mdiLayout = mdiLayout;

            rm = renderManager;
            mdiLayout.ActiveWindowChanged += new EventHandler(mdiLayout_ActiveWindowChanged);
        }

        public void Dispose()
        {
            destroyCameras();
            foreach (SceneViewWindow window in mdiWindows)
            {
                window.Dispose();
            }
            foreach(TextureSceneView window in textureWindows)
            {
                window.Dispose();
            }
        }

        public MDISceneViewWindow createWindow(String name, Vector3 translation, Vector3 lookAt, Vector3 boundMin, Vector3 boundMax, float minOrbitDistance, float maxOrbitDistance, int zIndexStart, MDISceneViewWindow previous = null, WindowAlignment alignment = WindowAlignment.Left)
        {
            MDISceneViewWindow window = doCreateWindow(name, translation, lookAt, boundMin, boundMax, minOrbitDistance, maxOrbitDistance, zIndexStart);
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

        public TextureSceneView createTextureSceneView(String name, Vector3 translation, Vector3 lookAt, int width, int height)
        {
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, Vector3.Zero, Vector3.Zero, 0, 1000, null, eventManager);
            orbitCamera.AllowRotation = AllowRotation;
            orbitCamera.AllowZoom = AllowZoom;

            TextureSceneView window = new TextureSceneView(this, mainTimer, orbitCamera, name, background, 0, width, height);

            if (camerasCreated)
            {
                window.createSceneView(currentScene);
            }

            textureWindows.Add(window);

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
            if (window == cloneWindow)
            {
                cloneWindow = null;
            }
            else if (mdiWindows.Remove(window as MDISceneViewWindow))
            {
                //On the last window, disable closing it.
                if (mdiWindows.Count == 1)
                {
                    mdiWindows[0].AllowClose = false;
                }
            }
            else
            {
                textureWindows.Remove(window as TextureSceneView);
            }

            if (window == activeWindow)
            {
                if (mdiWindows.Count > 0)
                {
                    activeWindow = mdiWindows[0];
                    if (ActiveWindowChanged != null)
                    {
                        ActiveWindowChanged.Invoke(activeWindow);
                    }
                }
                else
                {
                    activeWindow = null;
                }
            }

            window.Dispose();
        }

        public void createCameras(SimScene scene)
        {
            foreach (SceneViewWindow window in mdiWindows)
            {
                window.createSceneView(scene);
            }
            foreach(TextureSceneView window in textureWindows)
            {
                window.createSceneView(scene);
            }
            if (cloneWindow != null)
            {
                cloneWindow.createSceneView(scene);
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
            foreach (TextureSceneView window in textureWindows)
            {
                window.destroySceneView();
            }
            if (cloneWindow != null)
            {
                cloneWindow.destroySceneView();
            }
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

        public void createCloneWindow(WindowInfo windowInfo, bool floatOnParent)
        {
            if (cloneWindow == null)
            {
                CloneCamera cloneCamera = new CloneCamera(this);
                cloneWindow = new SingleViewCloneWindow(windowInfo, this, mainTimer, cloneCamera, "Clone", background, 0, floatOnParent);
                cloneWindow.Closed += new EventHandler(cloneWindow_Closed);
                if (WindowCreated != null)
                {
                    WindowCreated.Invoke(cloneWindow);
                }
                if (camerasCreated)
                {
                    cloneWindow.createSceneView(currentScene);
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

        public void createFromPresets(SceneViewWindowPresetSet presets, bool keepOldSettings = true)
        {
            //Capture current window configuration info
            List<Bookmark> currentWindowConfig = new List<Bookmark>();
            if (keepOldSettings)
            {
                SceneViewWindow activeWindow = ActiveWindow;
                if (activeWindow != null)
                {
                    TransparencyController.ActiveTransparencyState = activeWindow.CurrentTransparencyState;
                    LayerState layerState = new LayerState();
                    layerState.captureState();
                    currentWindowConfig.Add(new Bookmark("", activeWindow.Translation, activeWindow.LookAt, layerState));
                }
                foreach (MDISceneViewWindow window in mdiWindows)
                {
                    if (window != activeWindow)
                    {
                        TransparencyController.ActiveTransparencyState = window.CurrentTransparencyState;
                        LayerState layerState = new LayerState();
                        layerState.captureState();
                        currentWindowConfig.Add(new Bookmark("", window.Translation, window.LookAt, layerState));
                    }
                }
            }

            //Create windows
            int windowIndex = 0;
            int zOrder = 100;
            int zOrderInc = 10;
            closeAllWindows();
            MDISceneViewWindow camera;
            MDISceneViewWindow toSelect = null;
            foreach (SceneViewWindowPreset preset in presets.getPresetEnum())
            {
                if (windowIndex < currentWindowConfig.Count)
                {
                    Bookmark bmk = currentWindowConfig[windowIndex++];
                    camera = createWindow(preset.Name, bmk.CameraPosition.Translation, bmk.CameraPosition.LookAt, preset.BoundMin, preset.BoundMax, preset.OrbitMinDistance, preset.OrbitMaxDistance, zOrder, findWindow(preset.ParentWindow), preset.WindowPosition);
                    TransparencyController.ActiveTransparencyState = camera.CurrentTransparencyState;
                    bmk.Layers.instantlyApply();
                }
                else
                {
                    camera = createWindow(preset.Name, preset.Position, preset.LookAt, preset.BoundMin, preset.BoundMax, preset.OrbitMinDistance, preset.OrbitMaxDistance, zOrder, findWindow(preset.ParentWindow), preset.WindowPosition);
                }
                if (toSelect == null)
                {
                    toSelect = camera;
                }
                zOrder += zOrderInc;
            }
            if (toSelect != null)
            {
                mdiLayout.ActiveWindow = toSelect._getMDIWindow();
            }
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
                    return mdiWindows.Count > 0 ? mdiWindows[0] : null;
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

        public bool AutoAspectRatio
        {
            get
            {
                return autoAspectRatio;
            }
            set
            {
                autoAspectRatio = value;
                foreach (SceneViewWindow window in mdiWindows)
                {
                    window.AutoAspectRatio = autoAspectRatio;
                    window.layout();
                }
            }
        }

        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
                foreach (SceneViewWindow window in mdiWindows)
                {
                    window.AspectRatio = aspectRatio;
                    window.layout();
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
        private MDISceneViewWindow doCreateWindow(String name, Vector3 translation, Vector3 lookAt, Vector3 boundMin, Vector3 boundMax, float minOrbitDistance, float maxOrbitDistance, int zIndexStart)
        {
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, boundMin, boundMax, minOrbitDistance, maxOrbitDistance, null, eventManager);
            orbitCamera.AllowRotation = AllowRotation;
            orbitCamera.AllowZoom = AllowZoom;
            MDISceneViewWindow window = new MDISceneViewWindow(rendererWindow, this, mainTimer, orbitCamera, name, background, zIndexStart);
            window.AutoAspectRatio = autoAspectRatio;
            window.AspectRatio = aspectRatio;
            if (WindowCreated != null)
            {
                WindowCreated.Invoke(window);
            }
            if (camerasCreated)
            {
                window.createSceneView(currentScene);
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
    }
}
