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
            pickAnatomy = new MessageEvent(Events.SelectAndMove);
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

        void pickAnatomy_FirstFrameUpEvent(EventManager eventManager)
        {
            pickAnatomy.DownContinues -= pickAnatomy_DownContinues;
            ikScene.removeExternalControl(dragControl);
        }

        void pickAnatomy_FirstFrameDownEvent(EventManager eventManager)
        {
            Ray3 cameraRay;
            if (getCameraRay(eventManager, out cameraRay))
            {
                var matches = PoseableObjectsManager.findAnatomy(cameraRay);
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
                        pickAnatomy.DownContinues += pickAnatomy_DownContinues;
                        ikScene.addExternalControl(dragControl);
                        break;
                    }
                }
            }
        }

        void pickAnatomy_DownContinues(EventManager eventManager)
        {
            Ray3 cameraRay;
            if (getCameraRay(eventManager, out cameraRay))
            {
                dragControl.LinearMotor.TargetPosition = (cameraRay.Direction * hitDistance + cameraRay.Origin).toBepuVec3();
            }
        }

        private bool getCameraRay(EventManager eventManager, out Ray3 cameraRay)
        {
            Vector3 absMouse = eventManager.Mouse.getAbsMouse();
            if (!Gui.Instance.HandledMouseButtons && !InputManager.Instance.isModalAny())
            {
                SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
                Vector2 windowLoc = new Vector2(activeWindow.RenderXLoc, activeWindow.RenderYLoc);
                Size2 windowSize = new Size2(activeWindow.RenderWidth, activeWindow.RenderHeight);
                absMouse.x = (absMouse.x - windowLoc.x) / windowSize.Width;
                absMouse.y = (absMouse.y - windowLoc.y) / windowSize.Height;
                cameraRay = activeWindow.getCameraToViewportRay(absMouse.x, absMouse.y);
                return true;
            }
            cameraRay = new Ray3();
            return false;
        }

        void controller_SceneLoaded(SimScene scene)
        {
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
