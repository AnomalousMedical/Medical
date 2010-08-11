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

namespace Medical.Controller
{
    public class MeasurementGrid : IDisposable
    {
        private PluginManager pluginManager;
        private String name;
        private ManualObject manualObject;
        private SceneNode sceneNode;
        private EventManager events;
        private SceneViewController sceneViewController;
        private bool visible = false;
        private TextWatermark textWatermark;
        private Color color = Color.FromARGB(System.Drawing.Color.Green.ToArgb());
        private Vector3 origin = Vector3.Zero;

        public MeasurementGrid(String name, MedicalController medicalController, SceneViewController sceneViewController)
        {
            this.name = name;
            this.pluginManager = medicalController.PluginManager;
            this.events = medicalController.EventManager;
            this.sceneViewController = sceneViewController;
            sceneViewController.WindowCreated += new SceneViewWindowEvent(sceneViewController_WindowCreated);
            sceneViewController.WindowDestroyed += new SceneViewWindowEvent(sceneViewController_WindowDestroyed);
            textWatermark = new TextWatermark("MeasurementAmount", "Grid Spacing: 0 mm", 15.0f, GuiVerticalAlignment.GVA_TOP);
        }

        public void Dispose()
        {
            textWatermark.Dispose();
        }

        public void sceneLoaded(SimScene scene)
        {
            SimSubScene subScene = scene.getDefaultSubScene();
            if (subScene != null)
            {
                OgreSceneManager sceneManager = subScene.getSimElementManager<OgreSceneManager>();
                manualObject = sceneManager.SceneManager.createManualObject(name + "__MeasurementManualObject");
                manualObject.setRenderQueueGroup(95);
                sceneNode = sceneManager.SceneManager.createSceneNode(name + "__MeasurementManualObjectSceneNode");
                sceneNode.attachObject(manualObject);
                sceneNode.setVisible(visible);
                sceneNode.setPosition(origin);
                sceneManager.SceneManager.getRootSceneNode().addChild(sceneNode);
                drawGrid(5.0f, 25.0f);
                textWatermark.Visible = visible;
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
                if (sceneNode != null)
                {
                    sceneNode.setVisible(value);
                    textWatermark.Visible = value;
                }
            }
        }

        /// <summary>
        /// Draw a grid with the given spacing in milimeters taking up the space specified by gridmax.
        /// </summary>
        /// <param name="gridSpacingMM">The number of milimeters to space the grid lines.</param>
        /// <param name="gridMax">The area of the grid in 3d units.</param>
        public void drawGrid(float gridSpacingMM, float gridMax)
        {
            textWatermark.Text = String.Format("Grid Spacing: {0} mm", gridSpacingMM);
            float gridSpacing = gridSpacingMM * (1.0f / Measurement.unitsToMM);
            Vector3 startPoint = new Vector3(0.0f, -gridMax, 0.0f);
            Vector3 endPoint = new Vector3(0.0f, gridMax, 0.0f);
            manualObject.clear();
            manualObject.begin("Grid", OperationType.OT_LINE_LIST);
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

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public Vector3 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
                if (sceneNode != null)
                {
                    sceneNode.setPosition(origin);
                }
            }
        }

        public bool HideCaption
        {
            get
            {
                return !textWatermark.Visible;
            }
            set
            {
                textWatermark.Visible = !value;
            }
        }

        void sceneViewController_WindowDestroyed(SceneViewWindow window)
        {
            window.FindVisibleObjects -= window_FindVisibleObjects;
        }

        void sceneViewController_WindowCreated(SceneViewWindow window)
        {
            window.FindVisibleObjects += window_FindVisibleObjects;
        }

        void window_FindVisibleObjects(SceneViewWindow window, bool currentCameraRender)
        {
            if (currentCameraRender)
            {
                sceneNode.setOrientation(window.Orientation);
            }
        }

        /// <summary>
        /// Call this function before a screenshot is rendered to hide the
        /// grid lines if you wish them hidden in the screenshot. This
        /// function is setup to consume as an EventHandler, but it does not
        /// actually use the arguments.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        public void ScreenshotRenderStarted(Object sender, EventArgs e)
        {
            if (visible)
            {
                if (sceneNode != null)
                {
                    sceneNode.setVisible(false);
                    textWatermark.Visible = false;
                }
            }
        }

        /// <summary>
        /// Call this function after a screenshot is rendered to show the
        /// grid lines if you hid them with ScreenshotRenderStarted. This
        /// function is setup to consume as an EventHandler, but it does not
        /// actually use the arguments.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        public void ScreenshotRenderCompleted(Object sender, EventArgs e)
        {
            if (visible)
            {
                if (sceneNode != null)
                {
                    sceneNode.setVisible(true);
                    textWatermark.Visible = true;
                }
            }
        }
    }
}
