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
        public event SceneViewWindowRenderEvent FindVisibleObjects;
        public event SceneViewWindowRenderEvent RenderingStarted;
        public event SceneViewWindowRenderEvent RenderingEnded;
        public event SceneViewWindowResizedEvent Resized;

        private SceneView sceneView;
        private CameraMover cameraMover;
        private UpdateTimer mainTimer;
        private RendererWindow window;
        private String name;
        protected SceneViewController controller;

        private Vector3 startPosition;
        private Vector3 startLookAt;

        private Vector2 location = new Vector2(0.0f, 0.0f);
        private Size2 size = new Size2(1.0f, 1.0f);

        private Color backColor = new Color(0.149f, 0.149f, 0.149f);

        protected String transparencyStateName;

        public SceneViewWindow(SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
        {
            this.controller = controller;
            this.cameraMover = cameraMover;
            cameraMover.MotionValidator = this;
            this.name = name;
            this.mainTimer = mainTimer;
            this.startPosition = cameraMover.Translation;
            this.startLookAt = cameraMover.LookAt;
            mainTimer.addFixedUpdateListener(cameraMover);
            AllowNavigation = true;
            transparencyStateName = name;
            TransparencyController.createTransparencyState(transparencyStateName);
            UseDefaultTransparency = false;
        }

        public virtual void Dispose()
        {
            mainTimer.removeFixedUpdateListener(cameraMover);
            TransparencyController.removeTransparencyState(transparencyStateName);
        }

        public virtual void createSceneView(RendererWindow window, SimScene scene)
        {
            Log.Info("Creating SceneView for {0}.", name);
            this.window = window;
            SimSubScene defaultScene = scene.getDefaultSubScene();

            sceneView = window.createSceneView(defaultScene, name, cameraMover.Translation, cameraMover.LookAt);
            sceneView.setDimensions(location.x, location.y, size.Width, size.Height);
            sceneView.BackgroundColor = backColor;
            sceneView.addLight();
            sceneView.setNearClipDistance(1.0f);
            sceneView.setFarClipDistance(1000.0f);
            sceneView.moveSceneStats(new Vector2(0.0f, 0.1f));
            //camera.setRenderingMode(renderingMode);
            cameraMover.setCamera(sceneView);
            CameraResolver.addMotionValidator(this);
            sceneView.showSceneStats(MedicalConfig.EngineConfig.ShowStatistics);
            sceneView.FindVisibleObjects += sceneView_FindVisibleObjects;
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
                Log.Info("Destroying SceneView for {0}.", name);
                CameraResolver.removeMotionValidator(this);
                sceneView.FindVisibleObjects -= sceneView_FindVisibleObjects;
                sceneView.RenderingStarted -= sceneView_RenderingStarted;
                sceneView.RenderingEnded -= sceneView_RenderingEnded;
                cameraMover.setCamera(null);
                window.destroySceneView(sceneView);
                sceneView = null;
                if (CameraDestroyed != null)
                {
                    CameraDestroyed.Invoke(this);
                }
            }
        }

        public abstract void close();

        public void setPosition(CameraPosition position)
        {
            cameraMover.setNewPosition(position.Translation, position.LookAt);
        }

        public void setPosition(Vector3 translation, Vector3 lookAt)
        {
            cameraMover.setNewPosition(translation, lookAt);
        }

        public void setPosition(Vector3 translation, Vector3 lookAt, float duration)
        {
            cameraMover.setNewPosition(translation, lookAt, duration);
        }

        public void immediatlySetPosition(Vector3 translation, Vector3 lookAt)
        {
            cameraMover.immediatlySetPosition(translation, lookAt);
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            Size2 totalSize = TopmostWorkingSize;
            if (totalSize.Width == 0.0f)
            {
                totalSize.Width = 1.0f;
            }
            if (totalSize.Height == 0.0f)
            {
                totalSize.Height = 1.0f;
            }
            location = new Vector2(Location.x / totalSize.Width, Location.y / totalSize.Height);
            size = new Size2(WorkingSize.Width / totalSize.Width, WorkingSize.Height / totalSize.Height);
            if (sceneView != null)
            {
                sceneView.setDimensions(location.x, location.y, size.Width, size.Height);
            }
            if (Resized != null)
            {
                Resized.Invoke(this);
            }
        }

        public override Size2 DesiredSize
        {
            get
            {
                if (sceneView != null)
                {
                    return new Size2(sceneView.RenderWidth, sceneView.RenderHeight);
                }
                return new Size2();
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

        public void moveCameraToIncludePoint(Vector3 includePoint, float transitionTime)
        {
            float distance = computeOffsetToIncludePoint(includePoint);
            Vector3 direction = (Translation - LookAt).normalized();
            setPosition(Translation - (direction * distance), LookAt, transitionTime);
        }

        /// <summary>
        /// Compute an offset from the current camera position to one that includes the given point
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public float computeOffsetToIncludePoint(Vector3 include)
        {
            //Transform the point from world space to camera space
            Matrix4x4 viewMatrix = Camera.getViewMatrix();
            Vector3 localInclude = viewMatrix * include;
            float aspect = Camera.getAspectRatio();
            float fovy = Camera.getFOVy() * 0.5f;
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
            if (xOffset < yOffset)
            {
                return xOffset;
            }
            return yOffset;
        }

        public Ray3 getCameraToViewportRay(float x, float y)
        {
            if (sceneView != null)
            {
                return sceneView.getCameraToViewportRay(x, y);
            }
            return new Ray3();
        }

        public void showSceneStats(bool show)
        {
            if (sceneView != null)
            {
                sceneView.showSceneStats(show);
            }
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
                if (sceneView != null)
                {
                    sceneView.BackgroundColor = value;
                }
            }
        }

        public bool AllowNavigation { get; set; }

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

        public abstract bool Focused
        {
            get;
            set;
        }

        public CameraMover CameraMover
        {
            get
            {
                return cameraMover;
            }
        }

        void sceneView_FindVisibleObjects(SceneView sceneView)
        {
            if (FindVisibleObjects != null)
            {
                FindVisibleObjects.Invoke(this, sceneView.CurrentlyRendering);
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
