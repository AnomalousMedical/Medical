using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    public abstract class CameraMover : UpdateListener
    {
        /// <summary>
        /// set the current camera for this controller. This can be set to null to disable the controller.
        /// </summary>
        /// <param name="camera">The camera to use.</param>
        public abstract void setCamera(CameraControl camera);

        /// <summary>
        /// Set the camera to the given position looking at the given point.
        /// </summary>
        /// <param name="position">The position to set the camera at.</param>
        /// <param name="lookAt">The look at point of the camera.</param>
        public abstract void setNewPosition(Vector3 position, Vector3 lookAt);

        #region UpdateListener Members

        public abstract void exceededMaxDelta();

        public abstract void loopStarting();

        public abstract void sendUpdate(Clock clock);

        #endregion

        public abstract Vector3 Translation
        {
            get;
        }

        public abstract Vector3 LookAt
        {
            get;
        }

        public abstract CameraMotionValidator MotionValidator
        {
            get;
            set;
        }

        public abstract bool AllowRotation
        {
            get;
            set;
        }

        public abstract bool AllowZoom
        {
            get;
            set;
        }

        public abstract float OrbitDistance
        {
            get;
        }
    }
}
