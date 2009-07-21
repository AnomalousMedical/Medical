using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;

namespace Medical
{
    public class PredefinedCamera : Interface
    {
        [Editable]
        private String lookAtSimObjectName;

        [Editable]
        private String cameraName;

        private SimObject lookAtSimObject;

        protected override void constructed()
        {
            lookAtSimObject = Owner.getOtherSimObject(lookAtSimObjectName);
            if (lookAtSimObject == null)
            {
                blacklist("Cannot find look at named {0}.", lookAtSimObjectName);
            }
            else
            {
                PredefinedCameraController.add(this);
            }
        }

        protected override void destroy()
        {
            PredefinedCameraController.remove(this);
        }

        public Vector3 EyePoint
        {
            get
            {
                return Owner.Translation;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return lookAtSimObject.Translation;
            }
        }

        public String CameraName
        {
            get
            {
                return cameraName;
            }
        }
    }
}
