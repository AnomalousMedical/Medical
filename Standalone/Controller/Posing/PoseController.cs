using BEPUik;
using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Platform;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Controller
{
    class PoseController
    {
        private static ButtonEvent PickAnatomy;

        static PoseController()
        {
            PickAnatomy = new ButtonEvent(EventLayers.Posing);
            PickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(PickAnatomy);
        }

        private BEPUikScene ikScene;
        private SceneViewController sceneViewController;
        private AnatomyController anatomyController;
        private ExternalDragControl dragControl = new ExternalDragControl();
        private float hitDistance;
        private bool allowPosing = false;
        private MouseTravelTracker travelTracker = new MouseTravelTracker();

        public PoseController(StandaloneController controller)
        {
            controller.SceneLoaded += controller_SceneLoaded;
            controller.SceneUnloading += controller_SceneUnloading;
            sceneViewController = controller.SceneViewController;
            anatomyController = controller.AnatomyController;
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
                    anatomyController.setCommandPermission(AnatomyCommandPermissions.Posing, allowPosing);
                    togglePicking();
                }
            }
        }

        void pickAnatomy_FirstFrameUpEvent(EventLayer eventLayer)
        {
            PickAnatomy.OnHeldDown -= pickAnatomy_OnHeldDown;
            ikScene.removeExternalControl(dragControl);
            dragControl.TargetBone = null;
            if (eventLayer.EventProcessingAllowed && travelTracker.TraveledOverLimit)
            {
                eventLayer.alertEventsHandled();
            }
        }

        void pickAnatomy_FirstFrameDownEvent(EventLayer eventLayer)
        {
            travelTracker.reset();
            if (eventLayer.EventProcessingAllowed)
            {
                Ray3 cameraRay = getCameraRay(eventLayer);
                var matches = PoseableObjectsManager.findPoseable(cameraRay);
                foreach (var match in matches.Results)
                {
                    var bone = match.PoseableIdentifier.Bone;
                    if (bone != null && !bone.Pinned)
                    {
                        dragControl.TargetBone = bone;
                        hitDistance = match.Distance;
                        Vector3 hitPosition = cameraRay.Direction * hitDistance + cameraRay.Origin;
                        dragControl.LinearMotor.Offset = (hitPosition - bone.Owner.Translation).toBepuVec3();
                        dragControl.LinearMotor.TargetPosition = hitPosition.toBepuVec3();
                        PickAnatomy.OnHeldDown += pickAnatomy_OnHeldDown;
                        ikScene.addExternalControl(dragControl);
                        eventLayer.alertEventsHandled();
                        break;
                    }
                }
            }
        }

        void pickAnatomy_OnHeldDown(EventLayer eventLayer)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                travelTracker.traveled(eventLayer.Mouse.RelativePosition);
                Ray3 cameraRay = getCameraRay(eventLayer);
                dragControl.LinearMotor.TargetPosition = (cameraRay.Direction * hitDistance + cameraRay.Origin).toBepuVec3();
                eventLayer.alertEventsHandled();
            }
        }

        private Ray3 getCameraRay(EventLayer eventLayer)
        {
            IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            if(activeWindow != null)
            {
                return activeWindow.getCameraToViewportRayScreen(absMouse.x, absMouse.y);
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
            if (ikScene != null && allowPosing)
            {
                PickAnatomy.FirstFrameDownEvent += pickAnatomy_FirstFrameDownEvent;
                PickAnatomy.FirstFrameUpEvent += pickAnatomy_FirstFrameUpEvent;
            }
            else
            {
                PickAnatomy.FirstFrameDownEvent -= pickAnatomy_FirstFrameDownEvent;
                PickAnatomy.FirstFrameUpEvent -= pickAnatomy_FirstFrameUpEvent;
            }
        }
    }
}
