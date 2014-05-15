using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Logging;
using MyGUIPlugin;
using Medical.Controller;
using OgreWrapper;

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

        private static MouseButtonCode currentMouseButton = MedicalConfig.CameraMouseButton;
        private static MessageEvent rotateCamera;
        private static MessageEvent panCamera;
        private static MessageEvent zoomCamera;

        static OrbitCameraController()
        {
            rotateCamera = new MessageEvent(CameraEvents.RotateCamera);
            rotateCamera.addButton(currentMouseButton);
            DefaultEvents.registerDefaultEvent(rotateCamera);

            panCamera = new MessageEvent(CameraEvents.PanCamera);
            panCamera.addButton(currentMouseButton);
            panCamera.addButton(PlatformConfig.PanKey);
            DefaultEvents.registerDefaultEvent(panCamera);

            zoomCamera = new MessageEvent(CameraEvents.ZoomCamera);
            zoomCamera.addButton(currentMouseButton);
            zoomCamera.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(zoomCamera);

            MessageEvent lockX = new MessageEvent(CameraEvents.LockX);
            lockX.addButton(KeyboardButtonCode.KC_C);
            DefaultEvents.registerDefaultEvent(lockX);

            MessageEvent lockY = new MessageEvent(CameraEvents.LockY);
            lockY.addButton(KeyboardButtonCode.KC_X);
            DefaultEvents.registerDefaultEvent(lockY);
        }

        public static void changeMouseButton(MouseButtonCode newMouseButton)
        {
            rotateCamera.removeButton(currentMouseButton);
            panCamera.removeButton(currentMouseButton);
            zoomCamera.removeButton(currentMouseButton);

            currentMouseButton = newMouseButton;

            rotateCamera.addButton(currentMouseButton);
            panCamera.addButton(currentMouseButton);
            zoomCamera.addButton(currentMouseButton);
        }

        private const float HALF_PI = (float)Math.PI / 2.0f - 0.001f;
        private const int SCROLL_SCALE = 5;

        #endregion Static

        private Vector3 boundMax;
        private Vector3 boundMin;
        private float minOrbitDistance;
        private float maxOrbitDistance;

        private CameraPositioner camera;
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
        private float animationDuration = 0.0f;
        private Vector3 startLookAt;
        private float startOrbitDistance;
        private Quaternion startRotation;
        private Vector3 targetLookAt;
        private Vector3 targetPosition;
        private float targetOrbitDistance;
        private Quaternion targetRotation;
        float targetYaw;
        float targetPitch;
        Vector3 targetNormal;
        Vector3 targetRotatedUp;
        Vector3 targetRotatedLeft;

        //Velocity move variables
        private float pitchVelocity = 0.0f;
        private float yawVelocity = 0.0f;
        private Vector2 panVelocity = new Vector2(0.0f, 0.0f);
        private float zoomVelocity = 0.0f;

        private bool allowRotation = true;
        private bool allowZoom = true;
        private EasingFunction easingFunction = EasingFunction.EaseOutQuadratic;

        public OrbitCameraController(Vector3 translation, Vector3 lookAt, Vector3 boundMin, Vector3 boundMax, float minOrbitDistance, float maxOrbitDistance, CameraMotionValidator motionValidator, EventManager eventManager)
        {
            this.camera = null;
            this.events = eventManager;
            this.translation = translation;
            this.lookAt = lookAt;
            this.motionValidator = motionValidator;
            this.boundMax = boundMax;
            this.boundMin = boundMin;
            this.minOrbitDistance = minOrbitDistance;
            this.maxOrbitDistance = maxOrbitDistance;
            computeStartingValues(translation - lookAt, out orbitDistance, out yaw, out pitch, out normalDirection, out rotatedUp, out rotatedLeft);
        }

        public override void rotate(float yawDelta, float pitchDelta)
        {
            yaw += yawDelta;
            pitch += pitchDelta;
            moveCameraYawPitch();
        }

        public override void pan(float xDelta, float yDelta)
        {
            lookAt += rotatedLeft * xDelta;
            lookAt += rotatedUp * yDelta;
            moveLookAt();
        }

        public override void zoom(float zoomDelta)
        {
            orbitDistance += zoomDelta;
            moveZoom();
        }

        public override void sendUpdate(Clock clock)
        {
            if (camera != null)
            {
                if (automaticMovement)
                {
                    totalTime += (float)clock.Seconds;
                    if (totalTime > animationDuration)
                    {
                        totalTime = animationDuration;
                        automaticMovement = false;

                        orbitDistance = targetOrbitDistance;
                        yaw = targetYaw;
                        pitch = targetPitch;
                        normalDirection = targetNormal;
                        rotatedUp = targetRotatedUp;
                        rotatedLeft = targetRotatedLeft;
                    }
                    float slerpAmount = EasingFunctions.Ease(easingFunction, 0f, 1f, totalTime, animationDuration);
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
                else if (!velocityMove() && !Gui.Instance.HandledMouseButtons)
                {
                    mouseMove();
                }
            }
        }

        /// <summary>
        /// This funciton will move stuff based on velocities. This allows joystick control of the camera.
        /// </summary>
        /// <returns></returns>
        private bool velocityMove()
        {
            bool moved = false;
            if (yawVelocity != 0.0f || pitchVelocity != 0.0f)
            {
                yaw += yawVelocity;
                pitch += pitchVelocity;
                moveCameraYawPitch();
                moved = true;
            }
            if (panVelocity.x != 0.0f || panVelocity.y != 0.0f)
            {
                lookAt += rotatedLeft * panVelocity.x;
                lookAt += rotatedUp * panVelocity.y;
                moveLookAt();
                moved = true;
            }
            if (zoomVelocity != 0.0f)
            {
                orbitDistance += zoomVelocity;
                moveZoom();
                moved = true;
            }
            return moved;
        }

        private void mouseMove()
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
                    moveLookAt();
                    stopMaintainingIncludePoint();
                }
                else if (allowZoom && events[CameraEvents.ZoomCamera].Down)
                {
                    orbitDistance += mouseCoords.y;
                    moveZoom();
                    stopMaintainingIncludePoint();
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
                    moveCameraYawPitch();
                    stopMaintainingIncludePoint();
                }
            }
            if (activeWindow)
            {
                if (allowZoom && mouseCoords.z != 0)
                {
                    if (mouseCoords.z < 0)
                    {
                        orbitDistance += 3.6f;
                        if (orbitDistance > maxOrbitDistance)
                        {
                            orbitDistance = maxOrbitDistance;
                        }
                    }
                    else if (mouseCoords.z > 0)
                    {
                        orbitDistance -= 3.6f;
                        if (orbitDistance < minOrbitDistance)
                        {
                            orbitDistance = minOrbitDistance;
                        }
                    }
                    updateTranslation(normalDirection * orbitDistance + lookAt);
                    stopMaintainingIncludePoint();
                }
            }
        }

        private void moveZoom()
        {
            if (orbitDistance < minOrbitDistance)
            {
                orbitDistance = minOrbitDistance;
            }
            if (orbitDistance > maxOrbitDistance)
            {
                orbitDistance = maxOrbitDistance;
            }
            updateTranslation(normalDirection * orbitDistance + lookAt);
        }

        private void moveLookAt()
        {
            //Restrict look at position
            if (lookAt.x > boundMax.x)
            {
                lookAt.x = boundMax.x;
            }
            else if (lookAt.x < boundMin.x)
            {
                lookAt.x = boundMin.x;
            }
            if (lookAt.y > boundMax.y)
            {
                lookAt.y = boundMax.y;
            }
            else if (lookAt.y < boundMin.y)
            {
                lookAt.y = boundMin.y;
            }
            if (lookAt.z > boundMax.z)
            {
                lookAt.z = boundMax.z;
            }
            else if (lookAt.z < boundMin.z)
            {
                lookAt.z = boundMin.z;
            }

            updateTranslation(lookAt + normalDirection * orbitDistance);
        }

        /// <summary>
        /// Helper funciton to move the camera when the yaw and pitch have changed.
        /// </summary>
        private void moveCameraYawPitch()
        {
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

        /// <summary>
        /// set the current camera for this controller. This can be set to null to disable the controller.
        /// </summary>
        /// <param name="camera">The camera to use.</param>
        public override void setCamera(CameraPositioner camera)
        {
            this.camera = camera;
        }

        public override void setNewPosition(Vector3 position, Vector3 lookAt, float duration, EasingFunction easingFunction)
        {
            this.easingFunction = easingFunction;
            animationDuration = duration;
            if (animationDuration < 0.001f) //Be sure the duration is not 0.
            {
                animationDuration = 0.001f;
            }
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
                this.targetPosition = position;

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
            if(camera != null)
            {
                updateTranslation(translation);
                camera.LookAt = lookAt;
            }
        }

        public override void processIncludePoint(Camera camera)
        {
            if (currentIncludePoint.HasValue)
            {
                float duration = MedicalConfig.CameraTransitionTime;
                Vector3 inclLookAt = LookAt;
                Vector3 inclTrans = Translation;
                if (automaticMovement)
                {
                    duration = animationDuration - totalTime;
                    inclLookAt = targetLookAt;
                    inclTrans = targetPosition;
                }
                setNewPosition(SceneViewWindow.computeIncludePointAdjustedPosition(camera.getAspectRatio(), camera.getFOVy(), camera.getProjectionMatrix(), inclTrans, inclLookAt, currentIncludePoint.Value), inclLookAt, duration, easingFunction);
            }
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

        public override float YawVelocity
        {
            get
            {
                return yawVelocity;
            }
            set
            {
                yawVelocity = value;
            }
        }

        public override float PitchVelocity
        {
            get
            {
                return pitchVelocity;
            }
            set
            {
                pitchVelocity = value;
            }
        }

        public override float ZoomVelocity
        {
            get
            {
                return zoomVelocity;
            }
            set
            {
                zoomVelocity = value;
            }
        }

        public override Vector2 PanVelocity
        {
            get
            {
                return panVelocity;
            }
            set
            {
                panVelocity = value;
            }
        }

        public Vector3 BoundMax
        {
            get
            {
                return boundMax;
            }
            set
            {
                boundMax = value;
            }
        }

        public Vector3 BoundMin
        {
            get
            {
                return boundMin;
            }
            set
            {
                boundMin = value;
            }
        }
    }
}
