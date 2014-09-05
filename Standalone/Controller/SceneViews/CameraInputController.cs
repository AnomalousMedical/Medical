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
        private static MessageEvent rotateCamera;
        private static MessageEvent panCamera;
        private static MessageEvent zoomCamera;
        private static MessageEvent zoomInCamera;
        private static MessageEvent zoomOutCamera;

        static CameraInputController()
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
        private SceneViewController sceneViewController;

        public CameraInputController(SceneViewController sceneViewController, EventManager eventManager)
        {
            this.sceneViewController = sceneViewController;
            this.eventManager = eventManager;
            eventManager[EventLayers.Cameras].OnUpdate += processInputEvents;

            rotateCamera.FirstFrameDownEvent += rotateCamera_FirstFrameDownEvent;
            rotateCamera.FirstFrameUpEvent += rotateCamera_FirstFrameUpEvent;
        }

        public void Dispose()
        {
            eventManager[EventLayers.Cameras].OnUpdate -= processInputEvents;

            rotateCamera.FirstFrameDownEvent -= rotateCamera_FirstFrameDownEvent;
            rotateCamera.FirstFrameUpEvent -= rotateCamera_FirstFrameUpEvent;
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

                    if (eventLayer[CameraEvents.RotateCamera].FirstFrameDown)
                    {
                        currentlyInMotion = true;
                        eventLayer.alertEventsHandled();
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
                            cameraMover.panFromMotion(x, y, eventLayer.Mouse.AreaWidth, eventLayer.Mouse.AreaHeight);
                            eventLayer.alertEventsHandled();
                        }
                        else if (cameraMover.AllowZoom && eventLayer[CameraEvents.ZoomCamera].HeldDown)
                        {
                            cameraMover.zoomFromMotion(mouseCoords.y);
                            eventLayer.alertEventsHandled();
                        }
                        else if (cameraMover.AllowRotation && eventLayer[CameraEvents.RotateCamera].HeldDown)
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
                            cameraMover.rotateFromMotion(x, y);
                            eventLayer.alertEventsHandled();
                        }
                    }
                    if (cameraMover.AllowZoom)
                    {
                        if (eventLayer[CameraEvents.ZoomInCamera].Down)
                        {
                            cameraMover.incrementZoom(-1);
                            eventLayer.alertEventsHandled();
                        }
                        else if (eventLayer[CameraEvents.ZoomOutCamera].Down)
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
