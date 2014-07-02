using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine.Renderer;
using Logging;
using OgreWrapper;
using OgrePlugin;
using MyGUIPlugin;

namespace Medical.Controller
{
    public delegate void SceneViewWindowRenderEvent(SceneViewWindow window, bool currentCameraRender);
    public delegate void SceneViewWindowResizedEvent(SceneViewWindow window);

    public abstract class SceneViewWindow : LayoutContainer, IDisposable, CameraMotionValidator
    {
        public event SceneViewWindowEvent CameraCreated;
        public event SceneViewWindowEvent CameraDestroyed;
        public event SceneViewWindowRenderEvent RenderingStarted;
        public event SceneViewWindowRenderEvent RenderingEnded;
        public event SceneViewWindowResizedEvent Resized;
        public event SceneViewWindowEvent Disposed;

        private SceneView sceneView;
        private CameraMover cameraMover;
        private UpdateTimer mainTimer;
        private RendererWindow window;
        private String name;
        protected SceneViewController controller;

        private Vector3 startPosition;
        private Vector3 startLookAt;

        private Vector2 sceneViewportLocation = new Vector2(0f, 0f);
        private Size2 sceneViewportSize = new Size2(1f, 1f);
        private float inverseAspectRatio = 1.0f;

        private Color backColor = new Color(0.149f, 0.149f, 0.149f);

        protected String transparencyStateName;

        private bool autoAspectRatio = true;
        private Widget borderPanel0;
        private Widget borderPanel1;
        protected BackgroundScene background;
        private ViewportBackground vpBackground;
        private int zIndexStart;
        private int zOffset = 0;
        private RenderingMode renderingMode = RenderingMode.Solid;
        private bool clearEveryFrame = false;

        public SceneViewWindow(SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name, BackgroundScene background, int zIndexStart)
        {
            this.zIndexStart = zIndexStart;
            this.background = background;
            this.controller = controller;
            this.cameraMover = cameraMover;
            cameraMover.MotionValidator = this;
            this.name = name;
            this.mainTimer = mainTimer;
            this.startPosition = cameraMover.Translation;
            this.startLookAt = cameraMover.LookAt;
            transparencyStateName = name;
            TransparencyController.createTransparencyState(transparencyStateName);
            UseDefaultTransparency = false;
            NearPlaneWorldPos = 200;
            FarPlaneWorldPos = -200;
        }

        /// <summary>
        /// You must call this function to activate the background with the appropriate render target.
        /// </summary>
        /// <param name="renderTarget"></param>
        protected void createBackground(RenderTarget renderTarget, bool clearEveryFrame)
        {
            vpBackground = new ViewportBackground(name + "SceneViewBackground", zIndexStart + zOffset++, background, renderTarget, clearEveryFrame);
            vpBackground.BackgroundColor = backColor;
        }

        /// <summary>
        /// You must call this function to listen for updates on the camera mover. A lot of things will not work correctly
        /// if you do not call this function, however, since we also use this mechanism for the render to texture scene
        /// views and since they do not really require updates normally and since there are potentially a lot more of
        /// those types of scene views compared to the ones users actually interact with we want to be able to have scene
        /// views that do not automatically update.
        /// </summary>
        protected void listenForCameraMoverUpdates()
        {
            mainTimer.addUpdateListener(cameraMover);
        }

        public virtual void Dispose()
        {
            IDisposableUtil.DisposeIfNotNull(vpBackground);
            mainTimer.removeUpdateListener(cameraMover);
            TransparencyController.removeTransparencyState(transparencyStateName);
            destroyBorderPanels();
            if (Disposed != null)
            {
                Disposed.Invoke(this);
            }
        }

        public virtual void createSceneView(SimScene scene)
        {
            Log.Info("Creating SceneView for {0}.", name);
            SimSubScene defaultScene = scene.getDefaultSubScene();

            sceneView = window.createSceneView(defaultScene, name, cameraMover.Translation, cameraMover.LookAt, zIndexStart + zOffset++);
            sceneView.setDimensions(sceneViewportLocation.x, sceneViewportLocation.y, sceneViewportSize.Width, sceneViewportSize.Height);
            if (vpBackground != null)
            {
                vpBackground.setDimensions(sceneViewportLocation.x, sceneViewportLocation.y, sceneViewportSize.Width, sceneViewportSize.Height);
            }
            sceneView.BackgroundColor = backColor;
            sceneView.setNearClipDistance(1.0f);
            sceneView.setFarClipDistance(1000.0f);
            sceneView.ClearEveryFrame = clearEveryFrame;
            sceneView.setRenderingMode(renderingMode);
            cameraMover.setCamera(new CameraPositioner(sceneView, NearPlaneWorldPos, FarPlaneWorldPos));
            CameraResolver.addMotionValidator(this);
            sceneView.RenderingStarted += sceneView_RenderingStarted;
            sceneView.RenderingEnded += sceneView_RenderingEnded;
            if (CameraCreated != null)
            {
                CameraCreated.Invoke(this);
            }
        }

