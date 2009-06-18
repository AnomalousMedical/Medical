using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public delegate void RotationChanged(Quaternion newRotation, Object sender);

    public class RotateController
    {
        public event RotationChanged OnRotationChanged;

        private Quaternion currentRotation;
        private bool dispatching = false;

        public RotateController()
        {

        }

        public void setRotation(ref Quaternion rotation, Object sender)
        {
            if (!dispatching)
            {
                dispatching = true;
                currentRotation = rotation;
                if (OnRotationChanged != null)
                {
                    OnRotationChanged.Invoke(rotation, sender);
                }
                dispatching = false;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return currentRotation;
            }
        }
    }
}
