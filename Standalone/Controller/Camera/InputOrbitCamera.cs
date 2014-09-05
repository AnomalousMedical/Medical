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
        private EventManager eventManager;
        private MouseTravelTracker travelTracker = new MouseTravelTracker();

        public InputOrbitCamera(Vector3 translation, Vector3 lookAt, Vector3 boundMin, Vector3 boundMax, float minOrbitDistance, float maxOrbitDistance, EventManager eventManager)
            :base(translation, lookAt, boundMin, boundMax, minOrbitDistance, maxOrbitDistance)
        {
            this.eventManager = eventManager;
            eventManager[EventLayers.Cameras].OnUpdate += processInputEvents;

            rotateCamera.FirstFrameDownEvent += rotateCamera_FirstFrameDownEvent;
            rotateCamera.FirstFrameUpEvent += rotateCamera_FirstFrameUpEvent;
        }

        public override void Dispose()
        {
            eventManager[EventLayers.Cameras].OnUpdate -= processInputEvents;

            rotateCamera.FirstFrameDownEvent -= rotateCamera_FirstFrameDownEvent;
            rotateCamera.FirstFrameUpEvent -= rotateCamera_FirstFrameUpEvent;
            base.Dispose();
        }

        public CameraMotionValidator MotionValidator { get; set; }

        void rotateCamera_FirstFrameDownEvent(EventLayer eventLayer)
        {
            travelTracker.reset();
        }

        void rotateCamera_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if(travelTracker.TraveledOverLimit)
            {
                eventLayer.alertEventsHandled();
            }
        }

        void processInputEvents(EventLayer eventLayer)
        {
            if (AllowManualMovement && eventLayer.EventProcessingAllowed)
            {
                IntVector3 mouseCoords = eventLayer.Mouse.AbsolutePosition;
                bool activeWindow = MotionValidator == null || (MotionValidator.allowMotion((int)mouseCoords.x, (int)mouseCoords.y) && MotionValidator.isActiveWindow());
                if (eventLayer[CameraEvents.RotateCamera].FirstFrameDown)
                {
                    if (activeWindow)
                    {
                        currentlyInMotion = true;
                        eventLayer.alertEventsHandled();
                    }
                }
                else if (eventLayer[CameraEvents.RotateCamera].FirstFrameUp)
                {
                    currentlyInMotion = false;
                }
                mouseCoords = eventLayer.Mouse.RelativePosition;
                if (currentlyInMotion)
                {
                    travelTracker.traveled(mouseCoords);
                    if (eventLayer[CameraEvents.PanCamera].HeldDown)
                    {
                        int x = mouseCoords.x;
                        int y = mouseCoords.y;
                        if (eventLayer[CameraEvents.LockX].Down)
                        {
                            x = 0;
                        }
                        if (eventLayer[CameraEvents.LockY].Down)
                        {
                            y = 0;
                        }
                        panFromMotion(x, y, eventLayer.Mouse.AreaWidth, eventLayer.Mouse.AreaHeight);
                        eventLayer.alertEventsHandled();
                    }
                    else if (AllowZoom && eventLayer[CameraEvents.ZoomCamera].HeldDown)
                    {
                        zoomFromMotion(mouseCoords.y);
                        eventLayer.alertEventsHandled();
                    }
                    else if (AllowRotation && eventLayer[CameraEvents.RotateCamera].HeldDown)
                    {
                        int x = mouseCoords.x;
                        int y = mouseCoords.y;
                        if (eventLayer[CameraEvents.LockX].Down)
                        {
                            x = 0;
                        }
                        if (eventLayer[CameraEvents.LockY].Down)
                        {
                            y = 0;
                        }
                        rotateFromMotion(x, y);
                        eventLayer.alertEventsHandled();
                    }
                }
                if (activeWindow)
                {
                    if (AllowZoom)
                    {
                        if (eventLayer[CameraEvents.ZoomInCamera].Down)
                        {
                            incrementZoom(-1);
                            eventLayer.alertEventsHandled();
                        }
                        else if (eventLayer[CameraEvents.ZoomOutCamera].Down)
                        {
                            incrementZoom(1);
                            eventLayer.alertEventsHandled();
                        }
                    }
                }
            }
        }
    }
}
