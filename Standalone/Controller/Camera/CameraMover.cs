﻿using System;
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
        /// Rotate the camera by a certain amount (if supported).
        /// </summary>
        /// <param name="yawDelta"></param>
        /// <param name="pitchDelta"></param>
        public virtual void rotate(float yawDelta, float pitchDelta)
        {

        }

        public virtual void pan(float xDelta, float yDelta)
        {
            
        }

        public virtual void zoom(float zoomDelta)
        {
            
        }

        /// <summary>
        /// set the current camera for this controller. This can be set to null to disable the controller.
        /// </summary>
        /// <param name="camera">The camera to use.</param>
        public abstract void setCamera(SceneView camera);

        /// <summary>
        /// Set the camera to the given position looking at the given point.
        /// </summary>
        /// <param name="position">The position to set the camera at.</param>
        /// <param name="lookAt">The look at point of the camera.</param>
        public abstract void setNewPosition(Vector3 position, Vector3 lookAt);

        /// <summary>
        /// Set the camera to the given position looking at the given point.
        /// </summary>
        /// <param name="position">The position to set the camera at.</param>
        /// <param name="lookAt">The look at point of the camera.</param>
        /// <param name="duration">The amount of time for the transition to take.</param>
        public abstract void setNewPosition(Vector3 position, Vector3 lookAt, float duration);

        /// <summary>
        /// Set the position of the camera immediatly.
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="lookAt"></param>
        public abstract void immediatlySetPosition(Vector3 translation, Vector3 lookAt);

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

        public abstract CameraMotionValidator MotionValidator
        {
            get;
            set;
        }



        public abstract float YawVelocity
        {
            get;
            set;
        }

        public abstract float PitchVelocity
        {
            get;
            set;
        }

        public abstract Vector2 PanVelocity
        {
            get;
            set;
        }

        public abstract float ZoomVelocity
        {
            get;
            set;
        }
    }
}
