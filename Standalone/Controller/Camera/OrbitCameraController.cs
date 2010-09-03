using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Logging;

namespace Medical
{
    enum CameraEvents
    {
        RotateCamera,
        PanCamera,
        ZoomCamera,
        LockX,
        LockY,
    }

    public class OrbitCameraController : CameraMover
    {
        #region Static

        private readonly Vector3 LOOK_AT_BOUND_MAX = new Vector3(15.0f, 15.0f, 15.0f);
        private readonly Vector3 LOOK_AT_BOUND_MIN = new Vector3(-15.0f, -20.0f, -15.0f);

        static OrbitCameraController()
        {
            MessageEvent rotateCamera = new MessageEvent(CameraEvents.RotateCamera);
            rotateCamera.addButton(MouseButtonCode.MB_BUTTON1);
            DefaultEvents.registerDefaultEvent(rotateCamera);

            MessageEvent panCamera = new MessageEvent(CameraEvents.PanCamera);
            panCamera.addButton(MouseButtonCode.MB_BUTTON1);
            panCamera.addButton(KeyboardButtonCode.KC_LCONTROL);
            DefaultEvents.registerDefaultEvent(panCamera);

            MessageEvent zoomCamera = new MessageEvent(CameraEvents.ZoomCamera);
            zoomCamera.addButton(MouseButtonCode.MB_BUTTON1);
            zoomCamera.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(zoomCamera);

            MessageEvent lockX = new MessageEvent(CameraEvents.LockX);
            lockX.addButton(KeyboardButtonCode.KC_C);
            DefaultEvents.registerDefaultEvent(lockX);

            MessageEvent lockY = new MessageEvent(CameraEvents.LockY);
            lockY.addButton(KeyboardButtonCode.KC_X);
            DefaultEvents.registerDefaultEvent(lockY);
        }

        private const float HALF_PI = (float)Math.PI / 2.0f - 0.001f;
        private const int SCROLL_SCALE = 5;

        #endregion Static

        private SceneView camera;
        private EventManager events;

        //These three vectors form the axis relative to the current rotation.
        private Vector3 normalDirection; //z
        private Vector3 rotatedLeft; //x
        private Vector3 rotatedUp; //y

        private float orbitDistance;
        private float yaw;
        private float pitch;
        private bool currentlyInMotion;
        private Vector3 lookAt;
        private Vector3 translation;
        private CameraMotionValidator motionValidator = null;

        private bool automaticMovement = false;
        private float totalTime = 0.0f;
        private Vector3 startLookAt;
        private float startOrbitDistance;
        private Quaternion startRotation;
        private Vector3 targetLookAt;
        private float targetOrbitDistance;
        private Quaternion targetRotation;
        float targetYaw;
        float targetPitch;
        Vector3 targetNormal;
        Vector3 targetRotatedUp;
        Vector3 targetRotatedLeft;

        private bool allowRotation = true;
        private bool allowZoom = true;

        public OrbitCameraController(Vector3 translation, Vector3 lookAt, CameraMotionValidator motionValidator, EventManager eventManager)
        {
            this.camera = null;
            this.events = eventManager;
            this.translation = translation;
            this.lookAt = lookAt;
            this.motionValidator = motionValidator;
            computeStartingValues(translation - lookAt, out orbitDistance, out yaw, out pitch, out normalDirection, out rotatedUp, out rotatedLeft);
        }

        public override void sendUpdate(Clock clock)
        {
            if (camera != null)
            {
                if (automaticMovement)
                {
                    totalTime += (float)clock.Seconds;
                    if (totalTime > MedicalConfig.CameraTransitionTime)
                    {
                        totalTime = MedicalConfig.CameraTransitionTime;
                        automaticMovement = false;

                        orbitDistance = targetOrbitDistance;
                        yaw = targetYaw;
                        pitch = targetPitch;
                        normalDirection = targetNormal;
                        rotatedUp = targetRotatedUp;
                        rotatedLeft = targetRotatedLeft;
                    }
                    float slerpAmount = totalTime / MedicalConfig.CameraTransitionTime;
                    this.lookAt = startLookAt.lerp(ref targetLookAt, ref slerpAmount);
                    Quaternion rotation = startRotation.slerp(ref targetRotation, slerpAmount);
                    //If the rotation is not a valid number just use the target rotation
                    if (!rotation.isNumber())
                    {
                        rotation = targetRotation;
                    }
                    Vector3 currentNormalDirection = Quaternion.quatRotate(ref rotation, ref Vector3.Backward);
                    float currentOrbit = startOrbitDistance + (targetOrbitDistance - startOrbitDistance) * slerpAmount;
                    updateTranslation(currentNormalDirection * currentOrbit + lookAt);
                    camera.LookAt = lookAt;
                }
                else
                {
                    manualMove();
                }
            }
        }