        public virtual void destroySceneView()
        {
            if (sceneView != null)
            {
                if (CameraDestroyed != null)
                {
                    CameraDestroyed.Invoke(this);
                }
                --zOffset;
                Log.Info("Destroying SceneView for {0}.", name);
                CameraResolver.removeMotionValidator(this);
                sceneView.RenderingStarted -= sceneView_RenderingStarted;
                sceneView.RenderingEnded -= sceneView_RenderingEnded;
                cameraMover.setCamera(null);
                window.destroySceneView(sceneView);
                sceneView = null;
            }
        }

        public abstract void close();

        public void setPosition(CameraPosition cameraPosition, float duration)
        {
            cameraMover.setNewPosition(this.computeAdjustedTranslation(cameraPosition), cameraPosition.LookAt, duration, cameraPosition.Easing);
            if (cameraPosition.UseIncludePoint)
            {
                cameraMover.maintainIncludePoint(cameraPosition.IncludePoint);
            }
            else
            {
                cameraMover.stopMaintainingIncludePoint();
            }
        }

        public void immediatlySetPosition(CameraPosition cameraPosition)
        {
            cameraMover.immediatlySetPosition(cameraPosition.Translation, cameraPosition.LookAt);
            if (cameraPosition.UseIncludePoint)
            {
                cameraMover.maintainIncludePoint(cameraPosition.IncludePoint);
            }
            else
            {
                cameraMover.stopMaintainingIncludePoint();
            }
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            IntVector2 sceneViewLocation = new IntVector2(0, 0);
            IntSize2 size = new IntSize2(1, 1);

            IntSize2 totalSize = TopmostWorkingSize;
            if (totalSize.Width == 0)
            {
                totalSize.Width = 1;
            }
            if (totalSize.Height == 0)
            {
                totalSize.Height = 1;
            }

            sceneViewLocation = Location;
            size = WorkingSize;

            if (!AutoAspectRatio)
            {
                size.Height = (int)(size.Width * inverseAspectRatio);
                if (size.Height > WorkingSize.Height) //Letterbox width
                {
                    size.Height = WorkingSize.Height;
                    size.Width = (int)(size.Height * (1 / inverseAspectRatio));
                    sceneViewLocation.x += (WorkingSize.Width - size.Width) / 2;

                    borderPanel0.setCoord(Location.x, Location.y, sceneViewLocation.x - Location.x, WorkingSize.Height);
                    borderPanel1.setCoord(sceneViewLocation.x + size.Width, Location.y, sceneViewLocation.x - Location.x + 1, WorkingSize.Height);
                }
                else
                {
                    sceneViewLocation.y += (WorkingSize.Height - size.Height) / 2;

                    borderPanel0.setCoord(Location.x, Location.y, WorkingSize.Width, sceneViewLocation.y - Location.y);
                    borderPanel1.setCoord(Location.x, sceneViewLocation.y + size.Height, WorkingSize.Width, sceneViewLocation.y - Location.y + 1);
                }
            }

            RenderXLoc = sceneViewLocation.x;
            RenderYLoc = sceneViewLocation.y;

            sceneViewportLocation = new Vector2((float)sceneViewLocation.x / totalSize.Width, (float)sceneViewLocation.y / totalSize.Height);
            sceneViewportSize = new Size2((float)size.Width / totalSize.Width, (float)size.Height / totalSize.Height);

            if (sceneView != null)
            {
                sceneView.setDimensions(sceneViewportLocation.x, sceneViewportLocation.y, sceneViewportSize.Width, sceneViewportSize.Height);
                if (vpBackground != null)
                {
                    vpBackground.setDimensions(sceneViewportLocation.x, sceneViewportLocation.y, sceneViewportSize.Width, sceneViewportSize.Height);
                }
                cameraMover.processIncludePoint(Camera);
            }

            if (Resized != null)
            {
                Resized.Invoke(this);
            }
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                if (sceneView != null)
                {
                    return new IntSize2(sceneView.RenderWidth, sceneView.RenderHeight);
                }
                return new IntSize2();
            }
        }

