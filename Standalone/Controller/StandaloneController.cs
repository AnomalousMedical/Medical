using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Renderer;
using Engine.ObjectManagement;
using Engine;
using Engine.Platform;
using Logging;
using Medical;
using PCPlatform;

namespace Standalone
{
    class StandaloneController : IDisposable
    {
        private MedicalController medicalController;
        private WindowListener windowListener;

        public StandaloneController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/Articulometrics/Standalone");
        }

        public void Dispose()
        {
            medicalController.Dispose();
        }

        public void go()
        {
            medicalController = new MedicalController();
            medicalController.initialize(null, new AgnosticMessagePump(), createWindow);
            windowListener = new WindowListener(medicalController);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);

            medicalController.openScene(MedicalConfig.DefaultScene);
            
            createCamera(medicalController.PluginManager.RendererPlugin.PrimaryWindow, medicalController.MainTimer, medicalController.CurrentScene);

            medicalController.start();
        }

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out DefaultWindowInfo defaultWindow)
        {
            defaultWindow = new DefaultWindowInfo("Articulometrics", MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            defaultWindow.Fullscreen = MedicalConfig.EngineConfig.Fullscreen;
            defaultWindow.MonitorIndex = 0;
        }

        public void createCamera(RendererWindow window, UpdateTimer mainTimer, SimScene scene)
        {
            OrbitCameraController cameraController = new OrbitCameraController(new Vector3(0, -5, 150), new Vector3(0, -5, 0), null, medicalController.EventManager);
            SimSubScene defaultScene = scene.getDefaultSubScene();
            if (defaultScene != null)
            {
                CameraControl camera = window.createCamera(defaultScene, "Default", new Vector3(0, -5, 150), new Vector3(0, -5, 0));
                camera.BackgroundColor = Engine.Color.Black;
                camera.addLight();
                camera.setNearClipDistance(1.0f);
                camera.setFarClipDistance(1000.0f);
                //camera.setRenderingMode(renderingMode);
                mainTimer.addFixedUpdateListener(cameraController);
                cameraController.setCamera(camera);
                //CameraResolver.addMotionValidator(this);
                camera.showSceneStats(true);
                //OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
                //ogreCamera.PreFindVisibleObjects += camera_PreFindVisibleObjects;
                //if (CameraCreated != null)
                //{
                //    CameraCreated.Invoke(this);
                //}
            }
            else
            {
                Log.Default.sendMessage("Cannot find default subscene for the scene. Not creating camera.", LogLevel.Error, "Anomaly");
            }
        }
    }
}
