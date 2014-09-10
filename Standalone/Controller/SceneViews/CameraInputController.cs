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
        enum Gesture : int
        {
            None = 0,
            Rotate = 1,
            Zoom = 2,
            Pan = 3
        }

        private static MouseButtonCode currentMouseButton = MedicalConfig.CameraMouseButton;
        private static ButtonEvent RotateCamera;
        private static ButtonEvent PanCamera;
        private static ButtonEvent ZoomCamera;
        private static ButtonEvent ZoomInCamera;
        private static ButtonEvent ZoomOutCamera;
        private static ButtonEvent LockX;
        private static ButtonEvent LockY;

        private static FingerDragGesture rotateGesture;
        private static FingerDragGesture panGesture;
        private static FingerDragGesture zoomGesture;

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

            switch(PlatformConfig.TouchType)
            { 
                case TouchType.Screen:
                    zoomGesture = new FingerDragGesture(EventLayers.Cameras, 2, 0.5f, 0.01f);
                    DefaultEvents.registerDefaultEvent(zoomGesture);

                    rotateGesture = new FingerDragGesture(EventLayers.Cameras, 1, 0.5f, 0.01f);
                    DefaultEvents.registerDefaultEvent(rotateGesture);

                    panGesture = new FingerDragGesture(EventLayers.Cameras, 3, 0.5f, 0.01f); ;
                    DefaultEvents.registerDefaultEvent(panGesture);
                break;
            }
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
        private Gesture currentGesture = Gesture.None;

        public CameraInputController(SceneViewController sceneViewController, EventManager eventManager)
        {
            this.sceneViewController = sceneViewController;
            this.eventManager = eventManager;
            eventManager[EventLayers.Cameras].OnUpdate += processInputEvents;

            RotateCamera.FirstFrameDownEvent += rotateCamera_FirstFrameDownEvent;
            RotateCamera.FirstFrameUpEvent += rotateCamera_FirstFrameUpEvent;

            if (zoomGesture != null)
            {
                zoomGesture.Dragged += zoomGesture_Dragged;
                zoomGesture.MomentumStarted += zoomGesture_MomentumStarted;
                zoomGesture.MomentumEnded += zoomGesture_MomentumEnded;
                rotateGesture.Dragged += rotateGesture_Dragged;
                rotateGesture.MomentumStarted += rotateGesture_MomentumStarted;
                rotateGesture.MomentumEnded += rotateGesture_MomentumEnded;
                panGesture.Dragged += panGesture_Dragged;
                panGesture.MomentumStarted += panGesture_MomentumStarted;
                panGesture.MomentumEnded += panGesture_MomentumEnded;
            }
        }

        public void Dispose()
        {
            eventManager[EventLayers.Cameras].OnUpdate -= processInputEvents;

            RotateCamera.FirstFrameDownEvent -= rotateCamera_FirstFrameDownEvent;
            RotateCamera.FirstFrameUpEvent -= rotateCamera_FirstFrameUpEvent;

            if (zoomGesture != null)
            {
                zoomGesture.Dragged -= zoomGesture_Dragged;
                zoomGesture.MomentumStarted -= zoomGesture_MomentumStarted;
                zoomGesture.MomentumEnded -= zoomGesture_MomentumEnded;
                rotateGesture.Dragged -= rotateGesture_Dragged;
                rotateGesture.MomentumStarted -= rotateGesture_MomentumStarted;
                rotateGesture.MomentumEnded -= rotateGesture_MomentumEnded;
                panGesture.Dragged -= panGesture_Dragged;
                panGesture.MomentumStarted -= panGesture_MomentumStarted;
                panGesture.MomentumEnded -= panGesture_MomentumEnded;
            }
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

        void rotateGesture_Dragged(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Rotate)
            {
                currentGesture = Gesture.Rotate;
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 4.0f;
                    sceneView.CameraMover.rotate(-gesture.DeltaX * sensitivity, gesture.DeltaY * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }

        void rotateGesture_MomentumStarted(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Rotate)
            {
                eventLayer.alertEventsHandled();
            }
        }

        void rotateGesture_MomentumEnded(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (currentGesture <= Gesture.Rotate)
            {
                currentGesture = Gesture.None;
            }
        }

        void panGesture_Dragged(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Pan)
            {
                currentGesture = Gesture.Pan;
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 15.0f;
                    sceneView.CameraMover.pan(gesture.DeltaX * sensitivity, gesture.DeltaY * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }

        void panGesture_MomentumStarted(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Pan)
            {
                eventLayer.alertEventsHandled();
            }
        }

        void panGesture_MomentumEnded(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (currentGesture <= Gesture.Pan)
            {
                currentGesture = Gesture.None;
            }
        }

        void zoomGesture_Dragged(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Zoom)
            {
                currentGesture = Gesture.Zoom;
                SceneViewWindow sceneView = sceneViewController.ActiveWindow;
                if (sceneView != null)
                {
                    float sensitivity = 80.0f;
                    sceneView.CameraMover.zoom(gesture.DeltaY * sensitivity);
                    sceneView.CameraMover.stopMaintainingIncludePoint();
                }
                eventLayer.alertEventsHandled();
            }
        }

        void zoomGesture_MomentumStarted(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed && currentGesture <= Gesture.Zoom)
            {
                eventLayer.alertEventsHandled();
            }
        }

        void zoomGesture_MomentumEnded(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (currentGesture <= Gesture.Zoom)
            {
                currentGesture = Gesture.None;
            }
        }

        void Touches_AllFingersReleased(Touches obj)
        {
            currentGesture = Gesture.None;
        }
    }
}
