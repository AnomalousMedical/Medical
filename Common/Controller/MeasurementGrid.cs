using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.Platform;
using Engine.ObjectManagement;
using OgreWrapper;
using OgrePlugin;

namespace Medical
{
    public class MeasurementGrid
    {
        private PluginManager pluginManager;
        private String name;
        private ManualObject manualObject;
        private SceneNode sceneNode;
        private EventManager events;
        private DrawingWindowController drawingWindowController;
        private bool visible = true;

        public MeasurementGrid(String name, MedicalController medicalController, DrawingWindowController drawingWindowController)
        {
            this.name = name;
            this.pluginManager = medicalController.PluginManager;
            this.events = medicalController.EventManager;
            this.drawingWindowController = drawingWindowController;
            drawingWindowController.WindowCreated += new DrawingWindowEvent(drawingWindowController_WindowCreated);
        }

        void drawingWindowController_WindowCreated(DrawingWindow window)
        {
            window.PreFindVisibleObjects += new DrawingWindowRenderEvent(window_PreFindVisibleObjects);
        }

        void window_PreFindVisibleObjects(DrawingWindow window, bool currentCameraRender)
        {
            if (currentCameraRender)
            {
                sceneNode.setOrientation(window.Orientation);
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null)
            {
                OgreSceneManager sceneManager = subScene.getSimElementManager<OgreSceneManager>();
                manualObject = sceneManager.SceneManager.createManualObject(name + "__MeasurementManualObject");
                sceneNode = sceneManager.SceneManager.createSceneNode(name + "__MeasurementManualObjectSceneNode");
                sceneNode.attachObject(manualObject);
                sceneNode.setVisible(visible);
                sceneManager.SceneManager.getRootSceneNode().addChild(sceneNode);
                drawGrid(5.0f, 25.0f);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null && manualObject != null)
            {
                OgreSceneManager sceneManager = subScene.getSimElementManager<OgreSceneManager>();
                sceneManager.SceneManager.getRootSceneNode().removeChild(sceneNode);
                sceneNode.detachObject(manualObject);
                sceneManager.SceneManager.destroyManualObject(manualObject);
                sceneManager.SceneManager.destroySceneNode(sceneNode);
                manualObject = null;
                sceneNode = null;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                sceneNode.setVisible(value);
            }
        }

        private void drawGrid(float gridSpacingMM, float gridMax)
        {
            float gridSpacing = gridSpacingMM * (1.0f / Measurement.unitsToMM);
            Vector3 startPoint = new Vector3(0.0f, -gridMax, 0.0f);
            Vector3 endPoint = new Vector3(0.0f, gridMax, 0.0f);
            manualObject.clear();
            manualObject.begin("Grid", OperationType.OT_LINE_LIST);
            Color color = Color.FromARGB(System.Drawing.Color.Green.ToArgb());
            for (float x = 0; x < gridMax; x += gridSpacing)
            {
                startPoint.x = x;
                endPoint.x = x;
                manualObject.position(ref startPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
                manualObject.position(ref endPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
            }
            for (float x = -gridSpacing; x > -gridMax; x -= gridSpacing)
            {
                startPoint.x = x;
                endPoint.x = x;
                manualObject.position(ref startPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
                manualObject.position(ref endPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
            }
            startPoint.x = -gridMax;
            endPoint.x = gridMax;
            for (float y = 0; y < gridMax; y += gridSpacing)
            {
                startPoint.y = y;
                endPoint.y = y;
                manualObject.position(ref startPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
                manualObject.position(ref endPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
            }
            for (float y = -gridSpacing; y > -gridMax; y -= gridSpacing)
            {
                startPoint.y = y;
                endPoint.y = y;
                manualObject.position(ref startPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
                manualObject.position(ref endPoint);
                manualObject.color(color.r, color.g, color.b, color.a);
            }
            manualObject.end();
        }
    }
}
