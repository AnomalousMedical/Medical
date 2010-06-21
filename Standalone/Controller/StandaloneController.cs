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
using OgrePlugin;
using OgreWrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using MyGUIPlugin;
using Medical.GUI;

namespace Standalone
{
    public delegate void SceneEvent(SimScene scene);

    class StandaloneController : IDisposable
    {
        //Events
        public event SceneEvent SceneLoaded;
        public event SceneEvent SceneUnloading;

        //Members
        private MedicalController medicalController;
        private WindowListener windowListener;
        private SceneView camera;

        private BasicGUI basicGUI;

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

            basicGUI = new BasicGUI(this);

            if (medicalController.openScene(MedicalConfig.DefaultScene))
            {                
                createCamera(medicalController.PluginManager.RendererPlugin.PrimaryWindow, medicalController.MainTimer, medicalController.CurrentScene);

                if (SceneLoaded != null)
                {
                    SceneLoaded.Invoke(medicalController.CurrentScene);
                }

                medicalController.start();
            }
        }

        public void shutdown()
        {
            medicalController.MainTimer.stopLoop();
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
                mainTimer.addFixedUpdateListener(cameraController);

                camera = window.createSceneView(defaultScene, "Default", new Vector3(0, -5, 150), new Vector3(0, -5, 0));
                camera.BackgroundColor = Engine.Color.Black;
                camera.addLight();
                camera.setNearClipDistance(1.0f);
                camera.setFarClipDistance(1000.0f);
                //camera.setRenderingMode(renderingMode);
                cameraController.setCamera(camera);
                //CameraResolver.addMotionValidator(this);
                //camera.showSceneStats(true);
                //camera.setDimensions(0.3f, 0.0f, 0.7f, 1.0f);
                basicGUI.ScreenLayout.Root.Center = new SceneViewLayoutItem(camera);
                //OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
                //ogreCamera.PreFindVisibleObjects += camera_PreFindVisibleObjects;
                //if (CameraCreated != null)
                //{
                //    CameraCreated.Invoke(this);
                //}

                MyGUIInterface myGui = this.MedicalController.PluginManager.getPlugin("MyGUIPlugin") as MyGUIInterface;
                OgreRenderManager rm = myGui.OgrePlatform.getRenderManager();
                rm.setActiveViewport(1);
            }
            else
            {
                Log.Default.sendMessage("Cannot find default subscene for the scene. Not creating camera.", LogLevel.Error, "Anomaly");
            }
        }

        public MedicalController MedicalController
        {
            get
            {
                return medicalController;
            }
        }
    }
}

/*
static void DebugStructureAlignment(object structure) {
            var t = structure.GetType();
            if (t.IsValueType) {
                Console.WriteLine("Offset  Length  Field");
                int realTotal = 0;
                foreach (var iField in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                    Console.Write(Marshal.OffsetOf(t, iField.Name).ToString().PadLeft(6));
                    Console.Write("  ");
                    int size = Marshal.SizeOf(iField.GetValue(structure));
                    realTotal += size;
                    Console.Write(size.ToString().PadLeft(6));
                    Console.Write("  ");
                    Console.WriteLine(iField.Name);
                }
                Console.WriteLine("        " + Marshal.SizeOf(structure).ToString().PadLeft(6) + " bytes total");
                Console.WriteLine("        " + realTotal.ToString().PadLeft(6) + " bytes total (data without padding)");
            }
        }
*/