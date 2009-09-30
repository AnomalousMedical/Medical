using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class CloneCamera : CameraMover
    {
        private DrawingWindow followWindow;
        private CameraControl camera;

        public CloneCamera(DrawingWindow followWindow)
        {
            this.followWindow = followWindow;
        }

        public override void setCamera(CameraControl camera)
        {
            this.camera = camera;
        }

        public override void setNewPosition(Vector3 position, Engine.Vector3 lookAt)
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
    }
}
