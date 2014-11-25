using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class SetCameraAngleTask : Task, IDisposable
    {
        private const float HALF_PI = (float)Math.PI / 2.0f - 0.001f;

        private PopupMenu cameraAngleMenu;
        private SceneViewController sceneViewController;
        private AnatomyController anatomyController;

        public SetCameraAngleTask(SceneViewController sceneViewController, AnatomyController anatomyController)
            :base("Medical.SetCameraAngle", "Set Camera Angle", CommonResources.NoIcon, "Navigation")
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;

            cameraAngleMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            cameraAngleMenu.Visible = false;

            MenuItem sequenceItem = cameraAngleMenu.addItem("Front", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));

            sequenceItem = cameraAngleMenu.addItem("Back", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f));

            sequenceItem = cameraAngleMenu.addItem("Left", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f));

            sequenceItem = cameraAngleMenu.addItem("Right", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(0.0f, 1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f));

            sequenceItem = cameraAngleMenu.addItem("Top", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setTopBottomView(1);

            sequenceItem = cameraAngleMenu.addItem("Bottom", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => setTopBottomView(-1);

            sequenceItem = cameraAngleMenu.addItem("Center Visible Anatomy", MenuItemType.Normal);
            sequenceItem.MouseButtonClick += (s, e) => showAllVisibleAnatomy();
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(cameraAngleMenu);
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            cameraAngleMenu.setVisibleSmooth(true);
            LayerManager.Instance.upLayerItem(cameraAngleMenu);
            IntVector2 loc = taskPositioner.findGoodWindowPosition(cameraAngleMenu.Width, cameraAngleMenu.Height);
            cameraAngleMenu.setPosition(loc.x, loc.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        void setStraightAngleView(Vector3 lookAtMask, Vector3 translationMask)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;

            if (activeWindow != null)
            {
                Vector3 lookAt = activeWindow.LookAt;
                float length = (activeWindow.Translation - lookAt).length();
                Vector3 newTrans = new Vector3(lookAt.x * lookAtMask.x + length * translationMask.x,
                                               lookAt.y * lookAtMask.y + length * translationMask.y,
                                               lookAt.z * lookAtMask.z + length * translationMask.z);

                activeWindow.setPosition(new CameraPosition()
                {
                    Translation = newTrans,
                    LookAt = lookAt,
                }, MedicalConfig.CameraTransitionTime);
            }

            cameraAngleMenu.setVisibleSmooth(false);
        }

        void setTopBottomView(int direction)
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;

            if (activeWindow != null)
            {
                Vector3 lookAt = activeWindow.LookAt;

                Vector3 transMinusLookAt = activeWindow.Translation - lookAt;
                float length = (transMinusLookAt).length();
                transMinusLookAt.y = 0;

                //Compute the yaw.
                float localY = transMinusLookAt.y;
                transMinusLookAt.y = 0;
                float yaw = Vector3.Backward.angle(ref transMinusLookAt);
                if (!float.IsNaN(yaw))
                {
                    if (transMinusLookAt.x < 0)
                    {
                        yaw = -yaw;
                    }

                    Quaternion yawRot = new Quaternion(Vector3.Up, yaw);
                    Quaternion pitchRot = new Quaternion(Vector3.Left, HALF_PI * direction);

                    Quaternion rotation = yawRot * pitchRot;
                    Vector3 normalDirection = Quaternion.quatRotate(ref rotation, ref Vector3.Backward);
                    Vector3 newTrans = normalDirection * length + lookAt;

                    activeWindow.setPosition(new CameraPosition()
                    {
                        Translation = newTrans,
                        LookAt = lookAt,
                    }, MedicalConfig.CameraTransitionTime);
                }
            }

            cameraAngleMenu.setVisibleSmooth(false);
        }

        void showAllVisibleAnatomy()
        {
            AxisAlignedBox boundingBox = anatomyController.VisibleObjectsBoundingBox;
            SceneViewWindow window = sceneViewController.ActiveWindow;
            Vector3 center = boundingBox.Center;

            float nearPlane = window.Camera.getNearClipDistance();
            float theta = window.Camera.getFOVy();
            float aspectRatio = window.Camera.getAspectRatio();
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            Vector3 translation = center;
            Vector3 direction = (window.Translation - window.LookAt).normalized();
            float diagonalDistance = boundingBox.DiagonalDistance;
            if (diagonalDistance > float.Epsilon)
            {
                translation += direction * diagonalDistance / (float)Math.Tan(theta);
                CameraPosition cameraPosition = new CameraPosition()
                {
                    Translation = translation,
                    LookAt = center
                };

                window.setPosition(cameraPosition, MedicalConfig.CameraTransitionTime);
            }
        }
    }
}