        public void resetToStartPosition()
        {
            cameraMover.immediatlySetPosition(startPosition, startLookAt);
        }

        public override void bringToFront()
        {
            
        }

        public Vector3 getScreenPosition(Vector3 worldPosition)
        {
            Matrix4x4 proj = sceneView.ProjectionMatrix;
            Matrix4x4 view = sceneView.ViewMatrix;
            Vector3 screenPos = proj * (view * worldPosition);
            screenPos.x = (screenPos.x / 2.0f + 0.5f) * sceneView.RenderWidth;
            screenPos.y = (1 - (screenPos.y / 2.0f + 0.5f)) * sceneView.RenderHeight;
            return screenPos;
        }

        /// <summary>
        /// Setup the include point for the given camera position for this window. Modifies cameraPosition to have the calculated value
        /// and sets UseIncludePoint to true.
        /// </summary>
        /// <param name="cameraPosition"></param>
        public void calculateIncludePoint(CameraPosition cameraPosition)
        {
            //Make the include point projected out to the lookat location
            Ray3 camRay = this.getCameraToViewportRay(1, 0);
            cameraPosition.IncludePoint = camRay.Origin + camRay.Direction * (cameraPosition.LookAt - cameraPosition.Translation).length();
            cameraPosition.UseIncludePoint = true;
        }

        /// <summary>
        /// Compute a new translation for the given camera position and returns it.
        /// </summary>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public Vector3 computeAdjustedTranslation(CameraPosition cameraPosition)
        {
            if (cameraPosition.UseIncludePoint && cameraPosition.IncludePoint.isNumber())
            {
                return SceneViewWindow.computeIncludePointAdjustedPosition(Camera.getAspectRatio(), Camera.getFOVy(), Camera.getProjectionMatrix(), cameraPosition.Translation, cameraPosition.LookAt, cameraPosition.IncludePoint);
            }

            return cameraPosition.Translation;
        }

        public static Vector3 computeIncludePointAdjustedPosition(float aspect, float fovy, Matrix4x4 projectionMatrix, Vector3 translation, Vector3 lookAt, Vector3 includePoint)
        {
            fovy *= 0.5f;
            Vector3 direction = lookAt - translation;

            //Figure out direction, must use ogre fixed yaw calculation, first adjust direction to face -z
            Vector3 zAdjustVec = -direction;
            zAdjustVec.normalize();
            Quaternion targetWorldOrientation = Quaternion.shortestArcQuatFixedYaw(ref zAdjustVec);

            Matrix4x4 viewMatrix = Matrix4x4.makeViewMatrix(translation, targetWorldOrientation);
            float offset = SceneViewWindow.computeOffsetToIncludePoint(viewMatrix, projectionMatrix, includePoint, aspect, fovy);

            direction.normalize();
            Vector3 newTrans = translation + offset * direction;
            return newTrans;
        }

        public static float computeOffsetToIncludePoint(Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Vector3 include, float aspect, float fovy)
        {
            //Transform the point from world space to camera space
            Vector3 localInclude = viewMatrix * include;
            float fovx = (float)Math.Atan(aspect * Math.Tan(fovy));

            //Project the points onto the two triangles formed between the camera, the z coord and the x or y coord we know the 
            //length of the opposite leg (x or y) and can compute the adjacent leg by multiplying by the Tan(field of view). This gives
            //the distance from the include point the camera needs to be to include it in view on that dimension (x or y). 
            float yOffset = Math.Abs(localInclude.y) / (float)Math.Tan(fovy);
            float xOffset = Math.Abs(localInclude.x) / (float)Math.Tan(fovx);

            //The camera is currently at localInclude.z so figure out how far it would need to move to be at the final point. This is 
            //the difference between the z coord and the offset.
            float zValue = Math.Abs(localInclude.z);
            xOffset = zValue - xOffset;
            yOffset = zValue - yOffset;

            Vector3 projectionSpace = (projectionMatrix * viewMatrix) * include;

            //Find the dimension that is more off the screen.
            if (projectionSpace.x > projectionSpace.y)
            {
                return xOffset;
            }
            return yOffset;
        }

