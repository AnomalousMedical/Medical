using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    public class Disc : Interface
    {
        [Editable]
        private Vector3 normalOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 offset = Vector3.UnitY * -0.302f;

        [Editable]
        String controlPointObject;

        [Editable]
        String controlPointBehavior;

        private ControlPointBehavior controlPoint;

        protected override void constructed()
        {
            DiscController.addDisc(this);
            SimObject controlPointObj = Owner.getOtherSimObject(controlPointObject);
            if (controlPointObj != null)
            {
                controlPoint = controlPointObj.getElement(controlPointBehavior) as ControlPointBehavior;
                if (controlPoint == null)
                {
                    blacklist("Could not find controlPointBehavior {0}.", controlPointBehavior); 
                }
            }
            else
            {
                blacklist("Could not find controlPointObject {0}.", controlPointObject);
            }
        }

        protected override void destroy()
        {
            DiscController.removeDisc(this);
        }

        public Vector3 getOffset(float position)
        {
            return offset;
        }

        public void setOffset(Vector3 offset)
        {
            this.offset = offset;
            if (controlPoint != null)
            {
                controlPoint.positionModified();
            }
        }

        public Vector3 getNormalOffset()
        {
            return normalOffset;
        }
    }
}
