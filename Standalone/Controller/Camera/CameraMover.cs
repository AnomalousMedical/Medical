using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using OgreWrapper;

namespace Medical
{
    public abstract class CameraMover : UpdateListener, IDisposable
    {
        protected Vector3? currentIncludePoint = null;

        public virtual void Dispose()
        {

        }

        /// <summary>
        /// set the current camera for this controller. This can be set to null to disable the controller.
        /// </summary>
        /// <param name="camera">The camera to use.</param>
        public abstract void setCamera(CameraPositioner camera);

        /// <summary>
        /// Set the camera to the given position looking at the given point.
        /// </summary>
        /// <param name="position">The position to set the camera at.</param>
        /// <param name="lookAt">The look at point of the camera.</param>
        /// <param name="duration">The amount of time for the transition to take.</param>
        public abstract void setNewPosition(Vector3 position, Vector3 lookAt, float duration, EasingFunction easingFunction);

        /// <summary>
        /// Set the position of the camera immediatly.
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="lookAt"></param>
        public abstract void immediatlySetPosition(Vector3 translation, Vector3 lookAt);

        public void maintainIncludePoint(Vector3 includePoint)
        {
            this.currentIncludePoint = includePoint;
        }

        public void stopMaintainingIncludePoint()
        {
            this.currentIncludePoint = null;
        }

        public abstract void processIncludePoint(Camera camera);

        #region UpdateListener Members

        public virtual void exceededMaxDelta()
        {

        }

        public virtual void loopStarting()
        {

        }

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

        public virtual bool AllowManualMovement
        {
            get
            {
                return false;
            }
        }

        public virtual void panFromMotion(int x, int y, int areaWidth, int areaHeight)
        {
            
        }

        public virtual void zoomFromMotion(int y)
        {
            
        }

        public virtual void rotateFromMotion(int x, int y)
        {
            
        }

        public virtual void incrementZoom(int zoomDirection)
        {
            
        }

        public virtual bool AllowZoom
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public virtual bool AllowRotation
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
    }
}
