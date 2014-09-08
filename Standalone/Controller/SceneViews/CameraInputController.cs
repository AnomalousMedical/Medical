using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    class CameraInputController : IDisposable
    {
        private static MouseButtonCode currentMouseButton = MedicalConfig.CameraMouseButton;
        private static ButtonEvent RotateCamera;
        private static ButtonEvent PanCamera;
        private static ButtonEvent ZoomCamera;
        private static ButtonEvent ZoomInCamera;
        private static ButtonEvent ZoomOutCamera;
        private static ButtonEvent LockX;
        private static ButtonEvent LockY;

        static CameraInputController()
        {
            RotateCamera = new ButtonEvent(CameraEvents.RotateCamera, EventLayers.Cameras);
            RotateCamera.addButton(currentMouseButton);
            DefaultEvents.registerDefaultEvent(RotateCamera);

            PanCamera = new ButtonEvent(CameraEvents.PanCamera, EventLayers.Cameras);
            PanCamera.addButton(currentMouseButton);
            PanCamera.addButton(PlatformConfig.PanKey);
            DefaultEvents.registerDefaultEvent(PanCamera);

            ZoomCamera = new ButtonEvent(CameraEvents.ZoomCamera, EventLayers.Cameras);
            ZoomCamera.addButton(currentMouseButton);
            ZoomCamera.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(ZoomCamera);

            ZoomInCamera = new ButtonEvent(CameraEvents.ZoomInCamera, EventLayers.Cameras);
            ZoomInCamera.MouseWheelDirection = MouseWheelDirection.Up;
            DefaultEvents.registerDefaultEvent(ZoomInCamera);

            ZoomOutCamera = new ButtonEvent(CameraEvents.ZoomOutCamera, EventLayers.Cameras);
            ZoomOutCamera.MouseWheelDirection = MouseWheelDirection.Down;
            DefaultEvents.registerDefaultEvent(ZoomOutCamera);

            LockX = new ButtonEvent(CameraEvents.LockX, EventLayers.Cameras);
            LockX.addButton(KeyboardButtonCode.KC_C);
            DefaultEvents.registerDefaultEvent(LockX);

            LockY = new ButtonEvent(CameraEvents.LockY, EventLayers.Cameras);
            LockY.addButton(KeyboardButtonCode.KC_X);
            DefaultEvents.registerDefaultEvent(LockY);
        }

        public static void changeMouseButton(MouseButtonCode newMouseButton)
        {
            RotateCamera.removeButton(currentMouseButton);
            PanCamera.removeButton(currentMouseButton);
            ZoomCamera.removeButton(currentMouseButton);

            currentMouseButton = newMouseButton;

            RotateCamera.addButton(currentMouseButton);
            PanCamera.addButton(currentMouseButton);
            ZoomCamera.addButton(currentMouseButton);
        }

        private bool currentlyInMotion;
        private EventManager eventManager;
        private MouseTravelTracker travelTracker = new MouseTravelTracker();
        private SceneViewController sceneViewController;

        public CameraInputController(SceneViewController sceneViewController, EventManager eventManager)
        {
            this.sceneViewController = sceneViewController;
            this.eventManager = eventManager;
            eventManager[EventLayers.Cameras].OnUpdate += processInputEvents;

            RotateCamera.FirstFrameDownEvent += rotateCamera_FirstFrameDownEvent;
            RotateCamera.FirstFrameUpEvent += rotateCamera_FirstFrameUpEvent;
        }

        public void Dispose()
        {
            eventManager[EventLayers.Cameras].OnUpdate -= processInputEvents;

            RotateCamera.FirstFrameDownEvent -= rotateCamera_FirstFrameDownEvent;
            RotateCamera.FirstFrameUpEvent -= rotateCamera_FirstFrameUpEvent;
        }

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
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if (activeWindow != null && eventLayer.EventProcessingAllowed)
            {
                CameraMover cameraMover = activeWindow.CameraMover;
                if (cameraMover.AllowManualMovement)
                {
                    IntVector3 mouseCoords = eventLayer.Mouse.AbsolutePosition;

                    if (RotateCamera.FirstFrameDown)
                    {
                        currentlyInMotion = true;
                        eventLayer.alertEventsHandled();
                    }
                    else if (RotateCamera.FirstFrameUp)
                    {
                        currentlyInMotion = false;
                    }
                    mouseCoords = eventLayer.Mouse.RelativePosition;
                    if (currentlyInMotion)
                    {
                        travelTracker.traveled(mouseCoords);
                        if (PanCamera.HeldDown)
                        {
                            int x = mouseCoords.x;
                            int y = mouseCoords.y;
                            if (LockX.Down)
                            {
                                x = 0;
                            }
                            if (LockY.Down)
                            {
                                y = 0;
                            }
                            cameraMover.panFromMotion(x, y, eventLayer.Mouse.AreaWidth, eventLayer.Mouse.AreaHeight);
                            eventLayer.alertEventsHandled();
                        }
                        else if (cameraMover.AllowZoom && ZoomCamera.HeldDown)
                        {
                            cameraMover.zoomFromMotion(mouseCoords.y);
                            eventLayer.alertEventsHandled();
                        }
                        else if (cameraMover.AllowRotation && RotateCamera.HeldDown)
                        {
                            int x = mouseCoords.x;
                            int y = mouseCoords.y;
                            if (LockX.Down)
                            {
                                x = 0;
                            }
                            if (LockY.Down)
                            {
                                y = 0;
                            }
                            cameraMover.rotateFromMotion(x, y);
                            eventLayer.alertEventsHandled();
                        }
                    }
                    if (cameraMover.AllowZoom)
                    {
                        if (ZoomInCamera.Down)
                        {
                            cameraMover.incrementZoom(-1);
                            eventLayer.alertEventsHandled();
                        }
                        else if (ZoomOutCamera.Down)
                        {
                            cameraMover.incrementZoom(1);
                            eventLayer.alertEventsHandled();
                        }
                    }
                }
            }
        }
    }
}
