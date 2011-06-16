using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;
using Engine.Saving;
using Engine;

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
        }

        public CameraPosition()
        {

        }

        [Editable]
        public Vector3 Translation { get; set; }

        [Editable]
        public Vector3 LookAt { get; set; }

        private EditInterface editInterface;

        public EditInterface getEditInterface(string memberName, MemberScanner scanner)
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, scanner, memberName, null);
                editInterface.addCommand(new EditInterfaceCommand("Capture Camera Position", captureCameraPosition));
            }
            return editInterface;
        }

        private void captureCameraPosition(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery(CustomEditQueries.CaptureCameraPosition, null, this);
        }

        #region Saveable Members

        protected CameraPosition(LoadInfo info)
        {
            Translation = info.GetVector3("Translation", Translation);
            LookAt = info.GetVector3("LookAt", LookAt);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Translation", Translation);
            info.AddValue("LookAt", LookAt);
        }

        #endregion
    }
}
