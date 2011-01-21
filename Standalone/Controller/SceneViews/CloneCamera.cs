using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical.Controller
{
    class CloneCamera : CameraMover
    {
        private SceneViewController sceneViewController;
        private SceneView camera;

        public CloneCamera(SceneViewController sceneViewController)
        {
            this.sceneViewController = sceneViewController;
        }

        public override void setCamera(SceneView camera)
        {
            this.camera = camera;
        }

        public override void setNewPosition(Vector3 position, Engine.Vector3 lookAt)
        {

        }

        public override void setNewPosition(Vector3 position, Vector3 lookAt, float duration)
        {

        }

        public override void immediatlySetPosition(Vector3 translation, Vector3 lookAt)
        {

        }

        public override void sendUpdate(Clock clock)
        {
            SceneViewWindow followWindow = sceneViewController.ActiveWindow;
            if (camera != null && followWindow != null)
            {
                camera.Translation = followWindow.Translation;
                camera.LookAt = followWindow.LookAt;
            }
        }

        public override Vector3 Translation
        {
            get
            {
                SceneViewWindow followWindow = sceneViewController.ActiveWindow;
                if (followWindow != null)
                {
                    return followWindow.Translation;
                }
                return Vector3.Zero;
            }
        }

        public override Vector3 LookAt
        {
            get
            {
                SceneViewWindow followWindow = sceneViewController.ActiveWindow;
                if (followWindow != null)
                {
                    return followWindow.LookAt;
                }
                return Vector3.Zero;
            }
        }

        public override CameraMotionValidator MotionValidator
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public override float PitchVelocity { get; set; }

        public override float YawVelocity { get; set; }

        public override Vector2 PanVelocity { get; set; }

        public override float ZoomVelocity { get; set; }
    }
}