        private void manualMove()
        {
            Vector3 mouseCoords = events.Mouse.getAbsMouse();
            bool activeWindow = motionValidator == null || (motionValidator.allowMotion((int)mouseCoords.x, (int)mouseCoords.y) && motionValidator.isActiveWindow());
            if (events[CameraEvents.RotateCamera].FirstFrameDown)
            {
                if (activeWindow)
                {
                    currentlyInMotion = true;
                }
            }
            else if (events[CameraEvents.RotateCamera].FirstFrameUp)
            {
                currentlyInMotion = false;
            }
            mouseCoords = events.Mouse.getRelMouse();
            if (currentlyInMotion)
            {
                if (events[CameraEvents.PanCamera].Down)
                {
                    float scaleFactor = orbitDistance > 5.0f ? orbitDistance : 5.0f;
                    if (events[CameraEvents.LockX].Up)
                    {
                        lookAt += rotatedLeft * (mouseCoords.x / (events.Mouse.getMouseAreaWidth() * SCROLL_SCALE) * scaleFactor);
                    }
                    if (events[CameraEvents.LockY].Up)
                    {
                        lookAt += rotatedUp * (mouseCoords.y / (events.Mouse.getMouseAreaHeight() * SCROLL_SCALE) * scaleFactor);
                    }
                    //Restrict look at position
                    if (lookAt.x > LOOK_AT_BOUND_MAX.x)
                    {
                        lookAt.x = LOOK_AT_BOUND_MAX.x;
                    }
                    else if (lookAt.x < LOOK_AT_BOUND_MIN.x)
                    {
                        lookAt.x = LOOK_AT_BOUND_MIN.x;
                    }
                    if (lookAt.y > LOOK_AT_BOUND_MAX.y)
                    {
                        lookAt.y = LOOK_AT_BOUND_MAX.y;
                    }
                    else if (lookAt.y < LOOK_AT_BOUND_MIN.y)
                    {
                        lookAt.y = LOOK_AT_BOUND_MIN.y;
                    }
                    if (lookAt.z > LOOK_AT_BOUND_MAX.z)
                    {
                        lookAt.z = LOOK_AT_BOUND_MAX.z;
                    }
                    else if (lookAt.z < LOOK_AT_BOUND_MIN.z)
                    {
                        lookAt.z = LOOK_AT_BOUND_MIN.z;
                    }

                    updateTranslation(lookAt + normalDirection * orbitDistance);
                }
                else if (allowZoom && events[CameraEvents.ZoomCamera].Down)
                {
                    orbitDistance += mouseCoords.y;
                    if (orbitDistance < 0.2f)
                    {
                        orbitDistance = 0.2f;
                    }
                    if (orbitDistance > 500.0f)
                    {
                        orbitDistance = 500.0f;
                    }
                    updateTranslation(normalDirection * orbitDistance + lookAt);
                }
                else if (allowRotation && events[CameraEvents.RotateCamera].Down)
                {
                    if (events[CameraEvents.LockX].Up)
                    {
                        yaw += mouseCoords.x / -100.0f;
                    }
                    if (events[CameraEvents.LockY].Up)
                    {
                        pitch += mouseCoords.y / 100.0f;
                    }
                    if (pitch > HALF_PI)
                    {
                        pitch = HALF_PI;
                    }
                    if (pitch < -HALF_PI)
                    {
                        pitch = -HALF_PI;
                    }

                    Quaternion yawRot = new Quaternion(Vector3.Up, yaw);
                    Quaternion pitchRot = new Quaternion(Vector3.Left, pitch);

                    Quaternion rotation = yawRot * pitchRot;
                    normalDirection = Quaternion.quatRotate(ref rotation, ref Vector3.Backward);
                    rotatedUp = Quaternion.quatRotate(ref rotation, ref Vector3.Up);
                    rotatedLeft = normalDirection.cross(ref rotatedUp);

                    updateTranslation(normalDirection * orbitDistance + lookAt);
                    camera.LookAt = lookAt;
                }
            }
            if (activeWindow)
            {
                if (allowZoom && mouseCoords.z != 0)
                {
                    if (mouseCoords.z < 0)
                    {
                        orbitDistance += 3.6f;
                        if (orbitDistance > 500.0f)
                        {
                            orbitDistance = 500.0f;
                        }
                    }
                    else if (mouseCoords.z > 0)
                    {
                        orbitDistance -= 3.6f;
                        if (orbitDistance < 0.0f)
                        {
                            orbitDistance = 0.0f;
                        }
                    }
                    updateTranslation(normalDirection * orbitDistance + lookAt);
                }
            }
        }

        /// <summary>
        /// set the current camera for this controller. This can be set to null to disable the controller.
        /// </summary>
        /// <param name="camera">The camera to use.</param>
        public override void setCamera(SceneView camera)
        {
            this.camera = camera;
        }

