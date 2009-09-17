using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Engine.Renderer;
using Engine.ObjectManagement;
using Logging;
using Engine;
using OgrePlugin;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OgreWrapper;
using System.IO;

namespace Medical
{
    public partial class DrawingWindow : UserControl, OSWindow, CameraMotionValidator
    {
        public event DrawingWindowEvent CameraCreated;
        public event DrawingWindowEvent CameraDestroyed;
        public event OgreCameraCallback PreFindVisibleObjects;

        private List<OSWindowListener> listeners = new List<OSWindowListener>();
        private RendererWindow window;
        private String name;
        private CameraControl camera;
        private OrbitCameraController orbitCamera;
        private RendererPlugin renderer;
        private bool showSceneStats = false;
        private UpdateTimer mainTimer;
        private RenderingMode renderingMode = RenderingMode.Solid;
        private SimScene scene;
        private Watermark watermark = null;
        private EventManager eventManager;

        public DrawingWindow()
        {
            InitializeComponent();
        }

        internal void initialize(string name, EventManager eventManager, RendererPlugin renderer, Vector3 translation, Vector3 lookAt)
        {
            this.name = name;
            this.renderer = renderer;
            this.eventManager = eventManager;
            orbitCamera = new OrbitCameraController(translation, lookAt, eventManager);
            orbitCamera.MotionValidator = this;
            window = renderer.createRendererWindow(this, name);
            watermark = new TiledWatermark(name + "Watermark", "Watermark", 150, 60);
            //watermark = new TextWatermark(name + "Watermark", "Piper Clinic Copyright 2009", 32);
            //watermark = new SideLogoWatermark(name + "Watermark", "PiperClinic", 150, 60);
            //watermark = new CenteredWatermark(name + "Watermark", "PiperClinicAlpha", 1.0f, 0.4f);
        }

        public void recreateWindow()
        {
            destroyCamera();
            renderer.destroyRendererWindow(window);
            window = renderer.createRendererWindow(this, name);
            createCamera(mainTimer, scene);
        }

        public void createWatermark()
        {
            watermark.createOverlays();
        }

        public void destroyWatermark()
        {
            if (watermark != null)
            {
                watermark.destroyOverlays();
            }
        }

        private void disposeCallback()
        {
            destroyWatermark();
        }

        public void createCamera(UpdateTimer mainTimer, SimScene scene)
        {
            this.scene = scene;
            SimSubScene defaultScene = scene.getDefaultSubScene();
            if (defaultScene != null)
            {
                this.mainTimer = mainTimer;
                camera = window.createCamera(defaultScene, name, orbitCamera.Translation, orbitCamera.LookAt);
                camera.BackgroundColor = Engine.Color.FromARGB(BackColor.ToArgb());
                camera.addLight();
                camera.setNearClipDistance(1.0f);
                camera.setFarClipDistance(1000.0f);
                camera.setRenderingMode(renderingMode);
                mainTimer.addFixedUpdateListener(orbitCamera);
                orbitCamera.setCamera(camera);
                CameraResolver.addMotionValidator(this);
                camera.showSceneStats(showSceneStats);
                ((OgreCameraControl)camera).PreFindVisibleObjects += camera_PreFindVisibleObjects;
                if (CameraCreated != null)
                {
                    CameraCreated.Invoke(this);
                }
            }
            else
            {
                Log.Default.sendMessage("Cannot find default subscene for the scene. Not creating camera.", LogLevel.Error, "Anomaly");
            }
        }

        public void destroyCamera()
        {
            if (camera != null)
            {
                if (CameraDestroyed != null)
                {
                    CameraDestroyed.Invoke(this);
                }
                ((OgreCameraControl)camera).PreFindVisibleObjects -= camera_PreFindVisibleObjects;
                orbitCamera.setCamera(null);
                window.destroyCamera(camera);
                mainTimer.removeFixedUpdateListener(orbitCamera);
                camera = null;
                CameraResolver.removeMotionValidator(this);
            }
        }

        void camera_PreFindVisibleObjects(bool callingCameraRender)
        {
            watermark.setVisible(callingCameraRender);
            if (PreFindVisibleObjects != null)
            {
                PreFindVisibleObjects.Invoke(callingCameraRender);
            }
        }

        public void setCamera(Vector3 position, Vector3 lookAt)
        {
            orbitCamera.setNewPosition(position, lookAt);
        }

        public void setEnabled(bool enabled)
        {
            if (window != null)
            {
                window.setEnabled(enabled);
            }
        }

        public void showStats(bool show)
        {
            if (camera != null)
            {
                camera.showSceneStats(show);
            }
            showSceneStats = show;
        }

        public void setRenderingMode(RenderingMode mode)
        {
            this.renderingMode = mode;
            if (camera != null)
            {
                camera.setRenderingMode(renderingMode);
            }
        }

