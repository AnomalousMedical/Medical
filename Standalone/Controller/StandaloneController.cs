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
using CEGUIPlugin;
using OgrePlugin;
using OgreWrapper;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Standalone
{
    class StandaloneController : IDisposable
    {
        private MedicalController medicalController;
        private WindowListener windowListener;
        private ScreenLayoutManager screenLayoutManager;

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
            screenLayoutManager = new ScreenLayoutManager(medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);

            
            String resourcePath = "TEMPCEGUI/";

            //temp initialize ogre resources for cegui
            OgreResourceGroupManager rgm = OgreResourceGroupManager.getInstance();
            rgm.createResourceGroup("imagesets");
            rgm.createResourceGroup("fonts");
            rgm.createResourceGroup("layouts");
            rgm.createResourceGroup("schemes");
            rgm.createResourceGroup("looknfeels");
            rgm.createResourceGroup("lua_scripts");
            rgm.createResourceGroup("schemas");
            rgm.addResourceLocation(resourcePath + "schemes", "EngineArchive", "schemes", true);
            rgm.addResourceLocation(resourcePath + "imagesets", "EngineArchive", "imagesets", true);
            rgm.addResourceLocation(resourcePath + "fonts", "EngineArchive", "fonts", true);
            rgm.addResourceLocation(resourcePath + "layouts", "EngineArchive", "layouts", true);
            rgm.addResourceLocation(resourcePath + "looknfeel", "EngineArchive", "looknfeels", true);
            //rgm.addResourceLocation(resourcePath + "lua_scripts", "EngineArchive", "lua_scripts", true);
            //rgm.addResourceLocation(resourcePath + "xml_schemas", "EngineArchive", "schemas", true);

            rgm.initializeAllResourceGroups();

            if (medicalController.openScene(MedicalConfig.DefaultScene))
            {
                SchemeManager.Singleton.create("AnomalousLook.scheme");

                Window root = WindowManager.Singleton.createWindow("DefaultWindow", "Root");
                CEGUISystem.Instance.setGUISheet(root);

                Window leftLayout = WindowManager.Singleton.loadWindowLayout("left.layout");
                root.addChildWindow(leftLayout);
                PushButton button = leftLayout.getChildRecursive("Root/Window/Button") as PushButton;
                CEGUIEvent evt = new CEGUIEvent(button_TestEvent);
                button.Clicked += evt;
                screenLayoutManager.Root.Left = new CEGUILayoutItem(leftLayout);
                
                createCamera(medicalController.PluginManager.RendererPlugin.PrimaryWindow, medicalController.MainTimer, medicalController.CurrentScene);

                screenLayoutManager.layout();

                medicalController.start();
            }
            
        }

        void button_TestEvent(CEGUIPlugin.EventArgs e)
        {
            Log.Debug("Event recieved standalone.");
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

                SceneView camera = window.createSceneView(defaultScene, "Default", new Vector3(0, -5, 150), new Vector3(0, -5, 0));
                camera.BackgroundColor = Engine.Color.Black;
                camera.addLight();
                camera.setNearClipDistance(1.0f);
                camera.setFarClipDistance(1000.0f);
                //camera.setRenderingMode(renderingMode);
                cameraController.setCamera(camera);
                //CameraResolver.addMotionValidator(this);
                camera.showSceneStats(true);
                //camera.setDimensions(0.3f, 0.0f, 0.7f, 1.0f);
                screenLayoutManager.Root.Center = new SceneViewLayoutItem(camera);
                //OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
                //ogreCamera.PreFindVisibleObjects += camera_PreFindVisibleObjects;
                //if (CameraCreated != null)
                //{
                //    CameraCreated.Invoke(this);
                //}

                //create a secondary camera
                //SceneView camera2 = window.createSceneView(defaultScene, "Default2", new Vector3(0, -5, 150), new Vector3(0, -5, 0));
                //camera2.BackgroundColor = Engine.Color.Black;
                //camera2.addLight();
                //camera2.setNearClipDistance(1.0f);
                //camera2.setFarClipDistance(1000.0f);
                ////camera.setRenderingMode(renderingMode);
                ////cameraController.setCamera(camera2);
                ////CameraResolver.addMotionValidator(this);
                //camera2.showSceneStats(true);
                //camera2.setDimensions(0.3f, 0.3f, 0.5f, 0.5f);
                ////OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
                ////ogreCamera.PreFindVisibleObjects += camera_PreFindVisibleObjects;
                ////if (CameraCreated != null)
                ////{
                ////    CameraCreated.Invoke(this);
                ////}
            }
            else
            {
                Log.Default.sendMessage("Cannot find default subscene for the scene. Not creating camera.", LogLevel.Error, "Anomaly");
            }
        }

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
    }
}
