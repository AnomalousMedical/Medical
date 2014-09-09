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

        private static MultiFingerScrollGesture rotateGesture;
        private static MultiFingerScrollGesture panGesture;
        private static TwoFingerZoom zoomGesture;

        static CameraInputController()
        {
            RotateCamera = new ButtonEvent(EventLayers.Cameras);
            RotateCamera.addButton(currentMouseButton);
            DefaultEvents.registerDefaultEvent(RotateCamera);

            PanCamera = new ButtonEvent(EventLayers.Cameras);
            PanCamera.addButton(currentMouseButton);
            PanCamera.addButton(PlatformConfig.PanKey);
            DefaultEvents.registerDefaultEvent(PanCamera);

            ZoomCamera = new ButtonEvent(EventLayers.Cameras);
            ZoomCamera.addButton(currentMouseButton);
            ZoomCamera.addButton(KeyboardButtonCode.KC_LMENU);
            DefaultEvents.registerDefaultEvent(ZoomCamera);

            ZoomInCamera = new ButtonEvent(EventLayers.Cameras);
            ZoomInCamera.MouseWheelDirection = MouseWheelDirection.Up;
            DefaultEvents.registerDefaultEvent(ZoomInCamera);

            ZoomOutCamera = new ButtonEvent(EventLayers.Cameras);
            ZoomOutCamera.MouseWheelDirection = MouseWheelDirection.Down;
            DefaultEvents.registerDefaultEvent(ZoomOutCamera);

            LockX = new ButtonEvent(EventLayers.Cameras);
            LockX.addButton(KeyboardButtonCode.KC_C);
            DefaultEvents.registerDefaultEvent(LockX);

            LockY = new ButtonEvent(EventLayers.Cameras);
            LockY.addButton(KeyboardButtonCode.KC_X);
            DefaultEvents.registerDefaultEvent(LockY);

            zoomGesture = PlatformConfig.createZoomGesture();
            DefaultEvents.registerDefaultEvent(zoomGesture);

            rotateGesture = PlatformConfig.createRotateGesture();
            DefaultEvents.registerDefaultEvent(rotateGesture);

            panGesture = PlatformConfig.createPanGesture();
            DefaultEvents.registerDefaultEvent(panGesture);
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

            zoomGesture.Zoom += zoomGesture_Zoom;
            rotateGesture.Scroll += rotateGesture_Scroll;
            panGesture.Scroll += panGesture_Scroll;
        }

        public void Dispose()
        {
            eventManager[EventLayers.Cameras].OnUpdate -= processInputEvents;

            RotateCamera.FirstFrameDownEvent -= rotateCamera_FirstFrameDownEvent;
            RotateCamera.FirstFrameUpEvent -= rotateCamera_FirstFrameUpEvent;

            zoomGesture.Zoom -= zoomGesture_Zoom;
            rotateGesture.Scroll -= rotateGesture_Scroll;
            panGesture.Scroll -= panGesture_Scroll;
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
                        if (PanCamera.HeldDown)
                        {
                            travelTracker.traveled(mouseCoords);
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
                            travelTracker.traveled(mouseCoords);
                            cameraMover.zoomFromMotion(mouseCoords.y);
                            eventLayer.alertEventsHandled();
                        }
                        else if (cameraMover.AllowRotation && RotateCamera.HeldDown)
                        {
                            travelTracker.traveled(mouseCoords);
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

        void rotateGesture_Scroll(EventLayer eventLayer, float deltaX, float deltaY)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 4.0f;
                    sceneView.CameraMover.rotate(-deltaX * sensitivity, deltaY * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }

        void panGesture_Scroll(EventLayer eventLayer, float deltaX, float deltaY)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 15.0f;
                    sceneView.CameraMover.pan(deltaX * sensitivity, deltaY * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }

        void zoomGesture_Zoom(EventLayer eventLayer, float zoomDelta)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 80.0f;
                    sceneView.CameraMover.zoom(zoomDelta * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }
    }
}
