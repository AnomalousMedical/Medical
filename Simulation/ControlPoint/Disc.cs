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
        private Vector3 normalDiscOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 discOffset = Vector3.UnitY * -0.302f;

        [Editable]
        private Vector3 normalRDAOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 rdaOffset = Vector3.UnitY * -0.151f;

        [Editable]
        private Vector3 horizontalTickSpacing = Vector3.UnitX * 0.1f;

        [Editable]
        private Vector3 horizontalOffset = Vector3.Zero;

        [Editable]
        private float discPopLocation = 0.0f;

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

        public Vector3 getOffset(float location)
        {
            if (location < discPopLocation)
            {
                return rdaOffset + horizontalOffset;
            }
            else
            {
                return discOffset + horizontalOffset;
            }
        }

        public Vector3 HorizontalTickSpacing
        {
            get
            {
                return horizontalTickSpacing;
            }
        }

        public Vector3 DiscOffset
        {
            get
            {
                return discOffset;
            }
            set
            {
                discOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 NormalDiscOffset
        {
            get
            {
                return normalDiscOffset;
            }
        }

        public Vector3 RDAOffset
        {
            get
            {
                return rdaOffset;
            }
            set
            {
                rdaOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 NormalRDAOffset
        {
            get
            {
                return normalRDAOffset;
            }
        }

        public float PopLocation
        {
            get
            {
                return discPopLocation;
            }
            set
            {
                discPopLocation = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }

        public Vector3 HorizontalOffset
        {
            get
            {
                return horizontalOffset;
            }
            set
            {
                horizontalOffset = value;
                if (controlPoint != null)
                {
                    controlPoint.positionModified();
                }
            }
        }
    }
}
