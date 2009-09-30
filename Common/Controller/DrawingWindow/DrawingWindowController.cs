using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using System.Windows.Forms;
using Engine.Platform;
using Engine.Renderer;
using WeifenLuo.WinFormsUI.Docking;
using Medical.GUI;
using Logging;

namespace Medical
{
    public delegate void DrawingWindowEvent(DrawingWindow window);

    public class DrawingWindowController : IDisposable
    {
        public event DrawingWindowEvent WindowCreated;
        public event DrawingWindowEvent WindowDestroyed;

        private Dictionary<String, DrawingWindowHost> cameras = new Dictionary<String, DrawingWindowHost>();
        private bool camerasActive = false;
        private bool showStatsActive = false;
        private UpdateTimer mainTimer;
        private SimScene scene;
        private EventManager eventManager;
        private RendererPlugin rendererPlugin;
        private DockPanel dock;
        private DrawingWindowHost activeDrawingWindow = null;
        private bool allowRotation = true;
        private bool allowZoom = true;

        public DrawingWindowController()
        {
            
        }

        public void Dispose()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.Dispose();
                host.Dispose();
            }
        }

        public void initialize(DockPanel dock, EventManager eventManager, RendererPlugin rendererPlugin, ConfigFile configFile)
        {
            this.dock = dock;
            dock.ActiveDocumentChanged += new EventHandler(dock_ActiveDocumentChanged);
            this.eventManager = eventManager;
            this.rendererPlugin = rendererPlugin;
        }

        public void recreateAllWindows()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.recreateWindow();
            }
        }

        public void createFromPresets(DrawingWindowPresetSet presets)
        {
            closeAllWindows();
            DrawingWindowHost camera;
            foreach(DrawingWindowPreset preset in presets.getPresetEnum())
            {
                camera = addCamera(preset.Name, preset.Position, preset.LookAt);
            }
            DrawingWindowHost parent;
            foreach (DrawingWindowPreset preset in presets.getPresetEnum())
            {
                camera = cameras[preset.Name];
                parent = null;
                if (preset.ParentWindow != null)
                {
                    cameras.TryGetValue(preset.ParentWindow, out parent);
                    if (parent == null)
                    {
                        Log.Warning("Could not find parent window {0} for window {1}.", preset.ParentWindow, preset.Name);
                    }
                }
                if (parent == null)
                {
                    camera.Show(dock);
                }
                else
                {
                    switch (preset.WindowPosition)
                    {
                        case DrawingWindowPosition.Bottom:
                            camera.Show(parent.Pane, DockAlignment.Bottom, 0.5);
                            break;
                        case DrawingWindowPosition.Top:
                            camera.Show(parent.Pane, DockAlignment.Top, 0.5);
                            break;
                        case DrawingWindowPosition.Left:
                            camera.Show(parent.Pane, DockAlignment.Left, 0.5);
                            break;
                        case DrawingWindowPosition.Right:
                            camera.Show(parent.Pane, DockAlignment.Right, 0.5);
                            break;
                    }
                }
            }
        }

        public DrawingWindowHost createDrawingWindowHost(String name, Vector3 translation, Vector3 lookAt)
        {
            return addCamera(name, translation, lookAt);
        }

        public void destroyCameras()
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.destroyCamera();
            }
            camerasActive = false;
            mainTimer = null;
            scene = null;
        }

        public void createCameras(UpdateTimer mainTimer, SimScene scene, String sceneDirectory)
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.createCamera(mainTimer, scene);
            }
            camerasActive = true;
            this.mainTimer = mainTimer;
            this.scene = scene;
        }

        public void showStats(bool show)
        {
            foreach (DrawingWindowHost host in cameras.Values)
            {
                host.DrawingWindow.showStats(show);
            }
            showStatsActive = show;
        }

        public DrawingWindowHost getActiveWindow()
        {
            return activeDrawingWindow;
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
            }
        }

        internal void _alertCameraDestroyed(DrawingWindowHost host)
        {
            if (WindowDestroyed != null)
            {
                WindowDestroyed.Invoke(host.DrawingWindow);
            }
            cameras.Remove(host.DrawingWindow.CameraName);
        }

        private void dock_ActiveDocumentChanged(object sender, EventArgs e)
        {
            DrawingWindowHost changed = dock.ActiveDocument as DrawingWindowHost;
            if (changed != null)
            {
                activeDrawingWindow = changed;
            }
        }

        private DrawingWindowHost addCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            DrawingWindowHost cameraHost = new DrawingWindowHost(name, this);
            OrbitCameraController orbitCamera = new OrbitCameraController(translation, lookAt, cameraHost.DrawingWindow, eventManager);
            orbitCamera.AllowRotation = allowRotation;
            orbitCamera.AllowZoom = allowZoom;
            cameraHost.DrawingWindow.initialize(name, orbitCamera, rendererPlugin, translation, lookAt);
            cameras.Add(cameraHost.DrawingWindow.CameraName, cameraHost);

            if (WindowCreated != null)
            {
                WindowCreated.Invoke(cameraHost.DrawingWindow);
            }

            if (camerasActive)
            {
                cameraHost.DrawingWindow.createCamera(mainTimer, scene);
            }
            cameraHost.DrawingWindow.showStats(showStatsActive);
            return cameraHost;
        }

        private void closeAllWindows()
        {
            List<DrawingWindowHost> listCopy = new List<DrawingWindowHost>(cameras.Values);
            foreach (DrawingWindowHost host in listCopy)
            {
                host.Close();
            }
        }
    }
}