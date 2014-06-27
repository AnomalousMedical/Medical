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
            Easing = EasingFunction.EaseInOutQuadratic;
        }

        public CameraPosition(CameraPosition clone)
        {
            Translation = clone.Translation;
            LookAt = clone.LookAt;
            IncludePoint = clone.IncludePoint;
            UseIncludePoint = clone.UseIncludePoint;
            Easing = clone.Easing;
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
            Easing = info.GetValue("Easing", EasingFunction.EaseInOutQuadratic);
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