        public static Vector3 Unproject(float screenX, float screenY, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix)
        {
            Matrix4x4 inverseVP = (projectionMatrix * viewMatrix).inverse();

            float nx = (2.0f * screenX) - 1.0f;
            float ny = 1.0f - (2.0f * screenY);
            Vector3 midPoint = new Vector3(nx, ny, 0.0f);

            return inverseVP * midPoint;
        }

        public static Vector2 Project(Vector3 point, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, int screenWidth, int screenHeight)
        {
            Vector3 pos2d = projectionMatrix * (viewMatrix * point);
            return new Vector2(((pos2d.x * 0.5f) + 0.5f) * screenWidth, (1.0f - ((pos2d.y * 0.5f) + 0.5f)) * screenHeight);
        }

        public Ray3 getCameraToViewportRay(float x, float y)
        {
            if (sceneView != null)
            {
                return sceneView.getCameraToViewportRay(x, y);
            }
            return new Ray3();
        }

        public Vector3 unproject(float screenX, float screenY)
        {
            return Unproject(screenX, screenY, Camera.getViewMatrix(), Camera.getProjectionMatrix());
        }

        public void rotate(float yawDelta, float pitchDelta)
        {
            cameraMover.rotate(yawDelta, pitchDelta);
        }

        public void pan(float xDelta, float yDelta)
        {
            cameraMover.pan(xDelta, yDelta);
        }

        public void zoom(float zoomDelta)
        {
            cameraMover.zoom(zoomDelta);
        }

        /// <summary>
        /// Stop the window from auto adjusting to show the include point when the
        /// scene view is resized. Should call whenever the user updates a position
        /// manually through a control mechanism (rotating camera with mouse, touching screen etc)
        /// </summary>
        public void stopMaintainingIncludePoint()
        {
            //Do not add any custom logic here, only pass to camera mover.
            cameraMover.stopMaintainingIncludePoint();
        }

        public override bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public Vector3 Translation
        {
            get
            {
                return cameraMover.Translation;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return cameraMover.LookAt;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return sceneView.Direction;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return sceneView.Orientation;
            }
        }

        public float YawVelocity
        {
            get
            {
                return cameraMover.YawVelocity;
            }
            set
            {
                cameraMover.YawVelocity = value;
            }
        }

        public float PitchVelocity
        {
            get
            {
                return cameraMover.PitchVelocity;
            }
            set
            {
                cameraMover.PitchVelocity = value;
            }
        }

        public Vector2 PanVelocity
        {
            get
            {
                return cameraMover.PanVelocity;
            }
            set
            {
                cameraMover.PanVelocity = value;
            }
        }

        public float ZoomVelocity
        {
            get
            {
                return cameraMover.ZoomVelocity;
            }
            set
            {
                cameraMover.ZoomVelocity = value;
            }
        }

        public String CurrentTransparencyState
        {
            get
            {
                return UseDefaultTransparency ? TransparencyController.DefaultTransparencyState : transparencyStateName;
            }
        }