        public void setNewPosition(Vector3 translation, Vector3 lookAt)
        {
            orbitCamera.setNewPosition(translation, lookAt);
        }

        public void updateRender(bool swapBuffers)
        {
            camera.update(swapBuffers);
        }

        public Vector3 getScreenPosition(Vector3 worldPosition)
        {
            Matrix4x4 proj = camera.ProjectionMatrix;
            Matrix4x4 view = camera.ViewMatrix;
            Vector3 screenPos = proj * (view * worldPosition);
            screenPos.x = screenPos.x / 2.0f + 0.5f;
            screenPos.y = 1 - (screenPos.y / 2.0f + 0.5f);
            return screenPos;
        }

        public String CameraName
        {
            get
            {
                return name;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return orbitCamera.Translation;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return orbitCamera.LookAt;
            }
        }

        public Camera Camera
        {
            get
            {
                return ((OgreCameraControl)camera).Camera;
            }
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                return camera.ViewMatrix;
            }
        }

        public Matrix4x4 ProjectionMatrix
        {
            get
            {
                return camera.ProjectionMatrix;
            }
        }

        public float OrbitDistance
        {
            get
            {
                return orbitCamera.OrbitDistance;
            }
        }

        public bool AllowRotation
        {
            get
            {
                return orbitCamera.AllowRotation;
            }
            set
            {
                orbitCamera.AllowRotation = value;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                if (this.Size.Width > 0 && this.Size.Height > 0)
                {
                    listener.resized(this);
                    watermark.sizeChanged(this.Size.Width, this.Size.Height);
                }
            }
            base.OnResize(e);
        }

        protected override void OnMove(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.moved(this);
            }
            base.OnMove(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (camera != null)
            {
                camera.BackgroundColor = Engine.Color.FromARGB(BackColor.ToArgb());
            }
            base.OnBackColorChanged(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            foreach (OSWindowListener listener in listeners)
            {
                listener.closing(this);
            }
            if (window != null)
            {
                renderer.destroyRendererWindow(window);
            }
            base.OnHandleDestroyed(e);
        }

        #region OSWindow Members

        public void addListener(OSWindowListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(OSWindowListener listener)
        {
            listeners.Remove(listener);
        }

        public IntPtr WindowHandle
        {
            get
            {
                return this.Handle;
            }
        }

        public int WindowWidth
        {
            get
            {
                return this.Width;
            }
        }

        public int WindowHeight
        {
            get
            {
                return this.Height;
            }
        }

        #endregion

        #region CameraMotionValidator Members

        /// <summary>
        /// Determine if the camera should be allowed to move based on the current mouse location.
        /// </summary>
        /// <param name="x">The x location of the mouse.</param>
        /// <param name="y">The y location of the mouse.</param>
        /// <returns>True if the camera should be allowed to move.  False if it should stay still.</returns>
        public bool allowMotion(int x, int y)
        {
            Control topLevel = this.TopLevelControl;
            if (topLevel != null)
            {
                return ClientRectangle.Contains(this.PointToClient(topLevel.PointToScreen(new Point(x, y))));
            }
            return false;
        }

        /// <summary>
        /// Determine if the window is currently set as "active" allowing certain behavior.
        /// This is an optional check by classes using the validator it may be desirable to
        /// do an action even if the window is not active.
        /// </summary>
        /// <returns>True if the window is active.</returns>
        public bool isActiveWindow()
        {
            return this.Focused;
        }

        /// <summary>
        /// Get the location passed in the coordinates for the motion validator.
        /// </summary>
        /// <param name="x">X location.</param>
        /// <param name="y">Y location.</param>
        public void getLocalCoords(ref float x, ref float y)
        {
            doGetLocalCoords(ref x, ref y, this);
        }

        /// <summary>
        /// Helper function to find the local coords.  We need to ignore the top level frame,
        /// so this will recurse until the control has no parent.
        /// </summary>
        /// <param name="x">The x location.</param>
        /// <param name="y">The y location.</param>
        /// <param name="ctrl">The current control to scan.</param>
        private void doGetLocalCoords(ref float x, ref float y, Control ctrl)
        {
            if (ctrl.Parent != null)
            {
                Point p = ctrl.Location;
                x -= p.X;
                y -= p.Y;
                doGetLocalCoords(ref x, ref y, ctrl.Parent);
            }
        }

        /// <summary>
        /// Get the width of the mouse area for this validator.
        /// </summary>
        /// <returns>The width of the mouse area.</returns>
        public float getMouseAreaWidth()
        {
            return Width;
        }

        /// <summary>
        /// Get the height of the mouse area for this validator.
        /// </summary>
        /// <returns>The height of the mouse area.</returns>
        public float getMouseAreaHeight()
        {
            return Height;
        }

        /// <summary>
        /// Get the camera for this motion validator.
        /// </summary>
        /// <returns>The camera for this validator.</returns>
        public CameraControl getCamera()
        {
            return camera;
        }

        #endregion
    }
}
