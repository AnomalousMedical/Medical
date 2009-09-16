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
    public class DrawingWindowController : IDisposable
    {
        private List<DrawingWindowHost> cameras = new List<DrawingWindowHost>();
        private bool camerasActive = false;
        private bool showStatsActive = false;
        private UpdateTimer mainTimer;
        private SimScene scene;
        private EventManager eventManager;
        private RendererPlugin rendererPlugin;
        private SavedCameraController userCameras;
        private SavedCameraController sceneCameras = new SavedCameraController();
        private DockPanel dock;
        private DrawingWindowHost activeDrawingWindow = null;
        private bool watermarks = false;
        private bool allowRotation = true;

        public DrawingWindowController(String camerasFile)
        {
            userCameras = new SavedCameraController(camerasFile);
        }

        public void initialize(DockPanel dock, EventManager eventManager, RendererPlugin rendererPlugin, ConfigFile configFile)
        {
            this.dock = dock;
            dock.ActiveDocumentChanged += new EventHandler(dock_ActiveDocumentChanged);
            this.eventManager = eventManager;
            this.rendererPlugin = rendererPlugin;
        }

        public void showWatermarks(bool show)
        {
            if (watermarks != show)
            {
                watermarks = show;
                if (watermarks)
                {
                    foreach (DrawingWindowHost host in cameras)
                    {
                        host.DrawingWindow.createWatermark();
                    }
                }
                else
                {
                    foreach (DrawingWindowHost host in cameras)
                    {
                        host.DrawingWindow.destroyWatermark();
                    }
                }
            }
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
            if (camerasActive)
            {
                cameraHost.DrawingWindow.createCamera(mainTimer, scene);
            }
            cameraHost.DrawingWindow.showStats(showStatsActive);
            if (watermarks)
            {
                cameraHost.DrawingWindow.createWatermark();
            }
            return cameraHost;
        }

        public void recreateAllWindows()
        {
            foreach (DrawingWindowHost host in cameras)
            {
                host.DrawingWindow.recreateWindow();
            }
        }

        public void createFourWaySplit()
        {
            CameraSection cameraSection = MedicalConfig.CameraSection;
            SavedCameraDefinition camera;
            closeAllWindows();

            DrawingWindowHost camera1;
            if (sceneCameras.hasSavedCamera("Front"))
            {
                camera = sceneCameras.getSavedCamera("Front");
                camera1 = addCamera("Camera 1", camera.Position, camera.LookAt);
            }
            else
            {
                camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            }
            camera1.Show(dock);

            DrawingWindowHost camera2;
            if (sceneCameras.hasSavedCamera("Back"))
            {
                camera = sceneCameras.getSavedCamera("Back");
                camera2 = addCamera("Camera 2", camera.Position, camera.LookAt);
            }
            else
            {
                camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            }
            camera2.Show(camera1.Pane, DockAlignment.Right, 0.5);

            DrawingWindowHost camera3;
            if (sceneCameras.hasSavedCamera("Right"))
            {
                camera = sceneCameras.getSavedCamera("Right");
                camera3 = addCamera("Camera 3", camera.Position, camera.LookAt);
            }
            else
            {
                camera3 = addCamera("Camera 3", cameraSection.RightCameraPosition, cameraSection.RightCameraLookAt);
            }
            camera3.Show(camera1.Pane, DockAlignment.Bottom, 0.5);

            DrawingWindowHost camera4;
            if (sceneCameras.hasSavedCamera("Left"))
            {
                camera = sceneCameras.getSavedCamera("Left");
                camera4 = addCamera("Camera 4", camera.Position, camera.LookAt);
            }
            else
            {
                camera4 = addCamera("Camera 4", cameraSection.LeftCameraPosition, cameraSection.LeftCameraLookAt);
            }
            camera4.Show(camera2.Pane, DockAlignment.Bottom, 0.5);
        }

        public void createThreeWayUpperSplit()
        {
            CameraSection cameraSection = MedicalConfig.CameraSection;
            SavedCameraDefinition camera;
            closeAllWindows();

            DrawingWindowHost camera1;
            if (sceneCameras.hasSavedCamera("Front"))
            {
                camera = sceneCameras.getSavedCamera("Front");
                camera1 = addCamera("Camera 1", camera.Position, camera.LookAt);
            }
            else
            {
                camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            }
            camera1.Show(dock);

            DrawingWindowHost camera2;
            if (sceneCameras.hasSavedCamera("Back"))
            {
                camera = sceneCameras.getSavedCamera("Back");
                camera2 = addCamera("Camera 2", camera.Position, camera.LookAt);
            }
            else
            {
                camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            }
            camera2.Show(camera1.Pane, DockAlignment.Bottom, 0.5);

            DrawingWindowHost camera3;
            if (sceneCameras.hasSavedCamera("Right"))
            {
                camera = sceneCameras.getSavedCamera("Right");
                camera3 = addCamera("Camera 3", camera.Position, camera.LookAt);
            }
            else
            {
                camera3 = addCamera("Camera 3", cameraSection.RightCameraPosition, cameraSection.RightCameraLookAt);
            }
            camera3.Show(camera2.Pane, DockAlignment.Right, 0.5);
        }

        public void createTwoWaySplit()
        {
            CameraSection cameraSection = MedicalConfig.CameraSection;
            SavedCameraDefinition camera;
            closeAllWindows();

            DrawingWindowHost camera1;
            if (sceneCameras.hasSavedCamera("Front"))
            {
                camera = sceneCameras.getSavedCamera("Front");
                camera1 = addCamera("Camera 1", camera.Position, camera.LookAt);
            }
            else
            {
                camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            }
            camera1.Show(dock);

            DrawingWindowHost camera2;
            if (sceneCameras.hasSavedCamera("Back"))
            {
                camera = sceneCameras.getSavedCamera("Back");
                camera2 = addCamera("Camera 2", camera.Position, camera.LookAt);
            }
            else
            {
                camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            }
            camera2.Show(camera1.Pane, DockAlignment.Right, 0.5);
        }

        public void createOneWaySplit()
        {
            CameraSection cameraSection = MedicalConfig.CameraSection;
            SavedCameraDefinition camera;
            closeAllWindows();

            DrawingWindowHost camera1;
            if (sceneCameras.hasSavedCamera("Front"))
            {
                camera = sceneCameras.getSavedCamera("Front");
                camera1 = addCamera("Camera 1", camera.Position, camera.LookAt);
            }
            else
            {
                camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            }
            camera1.Show(dock);
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
            SimSubScene defaultScene = scene.getDefaultSubScene();
            if (defaultScene != null)
            {
                SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                sceneCameras.changeBackingFile(sceneDirectory + '/' + medicalScene.CameraFile);
            }
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

        public void restoreSavedCamera(String cameraName)
        {
            DrawingWindowHost activeWindow = dock.ActiveDocument as DrawingWindowHost;
            if (activeWindow != null && userCameras.hasSavedCamera(cameraName))
            {
                SavedCameraDefinition cameraDef = userCameras.getSavedCamera(cameraName);
                activeWindow.DrawingWindow.setCamera(cameraDef.Position, cameraDef.LookAt);
            }
        }

        public void restorePredefinedCamera(String cameraName)
        {
            DrawingWindowHost activeWindow = dock.ActiveDocument as DrawingWindowHost;
            if (activeWindow != null && sceneCameras.hasSavedCamera(cameraName))
            {
                SavedCameraDefinition camera = sceneCameras.getSavedCamera(cameraName);
                activeWindow.DrawingWindow.setCamera(camera.Position, camera.LookAt);
            }
        }

        public DrawingWindowHost getActiveWindow()
        {
            return activeDrawingWindow;
        }

        public IEnumerable<String> getSavedCameraNames()
        {
            return userCameras.getSavedCameraNames();
        }

        public IEnumerable<String> getSceneCameraNames()
        {
            return sceneCameras.getSavedCameraNames();
        }

        public bool saveCamera(String name)
        {
            DrawingWindowHost activeWindow = dock.ActiveDocument as DrawingWindowHost;
            if (activeWindow != null)
            {
                SavedCameraDefinition cam = new SavedCameraDefinition(name, activeWindow.DrawingWindow.Translation, activeWindow.DrawingWindow.LookAt);
                userCameras.addOrUpdateSavedCamera(cam);
                return true;
            }
            return false;
        }

        public bool destroySavedCamera(String name)
        {
            return userCameras.removeSavedCamera(name);
        }

        public bool hasSavedCamera(String name)
        {
            return userCameras.hasSavedCamera(name);
        }

        public void saveCameraFile()
        {
            userCameras.saveCameras();
        }

        internal void _alertCameraDestroyed(DrawingWindowHost host)
        {
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