        public bool UseDefaultTransparency { get; set; }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public Camera Camera
        {
            get
            {
                //Have to typecast the scene view for this.
                return ((OgreSceneView)sceneView).Camera;
            }
        }

        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                if (vpBackground != null)
                {
                    vpBackground.BackgroundColor = value;
                }
                if (sceneView != null)
                {
                    sceneView.BackgroundColor = value;
                }
            }
        }

        public bool ClearEveryFrame
        {
            get
            {
                return clearEveryFrame;
            }
            set
            {
                if (clearEveryFrame != value)
                {
                    clearEveryFrame = value;
                    if (sceneView != null)
                    {
                        sceneView.ClearEveryFrame = value;
                    }
                }
            }
        }

        public int RenderXLoc { get; private set; }

        public int RenderYLoc { get; private set; }

        public int RenderWidth
        {
            get
            {
                if (sceneView != null)
                {
                    return sceneView.RenderWidth;
                }
                else
                {
                    return 1;
                }
            }
        }

        public int RenderHeight
        {
            get
            {
                if (sceneView != null)
                {
                    return sceneView.RenderHeight;
                }
                else
                {
                    return 1;
                }
            }
        }

        public CameraMover CameraMover
        {
            get
            {
                return cameraMover;
            }
        }

        /// <summary>
        /// True to autocalculate the aspect ratio. If this is false the window will be letterboxed.
        /// </summary>
        public bool AutoAspectRatio
        {
            get
            {
                return autoAspectRatio;
            }
            set
            {
                if (autoAspectRatio != value)
                {
                    autoAspectRatio = value;
                    if (autoAspectRatio)
                    {
                        destroyBorderPanels();
                    }
                    else
                    {
                        createBorderPanels();
                    }
                }
            }
        }

        /// <summary>
        /// The aspect ratio as width / height
        /// </summary>
        public float AspectRatio
        {
            get
            {
                if (inverseAspectRatio != 0.0f)
                {
                    return 1.0f / inverseAspectRatio;
                }
                else
                {
                    return 1.0f;
                }
            }
            set
            {
                if (value != 0.0f)
                {
                    inverseAspectRatio = 1.0f / value;
                }
                else
                {
                    inverseAspectRatio = 1.0f;
                }
            }
        }

        public float NearPlaneWorldPos { get; set; }

        public float FarPlaneWorldPos { get; set; }

        public RenderingMode RenderingMode
        {
            get
            {
                return renderingMode;
            }
            set
            {
                if (renderingMode != value)
                {
                    renderingMode = value;
                    if (sceneView != null)
                    {
                        sceneView.setRenderingMode(renderingMode);
                    }
                }
            }
        }

        protected RendererWindow RendererWindow
        {
            get
            {
                return window;
            }
            set
            {
                window = value;
            }
        }

        void sceneView_RenderingEnded(SceneView sceneView)
        {
            if (RenderingEnded != null)
            {
                RenderingEnded.Invoke(this, sceneView.CurrentlyRendering);
            }
        }

        void sceneView_RenderingStarted(SceneView sceneView)
        {
            TransparencyController.applyTransparencyState(CurrentTransparencyState);
            if (RenderingStarted != null)
            {
                RenderingStarted.Invoke(this, sceneView.CurrentlyRendering);
            }
        }

        void createBorderPanels()
        {
            if (borderPanel0 == null)
            {
                borderPanel0 = Gui.Instance.createWidgetT("Widget", "SceneViewBorder", 0, 0, 1, 1, Align.Default, "Back", "");
                borderPanel1 = Gui.Instance.createWidgetT("Widget", "SceneViewBorder", 0, 0, 1, 1, Align.Default, "Back", "");
            }
        }

        void destroyBorderPanels()
        {
            if (borderPanel0 != null)
            {
                Gui.Instance.destroyWidget(borderPanel0);
                Gui.Instance.destroyWidget(borderPanel1);
                borderPanel0 = null;
                borderPanel1 = null;
            }
        }

        #region CameraMotionValidator Members

        /// <summary>
        /// Determine if the camera should be allowed to move based on the current mouse location.
        /// </summary>
        /// <param name="x">The x location of the mouse.</param>
        /// <param name="y">The y location of the mouse.</param>
        /// <returns>True if the camera should be allowed to move.  False if it should stay still.</returns>
        public bool allowMotion(int x, int y)
        {
            return !InputManager.Instance.isModalAny() && (x > Location.x && x < Location.x + WorkingSize.Width) && (y > Location.y && y < Location.y + WorkingSize.Height);
        }

        /// <summary>
        /// Determine if the window is currently set as "active" allowing certain behavior.
        /// This is an optional check by classes using the validator it may be desirable to
        /// do an action even if the window is not active.
        /// </summary>
        /// <returns>True if the window is active.</returns>
        public bool isActiveWindow()
        {
            return true;
        }

        /// <summary>
        /// Get the location passed in the coordinates for the motion validator.
        /// </summary>
        /// <param name="x">X location.</param>
        /// <param name="y">Y location.</param>
        public void getLocalCoords(ref float x, ref float y)
        {
            x -= Location.x;
            y -= Location.y;
        }

        /// <summary>
        /// Get the width of the mouse area for this validator.
        /// </summary>
        /// <returns>The width of the mouse area.</returns>
        public float getMouseAreaWidth()
        {
            return WorkingSize.Width;
        }

        /// <summary>
        /// Get the height of the mouse area for this validator.
        /// </summary>
        /// <returns>The height of the mouse area.</returns>
        public float getMouseAreaHeight()
        {
            return WorkingSize.Height;
        }

        /// <summary>
        /// Get the camera for this motion validator.
        /// </summary>
        /// <returns>The camera for this validator.</returns>
        public SceneView getCamera()
        {
            return sceneView;
        }

        #endregion
    }
}
