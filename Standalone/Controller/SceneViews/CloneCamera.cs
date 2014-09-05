using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgreWrapper;

namespace Medical.Controller
{
    class CloneCamera : CameraMover
    {
        private SceneViewController sceneViewController;
        private CameraPositioner camera;
        private UpdateTimer timer;

        public CloneCamera(SceneViewController sceneViewController, UpdateTimer timer)
        {
            this.sceneViewController = sceneViewController;
            this.timer = timer;
            timer.addUpdateListener(this);
        }

        public override void Dispose()
        {
            timer.removeUpdateListener(this);
            base.Dispose();
        }

        public override void setCamera(CameraPositioner camera)
        {
            this.camera = camera;
        }

        public override void setNewPosition(Vector3 position, Vector3 lookAt, float duration, EasingFunction easingFunction)
        {

        }

        public override void immediatlySetPosition(Vector3 translation, Vector3 lookAt)
        {

        }

        public override void processIncludePoint(Camera camera)
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
    }
}
