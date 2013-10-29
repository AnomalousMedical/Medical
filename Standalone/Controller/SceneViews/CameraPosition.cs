using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;
using Engine.Saving;
using Engine;
using Medical.Controller;

namespace Medical
{
    /// <summary>
    /// This class provides an easy way to get a camera position in an editable
    /// class. It contains the properties for where the camera goes and provides
    /// saving and editing provisions.
    /// </summary>
    public class CameraPosition : EditInterfaceOverride, Saveable
    {
        public enum CustomEditQueries
        {
            /// <summary>
            /// This query will request the current camera position. This will
            /// pass this CameraPosition class in argument slot 0.
            /// </summary>
            CaptureCameraPosition,
            /// <summary>
            /// This query will preview the current camera position. This will
            /// pass this CameraPosition class in argument slot 0.
            /// </summary>
            PreviewCameraPosition
        }

        public CameraPosition()
        {
            Easing = EasingFunction.EaseOutQuadratic;
        }

        [Editable]
        public Vector3 Translation { get; set; }

        [Editable]
        public Vector3 LookAt { get; set; }

        [Editable]
        public Vector3 IncludePoint { get; set; }

        [Editable]
        public bool UseIncludePoint { get; set; }

        [Editable]
        public EasingFunction Easing { get; set; }

        public void calculateIncludePoint(SceneViewWindow sceneWindow)
        {
            //Make the include point projected out to the lookat location
            Ray3 camRay = sceneWindow.getCameraToViewportRay(1, 0);
            IncludePoint = camRay.Origin + camRay.Direction * (LookAt - Translation).length();
            UseIncludePoint = true;
        }

        public Vector3 computeTranslationWithIncludePoint(SceneViewWindow sceneWindow)
        {
            if (UseIncludePoint && IncludePoint.isNumber())
            {
                float aspect = sceneWindow.Camera.getAspectRatio();
                float fovy = sceneWindow.Camera.getFOVy() * 0.5f;

                Vector3 direction = LookAt - Translation;

                //Figure out direction, must use ogre fixed yaw calculation, first adjust direction to face -z
                Vector3 zAdjustVec = -direction;
                zAdjustVec.normalize();
                Quaternion targetWorldOrientation = Quaternion.shortestArcQuatFixedYaw(ref zAdjustVec);

                Matrix4x4 viewMatrix = Matrix4x4.makeViewMatrix(Translation, targetWorldOrientation);
                Matrix4x4 projectionMatrix = sceneWindow.Camera.getProjectionMatrix();
                float offset = SceneViewWindow.computeOffsetToIncludePoint(viewMatrix, projectionMatrix, IncludePoint, aspect, fovy);

                direction.normalize();
                Vector3 newTrans = Translation + offset * direction;
                return newTrans;
            }

            return Translation;
        }

        private EditInterface editInterface;

        public EditInterface getEditInterface(string memberName, MemberScanner scanner)
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, scanner, memberName, null);
                editInterface.addCommand(new EditInterfaceCommand("Capture Camera Position", captureCameraPosition));
                editInterface.addCommand(new EditInterfaceCommand("Preview", delegate(EditUICallback callback, EditInterfaceCommand caller)
                {
                    callback.runOneWayCustomQuery(CustomEditQueries.PreviewCameraPosition, this);
                }));
            }
            return editInterface;
        }

        private void captureCameraPosition(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runOneWayCustomQuery(CustomEditQueries.CaptureCameraPosition, this);
            editInterface.fireDataNeedsRefresh();
        }

        #region Saveable Members

        protected CameraPosition(LoadInfo info)
        {
            Translation = info.GetVector3("Translation", Translation);
            LookAt = info.GetVector3("LookAt", LookAt);
            IncludePoint = info.GetVector3("IncludePoint", IncludePoint);
            UseIncludePoint = info.GetBoolean("UseIncludePoint", false);
            Easing = info.GetValue("Easing", EasingFunction.EaseOutQuadratic);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Translation", Translation);
            info.AddValue("LookAt", LookAt);
            info.AddValue("IncludePoint", IncludePoint);
            info.AddValue("UseIncludePoint", UseIncludePoint);
            info.AddValue("Easing", Easing);
        }

        #endregion
    }
}
