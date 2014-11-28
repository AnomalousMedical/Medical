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
    class CameraAngleGui : PinableMDIDialog
    {
        private const float HALF_PI = (float)Math.PI / 2.0f - 0.001f;

        private SceneViewController sceneViewController;
        private AnatomyController anatomyController;
        private SceneViewWindow activeWindow = null;

        private Button undoItem;
        private Button redoItem;


        public CameraAngleGui(SceneViewController sceneViewController, AnatomyController anatomyController)
            :base("Medical.GUI.CameraAngleGui.CameraAngleGui.layout")
        {
            this.sceneViewController = sceneViewController;
            this.anatomyController = anatomyController;

            Button button = window.findWidget("Front") as Button;
            button.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));

            button = window.findWidget("Back") as Button;
            button.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f));

            button = window.findWidget("Left") as Button;
            button.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f));

            button = window.findWidget("Right") as Button;
            button.MouseButtonClick += (s, e) => setStraightAngleView(new Vector3(0.0f, 1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f));

            button = window.findWidget("Top") as Button;
            button.MouseButtonClick += (s, e) => setTopBottomView(1);

            button = window.findWidget("Bottom") as Button;
            button.MouseButtonClick += (s, e) => setTopBottomView(-1);

            button = window.findWidget("CenterVisible") as Button;
            button.MouseButtonClick += (s, e) => showAllVisibleAnatomy();

            undoItem = window.findWidget("Undo") as Button;
            undoItem.MouseButtonClick += (s, e) => undo();

            redoItem = window.findWidget("Redo") as Button;
            redoItem.MouseButtonClick += (s, e) => redo();

            sceneViewController.ActiveWindowChanged += sceneViewController_ActiveWindowChanged;
            sceneViewController_ActiveWindowChanged(sceneViewController.ActiveWindow);
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

                CameraPosition undoPosition = activeWindow.createCameraPosition();
                activeWindow.setPosition(new CameraPosition()
                {
                    Translation = newTrans,
                    LookAt = lookAt,
                }, MedicalConfig.CameraTransitionTime);
                activeWindow.pushUndoState(undoPosition);
            }
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

                    CameraPosition undoPosition = activeWindow.createCameraPosition();
                    activeWindow.setPosition(new CameraPosition()
                    {
                        Translation = newTrans,
                        LookAt = lookAt,
                    }, MedicalConfig.CameraTransitionTime);
                    activeWindow.pushUndoState(undoPosition);
                }
            }
        }

        void showAllVisibleAnatomy()
        {
            AxisAlignedBox boundingBox = anatomyController.VisibleObjectsBoundingBox;
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            Vector3 center = boundingBox.Center;

            float nearPlane = activeWindow.Camera.getNearClipDistance();
            float theta = activeWindow.Camera.getFOVy();
            float aspectRatio = activeWindow.Camera.getAspectRatio();
            if (aspectRatio < 1.0f)
            {
                theta *= aspectRatio;
            }

            Vector3 translation = center;
            Vector3 direction = (activeWindow.Translation - activeWindow.LookAt).normalized();
            float diagonalDistance = boundingBox.DiagonalDistance;
            if (diagonalDistance > float.Epsilon)
            {
                translation += direction * diagonalDistance / (float)Math.Tan(theta);

                CameraPosition undoPosition = activeWindow.createCameraPosition();
                activeWindow.setPosition(new CameraPosition()
                {
                    Translation = translation,
                    LookAt = center
                }, MedicalConfig.CameraTransitionTime);
                activeWindow.pushUndoState(undoPosition);
            }
        }

        void undo()
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            activeWindow.undo();
        }

        void redo()
        {
            SceneViewWindow activeWindow = sceneViewController.ActiveWindow;
            activeWindow.redo();
        }

        void sceneViewController_ActiveWindowChanged(SceneViewWindow window)
        {
            if (activeWindow != null)
            {
                activeWindow.OnUndoRedoChanged -= setupUndoRedo;
                activeWindow.OnRedo -= setupUndoRedo;
                activeWindow.OnUndo -= setupUndoRedo;
            }
            this.activeWindow = window;
            if (activeWindow != null)
            {
                activeWindow.OnUndoRedoChanged += setupUndoRedo;
                activeWindow.OnRedo += setupUndoRedo;
                activeWindow.OnUndo += setupUndoRedo;
                setupUndoRedo(activeWindow);
            }
        }

        void setupUndoRedo(SceneViewWindow win)
        {
            redoItem.Enabled = win.HasRedo;
            undoItem.Enabled = win.HasUndo;
        }
    }
}
