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
        private SceneViewWindow followWindow;
        private SceneView camera;

        public CloneCamera(SceneViewWindow followWindow)
        {
            this.followWindow = followWindow;
        }

        public override void setCamera(SceneView camera)
        {
            this.camera = camera;
        }

        public override void setNewPosition(Vector3 position, Engine.Vector3 lookAt)
        {

        }

        public override void immediatlySetPosition(Vector3 translation, Vector3 lookAt)
        {

        }

        public override void sendUpdate(Clock clock)
        {
            if (camera != null)
            {
                camera.Translation = followWindow.Translation;
                camera.LookAt = followWindow.LookAt;
            }
        }

        public override Vector3 Translation
        {
            get
            {
                return followWindow.Translation;
            }
        }

        public override Vector3 LookAt
        {
            get
            {
                return followWindow.LookAt;
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
    }
}
