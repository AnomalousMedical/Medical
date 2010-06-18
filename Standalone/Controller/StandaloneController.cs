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
        private ScreenLayoutManager screenLayoutManager;
        private SceneView camera;
        private LayerGUIController layerGUIController;
        private MandibleGUIController mandibleGUIController;

        public StandaloneController()
        {
            MedicalConfig config = new MedicalConfig(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Anomalous Medical/Articulometrics/Standalone");
        }

        public void Dispose()
        {
            layerGUIController.Dispose();
            medicalController.Dispose();
        }

        public void go()
        {
            medicalController = new MedicalController();
            medicalController.initialize(null, new AgnosticMessagePump(), createWindow);
            windowListener = new WindowListener(medicalController);
            medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle.addListener(windowListener);
            screenLayoutManager = new ScreenLayoutManager(medicalController.PluginManager.RendererPlugin.PrimaryWindow.Handle);

            MyGUIInterface myGui = medicalController.PluginManager.getPlugin("MyGUIPlugin") as MyGUIInterface;

            Gui gui = Gui.Instance;
            gui.setVisiblePointer(false);

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Layouts", "EngineArchive", "MyGUI", true);
            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Imagesets", "EngineArchive", "MyGUI", true);

            LanguageManager.Instance.loadUserTags("core_theme_black_blue_tag.xml");
            gui.load("core_skin.xml");
            gui.load("LayersToolstrip.xml");

            Layout ribbon = LayoutManager.Instance.loadLayout("Ribbon.layout");
            screenLayoutManager.Root.Top = new MyGUILayoutContainer(ribbon.getWidget(0));
            layerGUIController = new LayerGUIController(gui);
            mandibleGUIController = new MandibleGUIController(gui, this);

            Button quitButton = gui.findWidgetT("File/Quit") as Button;
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);

            Button layerButton = gui.findWidgetT("Layers/Skin") as Button;
            StaticImage image = layerButton.StaticImage;
            image.setItemResource("LayersToolstripIcons");
            image.setItemGroup("Icons");
            image.setItemName("Skin");
            Console.WriteLine(image);

            if (medicalController.openScene(MedicalConfig.DefaultScene))
            {                
                createCamera(medicalController.PluginManager.RendererPlugin.PrimaryWindow, medicalController.MainTimer, medicalController.CurrentScene);

                screenLayoutManager.layout();

                OgreRenderManager rm = myGui.OgrePlatform.getRenderManager();
                rm.setActiveViewport(1);

                if (SceneLoaded != null)
                {
                    SceneLoaded.Invoke(medicalController.CurrentScene);
                }

                medicalController.start();
            }
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
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