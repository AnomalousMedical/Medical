using BEPUik;
using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Platform;
using Medical.Pose;
using Medical.Pose.Commands;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    public class PoseController
    {
        private static ButtonEvent PickAnatomy;
        private static FingerDragGesture moveAnatomyGesture;

        static PoseController()
        {
            PickAnatomy = new ButtonEvent(EventLayers.Posing);
            PickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(PickAnatomy);

            moveAnatomyGesture = new FingerDragGesture(EventLayers.Posing, 1, 0.000001f, float.MaxValue, 1);
            DefaultEvents.registerDefaultEvent(moveAnatomyGesture);
        }

        private BEPUikScene ikScene;
        private SceneViewController sceneViewController;
        private AnatomyController anatomyController;
        private MusclePositionController musclePositionController;
        private ExternalDragControl dragControl = new ExternalDragControl();
        private PoseHandler currentHandler;
        private bool repinBone = false;
        private float hitDistance;
        private bool allowPosing = false;
        private TravelTracker travelTracker = new TravelTracker();
        private bool allowMousePosing = true;
        private MusclePosition poseStartPosition = null;
        private HashSet<String> activeModes = new HashSet<string>();

        public PoseController(StandaloneController controller)
        {
            activeModes.Add("Main");

            controller.SceneLoaded += controller_SceneLoaded;
            controller.SceneUnloading += controller_SceneUnloading;
            sceneViewController = controller.SceneViewController;
            anatomyController = controller.AnatomyController;
            musclePositionController = controller.MusclePositionController;

            if(controller.MedicalController.CurrentScene != null)
            {
                controller_SceneLoaded(controller.MedicalController.CurrentScene);
            }
        }

        public bool AllowPosing
        {
            get
            {
                return allowPosing;
            }
            set
            {
                if(allowPosing != value)
                {
                    allowPosing = value;
                    togglePicking();
                }
            }
        }

        void pickAnatomy_FirstFrameUpEvent(EventLayer eventLayer)
        {
            eventLayer.clearFocusLayer();
            PickAnatomy.OnHeldDown -= pickAnatomy_OnHeldDown;
            if (allowMousePosing)
            {
                clearDragTarget();
                if (eventLayer.EventProcessingAllowed && travelTracker.TraveledOverLimit)
                {
                    eventLayer.alertEventsHandled();
                }
            }
        }

        void pickAnatomy_FirstFrameDownEvent(EventLayer eventLayer)
        {
            if (allowMousePosing)
            {
                eventLayer.makeFocusLayer();
                travelTracker.reset();
                if (eventLayer.EventProcessingAllowed && !PickAnatomy.DownAndUpThisFrame)
                {
                    IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
                    Ray3 cameraRay = getCameraRay(absMouse.x, absMouse.y);
                    if (findDragTarget(cameraRay))
                    {
                        PickAnatomy.OnHeldDown += pickAnatomy_OnHeldDown;
                        eventLayer.alertEventsHandled();
                    }
                }
            }
        }

        void pickAnatomy_OnHeldDown(EventLayer eventLayer)
        {
            if (allowMousePosing && eventLayer.EventProcessingAllowed)
            {
                travelTracker.traveled(eventLayer.Mouse.RelativePosition);
                IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
                Ray3 cameraRay = getCameraRay(absMouse.x, absMouse.y);
                moveDragTarget(cameraRay);
                eventLayer.alertEventsHandled();
            }
        }

        void moveAnatomyGesture_GestureStarted(EventLayer eventLayer, FingerDragGesture gesture)
        {
            allowMousePosing = false;
            travelTracker.reset();
            if (eventLayer.EventProcessingAllowed)
            {
                Ray3 cameraRay = getCameraRay(gesture.AbsoluteX, gesture.AbsoluteY);
                if (findDragTarget(cameraRay))
                {
                    moveAnatomyGesture.Dragged += moveAnatomyGesture_Dragged;
                    moveAnatomyGesture.MomentumEnded += moveAnatomyGesture_MomentumEnded;
                    eventLayer.alertEventsHandled();
                }
            }
        }

        void moveAnatomyGesture_Dragged(EventLayer eventLayer, FingerDragGesture gesture)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                travelTracker.traveled((int)gesture.DeltaX, (int)gesture.DeltaY);
                Ray3 cameraRay = getCameraRay(gesture.AbsoluteX, gesture.AbsoluteY);
                moveDragTarget(cameraRay);
                eventLayer.alertEventsHandled();
            }
        }

        void moveAnatomyGesture_MomentumEnded(EventLayer eventLayer, FingerDragGesture gesture)
        {
            allowMousePosing = true;
            moveAnatomyGesture.Dragged -= moveAnatomyGesture_Dragged;
            moveAnatomyGesture.MomentumEnded -= moveAnatomyGesture_MomentumEnded;
            clearDragTarget();
            if (eventLayer.EventProcessingAllowed && travelTracker.TraveledOverLimit)
            {
                eventLayer.alertEventsHandled();
            }
        }

        private Ray3 getCameraRay(int x, int y)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if(activeWindow != null)
            {
                return activeWindow.getCameraToViewportRayScreen(x, y);
            }
            return new Ray3();
        }

        void controller_SceneLoaded(SimScene scene)
        {
            var simScene = scene.getDefaultSubScene().getSimElementManager<SimulationScene>();
            AllowPosing = simScene.AllowIK;

            ikScene = scene.getDefaultSubScene().getSimElementManager<BEPUikScene>();
            togglePicking();
        }

        void controller_SceneUnloading(SimScene scene)
        {
            ikScene = null;
            togglePicking();
        }

        void togglePicking()
        {
            anatomyController.setCommandPermission(AnatomyCommandPermissions.Posing, allowPosing);
            if (ikScene != null && allowPosing)
            {
                PickAnatomy.FirstFrameDownEvent += pickAnatomy_FirstFrameDownEvent;
                PickAnatomy.FirstFrameUpEvent += pickAnatomy_FirstFrameUpEvent;
                moveAnatomyGesture.GestureStarted += moveAnatomyGesture_GestureStarted;
            }
            else
            {
                PickAnatomy.FirstFrameDownEvent -= pickAnatomy_FirstFrameDownEvent;
                PickAnatomy.FirstFrameUpEvent -= pickAnatomy_FirstFrameUpEvent;
                moveAnatomyGesture.GestureStarted -= moveAnatomyGesture_GestureStarted;
            }
        }

        private bool findDragTarget(Ray3 cameraRay)
        {
            var matches = PoseableObjectsManager.findPoseable(cameraRay);
            foreach (var match in matches.Results)
            {
                var bone = match.PoseableIdentifier.PoseHandler.Bone;
                if (bone != null)
                {
                    if(bone.Pinned)
                    {
                        repinBone = true;
                        bone.Pinned = false;
                    }
                    else
                    {
                        repinBone = false;
                    }

                    dragControl.TargetBone = bone;
                    hitDistance = match.Distance;
                    Vector3 hitPosition = cameraRay.Direction * hitDistance + cameraRay.Origin;
                    dragControl.LinearMotor.Offset = (hitPosition - bone.Owner.Translation).toBepuVec3();
                    dragControl.LinearMotor.TargetPosition = hitPosition.toBepuVec3();
                    ikScene.addExternalControl(dragControl);
                    poseStartPosition = new MusclePosition(true);
                    currentHandler = match.PoseableIdentifier.PoseHandler;
                    currentHandler.posingStarted(activeModes);

                    return true;
                }
            }
            return false;
        }

        private void moveDragTarget(Ray3 cameraRay)
        {
            dragControl.LinearMotor.TargetPosition = (cameraRay.Direction * hitDistance + cameraRay.Origin).toBepuVec3();
        }

        private void clearDragTarget()
        {
            if (poseStartPosition != null)
            {
                //Only record undo if we moved far enough
                if (travelTracker.TraveledOverLimit)
                {
                    musclePositionController.pushUndoState(poseStartPosition);
                }
                poseStartPosition = null;
            }

            if (currentHandler != null)
            {
                currentHandler.posingEnded(activeModes);
                currentHandler = null;
            }

            if(dragControl.TargetBone != null)
            {
                dragControl.TargetBone.Pinned = repinBone;
            }

            ikScene.removeExternalControl(dragControl);
            dragControl.TargetBone = null;
        }
    }
}
