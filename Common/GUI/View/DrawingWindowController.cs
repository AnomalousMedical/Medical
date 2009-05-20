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

namespace Medical
{
    public class DrawingWindowController
    {
        private MedicalController controller;
        private List<DrawingWindowHost> cameras = new List<DrawingWindowHost>();
        private bool camerasActive = false;
        private bool showStatsActive = false;
        private UpdateTimer mainTimer;
        private SimScene scene;
        private EventManager eventManager;
        private RendererPlugin rendererPlugin;
        private CameraSection cameraSection;

        public DrawingWindowController()
        {

        }

        public void initialize(MedicalController controller, EventManager eventManager, RendererPlugin rendererPlugin, ConfigFile configFile)
        {
            this.cameraSection = new CameraSection(configFile);
            this.controller = controller;
            this.eventManager = eventManager;
            this.rendererPlugin = rendererPlugin;
        }

        private DrawingWindowHost addCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            DrawingWindowHost cameraHost = new DrawingWindowHost(name, this);
            cameraHost.DrawingWindow.initialize(name, eventManager, rendererPlugin, translation, lookAt, this);
            cameras.Add(cameraHost);
            if (camerasActive)
            {
                cameraHost.DrawingWindow.createCamera(mainTimer, scene);
            }
            cameraHost.DrawingWindow.showStats(showStatsActive);
            return cameraHost;
        }

        public void createFourWaySplit()
        {
            closeAllWindows();
            DrawingWindowHost camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            controller.showDockContent(camera1);
            DrawingWindowHost camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            camera2.Show(camera1.Pane, DockAlignment.Right, 0.5);
            DrawingWindowHost camera3 = addCamera("Camera 3", cameraSection.RightCameraPosition, cameraSection.RightCameraLookAt);
            camera3.Show(camera1.Pane, DockAlignment.Bottom, 0.5);
            DrawingWindowHost camera4 = addCamera("Camera 4", cameraSection.LeftCameraPosition, cameraSection.LeftCameraLookAt);
            camera4.Show(camera2.Pane, DockAlignment.Bottom, 0.5);
        }

        public void createThreeWayUpperSplit()
        {
            closeAllWindows();
            DrawingWindowHost camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            controller.showDockContent(camera1);
            DrawingWindowHost camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            camera2.Show(camera1.Pane, DockAlignment.Bottom, 0.5);
            DrawingWindowHost camera3 = addCamera("Camera 3", cameraSection.RightCameraPosition, cameraSection.RightCameraLookAt);
            camera3.Show(camera2.Pane, DockAlignment.Right, 0.5);
        }

        public void createTwoWaySplit()
        {
            closeAllWindows();
            DrawingWindowHost camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            controller.showDockContent(camera1);
            DrawingWindowHost camera2 = addCamera("Camera 2", cameraSection.BackCameraPosition, cameraSection.BackCameraLookAt);
            camera2.Show(camera1.Pane, DockAlignment.Right, 0.5);
        }

        public void createOneWaySplit()
        {
            closeAllWindows();
            DrawingWindowHost camera1 = addCamera("Camera 1", cameraSection.FrontCameraPosition, cameraSection.FrontCameraLookAt);
            controller.showDockContent(camera1);
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

        public void createCameras(UpdateTimer mainTimer, SimScene scene)
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
    }
}