        /// <summary>
        /// Set the camera to the given position looking at the given point.
        /// </summary>
        /// <param name="position">The position to set the camera at.</param>
        /// <param name="lookAt">The look at point of the camera.</param>
        public override void setNewPosition(Vector3 position, Vector3 lookAt)
        {
            if (camera != null)
            {
                //If the camera is currently moving the final positions are not yet recorded so do that now.
                if (automaticMovement)
                {
                    computeStartingValues(translation - this.lookAt, out orbitDistance, out yaw, out pitch, out normalDirection, out rotatedUp, out rotatedLeft);
                }

                //Starting position
                startLookAt = this.lookAt;
                startOrbitDistance = orbitDistance;

                //Target position
                Vector3 localVec = position - lookAt;
                computeStartingValues(localVec, out targetOrbitDistance, out targetYaw, out targetPitch, out targetNormal, out targetRotatedUp, out targetRotatedLeft);
                this.targetLookAt = lookAt;

                //Rotations
                Quaternion yawRot = new Quaternion(Vector3.Up, yaw);
                Quaternion pitchRot = new Quaternion(Vector3.Left, pitch);
                Quaternion targetYawRot = new Quaternion(Vector3.Up, targetYaw);
                Quaternion targetPitchRot = new Quaternion(Vector3.Left, targetPitch);
                if (targetYaw < yaw)
                {
                    float oneRotYaw = targetYaw + 2f * (float)Math.PI;
                    float oneRotYawMinus180 = oneRotYaw - (float)Math.PI;
                    if (yaw > oneRotYawMinus180 && yaw < oneRotYaw)
                    {
                        targetYawRot = new Quaternion(Vector3.Up, oneRotYaw);
                    }
                }
                if (targetYaw > yaw)
                {
                    float yawDifference = targetYaw - yaw;
                    if (yawDifference > Math.PI)
                    {
                        float negRot = targetYaw - 2f * (float)Math.PI;
                        targetYawRot = new Quaternion(Vector3.Up, negRot);
                    }
                }
                startRotation = yawRot * pitchRot;
                targetRotation = targetYawRot * targetPitchRot;
                automaticMovement = true;
                totalTime = 0.0f;
            }
        }

        public override void immediatlySetPosition(Vector3 translation, Vector3 lookAt)
        {
            this.translation = translation;
            this.lookAt = lookAt;
            computeStartingValues(translation - lookAt, out orbitDistance, out yaw, out pitch, out normalDirection, out rotatedUp, out rotatedLeft);
        }

        /// <summary>
        /// Helper function to compute the starting orbitDistance, yaw, pitch,
        /// normalDirection and left values.
        /// </summary>
        /// <param name="localTrans">The translation of the camera relative to the look at point.</param>
        private static void computeStartingValues(Vector3 localTrans, out float orbitDistance, out float yaw, out float pitch, out Vector3 normalDirection, out Vector3 rotatedUp, out Vector3 rotatedLeft)
        {
            //Compute the orbit distance, this is the distance from the location to the look at point.
            orbitDistance = localTrans.length();

            //Compute the yaw.
            float localY = localTrans.y;
            localTrans.y = 0;
            yaw = Vector3.Backward.angle(ref localTrans);
            if (localTrans.x < 0)
            {
                yaw = -yaw;
            }

            //Compute the pitch by rotating the local translation to -yaw.
            localTrans.y = localY;
            localTrans = Quaternion.quatRotate(new Quaternion(Vector3.Up, -yaw), localTrans);
            localTrans.x = 0;
            pitch = Vector3.Backward.angle(ref localTrans);
            if (localTrans.y < 0)
            {
                pitch = -pitch;
            }

            //Compute the normal direction and the left vector.
            Quaternion yawRot = new Quaternion(Vector3.Up, yaw);
            Quaternion pitchRot = new Quaternion(Vector3.Left, pitch);
            Quaternion rotation = yawRot * pitchRot;
            normalDirection = Quaternion.quatRotate(ref rotation, ref Vector3.Backward);
            rotatedUp = Quaternion.quatRotate(ref rotation, ref Vector3.Up);
            rotatedLeft = normalDirection.cross(ref rotatedUp);
        }

        private void updateTranslation(Vector3 translation)
        {
            camera.Translation = translation;
            this.translation = translation;
        }

        public override Vector3 Translation
        {
            get
            {
                return translation;
            }
        }

        public override Vector3 LookAt
        {
            get
            {
                return lookAt;
            }
        }

        public override CameraMotionValidator MotionValidator
        {
            get
            {
                return motionValidator;
            }
            set
            {
                motionValidator = value;
            }
        }

        public bool AllowRotation
        {
            get
            {
                return allowRotation;
            }
            set
            {
                allowRotation = value;
            }
        }

        public bool AllowZoom
        {
            get
            {
                return allowZoom;
            }
            set
            {
                allowZoom = value;
            }
        }
    }
}
