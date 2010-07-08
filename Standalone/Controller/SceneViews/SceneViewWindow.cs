using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine.Renderer;
using Logging;

namespace Medical.Controller
{
    public delegate void SceneViewWindowRenderEvent(SceneViewWindow window, bool currentCameraRender);

    public class SceneViewWindow : LayoutContainer, IDisposable, CameraMotionValidator
    {
        public event SceneViewWindowEvent CameraCreated;
        public event SceneViewWindowEvent CameraDestroyed;
        public event SceneViewWindowRenderEvent FindVisibleObjects;

        private SceneView sceneView;
        private CameraMover cameraMover;
        private UpdateTimer mainTimer;
        private RendererWindow window;
        private String name;

        private Vector3 startPosition;
        private Vector3 startLookAt;

        public SceneViewWindow(UpdateTimer mainTimer, CameraMover cameraMover, String name)
        {
            this.cameraMover = cameraMover;
            cameraMover.MotionValidator = this;
            this.name = name;
            this.mainTimer = mainTimer;
            this.startPosition = cameraMover.Translation;
            this.startLookAt = cameraMover.LookAt;
            mainTimer.addFixedUpdateListener(cameraMover);
        }

        public void Dispose()
        {
            mainTimer.removeFixedUpdateListener(cameraMover);
        }

        public void createSceneView(RendererWindow window, SimScene scene)
        {
            Log.Info("Creating SceneView for {0}.", name);
            this.window = window;
            SimSubScene defaultScene = scene.getDefaultSubScene();

            sceneView = window.createSceneView(defaultScene, name, cameraMover.Translation, cameraMover.LookAt);
            sceneView.BackgroundColor = Engine.Color.Black;
            sceneView.addLight();
            sceneView.setNearClipDistance(1.0f);
            sceneView.setFarClipDistance(1000.0f);
            //camera.setRenderingMode(renderingMode);
            cameraMover.setCamera(sceneView);
            CameraResolver.addMotionValidator(this);
            sceneView.showSceneStats(true);
            //basicGUI.ScreenLayout.Root.Center = new SceneViewLayoutItem(sceneView);
            //OgreCameraControl ogreCamera = ((OgreCameraControl)camera);
            sceneView.FindVisibleObjects += sceneView_FindVisibleObjects;
            if (CameraCreated != null)
            {
                CameraCreated.Invoke(this);
            }
        }

        public void destroySceneView()
        {
            if (sceneView != null)
            {
                Log.Info("Destroying SceneView for {0}.", name);
                CameraResolver.removeMotionValidator(this);
                sceneView.FindVisibleObjects -= sceneView_FindVisibleObjects;
                cameraMover.setCamera(null);
                window.destroySceneView(sceneView);
                sceneView = null;
                if (CameraDestroyed != null)
                {
                    CameraDestroyed.Invoke(this);
                }
            }
        }

        public void setPosition(Vector3 translation, Vector3 lookAt)
        {
            cameraMover.setNewPosition(translation, lookAt);
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
            if (sceneView != null)
            {
                sceneView.setDimensions(Location.x / totalSize.Width, Location.y / totalSize.Height, WorkingSize.Width / totalSize.Width, WorkingSize.Height / totalSize.Height);
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

        public String Name
        {
            get
            {
                return name;
            }
        }

        void sceneView_FindVisibleObjects(SceneView sceneView)
        {
            if (FindVisibleObjects != null)
            {
                FindVisibleObjects.Invoke(this, sceneView.CurrentlyRendering);
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
            return (x > Location.x && x < Location.x + WorkingSize.Width) && (y > Location.y && y < Location.y + WorkingSize.Height);
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
