using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This is an orbit camera that can process input from the user to move the camera.
    /// </summary>
    class InputOrbitCamera : OrbitCameraController
    {
        private static MouseButtonCode currentMouseButton = MedicalConfig.CameraMouseButton;
        private static MessageEvent rotateCamera;
        private static MessageEvent panCamera;
        private static MessageEvent zoomCamera;
        private static MessageEvent zoomInCamera;
        private static MessageEvent zoomOutCamera;

        static InputOrbitCamera()
        {
            rotateCamera = new MessageEvent(CameraEvents.RotateCamera, EventLayers.Cameras);
            rotateCamera.addButton(currentMouseButton);
            DefaultEvents.registerDefaultEvent(rotateCamera);

            panCamera = new MessageEvent(CameraEvents.PanCamera, EventLayers.Cameras);
            panCamera.addButton(currentMouseButton);
            panCamera.addButton(PlatformConfig.PanKey);
            DefaultEvents.registerDefaultEvent(panCamera);

            zoomCamera = new MessageEvent(CameraEvents.ZoomCamera, EventLayers.Cameras);
            zoomCamera.addButton(currentMouseButton);
            zoomCamera.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(zoomCamera);

            zoomInCamera = new MessageEvent(CameraEvents.ZoomInCamera, EventLayers.Cameras);
            zoomInCamera.MouseWheelDirection = MouseWheelDirection.Up;
            DefaultEvents.registerDefaultEvent(zoomInCamera);

            zoomOutCamera = new MessageEvent(CameraEvents.ZoomOutCamera, EventLayers.Cameras);
            zoomOutCamera.MouseWheelDirection = MouseWheelDirection.Down;
            DefaultEvents.registerDefaultEvent(zoomOutCamera);

            MessageEvent lockX = new MessageEvent(CameraEvents.LockX, EventLayers.Cameras);
            lockX.addButton(KeyboardButtonCode.KC_C);
            DefaultEvents.registerDefaultEvent(lockX);

            MessageEvent lockY = new MessageEvent(CameraEvents.LockY, EventLayers.Cameras);
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

        private bool currentlyInMotion;

        public InputOrbitCamera(Vector3 translation, Vector3 lookAt, Vector3 boundMin, Vector3 boundMax, float minOrbitDistance, float maxOrbitDistance, EventManager eventManager)
            :base(translation, lookAt, boundMin, boundMax, minOrbitDistance, maxOrbitDistance)
        {
            eventManager[EventLayers.Cameras].OnUpdate += processInputEvents;
        }

        public CameraMotionValidator MotionValidator { get; set; }

        void processInputEvents(EventLayer events)
        {
            if (AllowManualMovement && events.EventProcessingAllowed)
            {
                IntVector3 mouseCoords = events.Mouse.AbsolutePosition;
                bool activeWindow = MotionValidator == null || (MotionValidator.allowMotion((int)mouseCoords.x, (int)mouseCoords.y) && MotionValidator.isActiveWindow());
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
                mouseCoords = events.Mouse.RelativePosition;
                if (currentlyInMotion)
                {
                    if (events[CameraEvents.PanCamera].HeldDown)
                    {
                        int x = mouseCoords.x;
                        int y = mouseCoords.y;
                        if (events[CameraEvents.LockX].Down)
                        {
                            x = 0;
                        }
                        if (events[CameraEvents.LockY].Down)
                        {
                            y = 0;
                        }
                        panFromMotion(x, y, events.Mouse.AreaWidth, events.Mouse.AreaHeight);
                    }
                    else if (AllowZoom && events[CameraEvents.ZoomCamera].HeldDown)
                    {
                        zoomFromMotion(mouseCoords.y);
                    }
                    else if (AllowRotation && events[CameraEvents.RotateCamera].HeldDown)
                    {
                        int x = mouseCoords.x;
                        int y = mouseCoords.y;
                        if (events[CameraEvents.LockX].Down)
                        {
                            x = 0;
                        }
                        if (events[CameraEvents.LockY].Down)
                        {
                            y = 0;
                        }
                        rotateFromMotion(x, y);
                    }
                }
                if (activeWindow)
                {
                    if (AllowZoom)
                    {
                        if (events[CameraEvents.ZoomInCamera].Down)
                        {
                            incrementZoom(-1);
                            events.alertEventsHandled();
                        }
                        else if (events[CameraEvents.ZoomOutCamera].Down)
                        {
                            incrementZoom(1);
                            events.alertEventsHandled();
                        }
                    }
                }
            }
        }
    }
}
