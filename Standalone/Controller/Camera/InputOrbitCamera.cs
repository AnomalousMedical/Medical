using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class InputOrbitCamera : OrbitCameraController
    {
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
