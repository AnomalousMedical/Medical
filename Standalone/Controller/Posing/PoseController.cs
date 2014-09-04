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
        enum Events
        {
            SelectAndMove,
        }

        private static MessageEvent pickAnatomy;

        static PoseController()
        {
            pickAnatomy = new MessageEvent(Events.SelectAndMove, EventLayers.Posing);
            pickAnatomy.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(pickAnatomy);
        }

        private BEPUikScene ikScene;
        private SceneViewController sceneViewController;
        private AnatomyController anatomyController;
        private ExternalDragControl dragControl = new ExternalDragControl();
        private float hitDistance;
        private bool allowPosing = false;

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
            pickAnatomy.OnHeldDown -= pickAnatomy_OnHeldDown;
            ikScene.removeExternalControl(dragControl);
            dragControl.TargetBone = null;
        }

        void pickAnatomy_FirstFrameDownEvent(EventLayer eventLayer)
        {
            Ray3 cameraRay;
            if (getCameraRay(eventLayer, out cameraRay))
            {
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
                        pickAnatomy.OnHeldDown += pickAnatomy_OnHeldDown;
                        ikScene.addExternalControl(dragControl);
                        eventLayer.alertEventsHandled();
                        break;
                    }
                }
            }
        }

        void pickAnatomy_OnHeldDown(EventLayer eventLayer)
        {
            Ray3 cameraRay;
            if (getCameraRay(eventLayer, out cameraRay))
            {
                dragControl.LinearMotor.TargetPosition = (cameraRay.Direction * hitDistance + cameraRay.Origin).toBepuVec3();
            }
            eventLayer.alertEventsHandled();
        }

        private bool getCameraRay(EventLayer eventLayer, out Ray3 cameraRay)
        {
            IntVector3 absMouse = eventLayer.Mouse.AbsolutePosition;
            if (!Gui.Instance.HandledMouse && !InputManager.Instance.isModalAny())
            {
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                cameraRay = activeWindow.getCameraToViewportRayScreen(absMouse.x, absMouse.y);
                return true;
            }
            cameraRay = new Ray3();
            return false;
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
                pickAnatomy.FirstFrameDownEvent += pickAnatomy_FirstFrameDownEvent;
                pickAnatomy.FirstFrameUpEvent += pickAnatomy_FirstFrameUpEvent;
            }
            else
            {
                pickAnatomy.FirstFrameDownEvent -= pickAnatomy_FirstFrameDownEvent;
                pickAnatomy.FirstFrameUpEvent -= pickAnatomy_FirstFrameUpEvent;
            }
        }
    }
}
