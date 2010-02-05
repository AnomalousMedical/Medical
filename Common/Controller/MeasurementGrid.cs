using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.Platform;
using Engine.ObjectManagement;

namespace Medical
{
    public class MeasurementGrid
    {
        private PluginManager pluginManager;
        private String name;
        private DebugDrawingSurface drawingSurface = null;
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
                drawingSurface.setOrientation(window.Orientation);
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null)
            {
                drawingSurface = pluginManager.RendererPlugin.createDebugDrawingSurface(name + "MeasurementGridSurface", subScene);
                drawingSurface.setVisible(visible);
                drawGrid(5.0f * (1.0f / Measurement.unitsToMM), 25.0f);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            if (drawingSurface != null)
            {
                pluginManager.RendererPlugin.destroyDebugDrawingSurface(drawingSurface);
                drawingSurface = null;
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
                drawingSurface.setVisible(value);
            }
        }

        private void drawGrid(float gridSpacing, float gridMax)
        {
            Vector3 startPoint = new Vector3(0.0f, -gridMax, 0.0f);
            Vector3 endPoint = new Vector3(0.0f, gridMax, 0.0f);
            drawingSurface.begin("Grid", DrawingType.LineList);
            drawingSurface.setColor(Color.FromARGB(System.Drawing.Color.Green.ToArgb()));
            for (float x = 0; x < gridMax; x += gridSpacing)
            {
                startPoint.x = x;
                endPoint.x = x;
                drawingSurface.drawLine(startPoint, endPoint);
            }
            for (float x = -gridSpacing; x > -gridMax; x -= gridSpacing)
            {
                startPoint.x = x;
                endPoint.x = x;
                drawingSurface.drawLine(startPoint, endPoint);
            }
            startPoint.x = -gridMax;
            endPoint.x = gridMax;
            for (float y = 0; y < gridMax; y += gridSpacing)
            {
                startPoint.y = y;
                endPoint.y = y;
                drawingSurface.drawLine(startPoint, endPoint);
            }
            for (float y = -gridSpacing; y > -gridMax; y -= gridSpacing)
            {
                startPoint.y = y;
                endPoint.y = y;
                drawingSurface.drawLine(startPoint, endPoint);
            }
            drawingSurface.end();
        }
    }
}
