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

namespace Medical
{
    public delegate void DrawingWindowEvent(DrawingWindow window);

    public class DrawingWindowController : IDisposable
    {
        public event DrawingWindowEvent WindowCreated;
        public event DrawingWindowEvent WindowDestroyed;

        private List<DrawingWindowHost> cameras = new List<DrawingWindowHost>();
        private bool camerasActive = false;
        private bool showStatsActive = false;
        private UpdateTimer mainTimer;
        private SimScene scene;
        private EventManager eventManager;
        private RendererPlugin rendererPlugin;
        private DockPanel dock;
        private DrawingWindowHost activeDrawingWindow = null;
        private bool allowRotation = true;

        public DrawingWindowController()
        {
            
        }

        public void initialize(DockPanel dock, EventManager eventManager, RendererPlugin rendererPlugin, ConfigFile configFile)
        {
            this.dock = dock;
            dock.ActiveDocumentChanged += new EventHandler(dock_ActiveDocumentChanged);
            this.eventManager = eventManager;
            this.rendererPlugin = rendererPlugin;
        }

        void dock_ActiveDocumentChanged(object sender, EventArgs e)
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
            cameraHost.DrawingWindow.initialize(name, eventManager, rendererPlugin, translation, lookAt);
            cameraHost.DrawingWindow.AllowRotation = allowRotation;
            cameras.Add(cameraHost);
            
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

        public void recreateAllWindows()
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.recreateWindow();
            }
        }

        public void createFromPresets(DrawingWindowPresetSet presets)
        {
            closeAllWindows();
            foreach(DrawingWindowPreset preset in presets.getPresetEnum())
            {
                DrawingWindowHost camera = addCamera(preset.Name, preset.Position, preset.LookAt);
            }
            foreach (DrawingWindowHost host in cameras)
            {
                host.Show(dock);
            }
        }

        public DrawingWindowHost createDrawingWindowHost(String name, Vector3 translation, Vector3 lookAt)
        {
            return addCamera(name, translation, lookAt);
        }

        public void destroyCameras()
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.destroyCamera();
            }
            camerasActive = false;
            mainTimer = null;
            scene = null;
        }

        public void createCameras(UpdateTimer mainTimer, SimScene scene, String sceneDirectory)
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.createCamera(mainTimer, scene);
            }
            camerasActive = true;
            this.mainTimer = mainTimer;
            this.scene = scene;
        }

        public void showStats(bool show)
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.showStats(show);
            }
            showStatsActive = show;
        }

        public DrawingWindowHost getActiveWindow()
        {
            return activeDrawingWindow;
        }

        internal void _alertCameraDestroyed(DrawingWindowHost host)
        {
            if (WindowDestroyed != null)
            {
                WindowDestroyed.Invoke(host.DrawingWindow);
            }
            cameras.Remove(host);
        }

        private void closeAllWindows()
        {
            List<DrawingWindowHost> listCopy = new List<DrawingWindowHost>(cameras);
            foreach (DrawingWindowHost host in listCopy)
            {
                host.Close();
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
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.Dispose();
                host.Dispose();
            }
        }

        #endregion
    }
}