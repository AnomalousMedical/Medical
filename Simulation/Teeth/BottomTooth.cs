using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Logging;
using BulletPlugin;
using Engine.ObjectManagement;

namespace Medical
{
    class BottomTooth : Tooth
    {
        protected override void looseChanged(bool loose)
        {
            
        }

        protected override void constructed()
        {
            base.constructed();
            if (actorElement != null)
            {
                actorElement.ContactStarted += actorElement_ContactStarted;
                actorElement.ContactEnded += actorElement_ContactEnded;
            }
        }

        protected override void destroy()
        {
            base.destroy();
            actorElement.ContactStarted -= actorElement_ContactStarted;
            actorElement.ContactEnded -= actorElement_ContactEnded;
        }

        protected override void applyAdaptation(ToothType type, bool adapt)
        {
            if (adapt)
            {
                if (type == ToothType.Bottom && collidingTeeth.Count > 0)
                {
                    joint.setLinearLowerLimit(new Vector3(-1.0f, -1.0f, -1.0f));
                    joint.setLinearUpperLimit(new Vector3(1.0f, 1.0f, 1.0f));
                    joint.setAngularLowerLimit(new Vector3(-3.14f, -3.14f, -3.14f));
                    joint.setAngularUpperLimit(new Vector3(3.14f, 3.14f, 3.14f));
                }
            }
            else
            {
                SimObject other = Owner.getOtherSimObject("Mandible");
                Offset = Quaternion.quatRotate(other.Rotation.inverse(), Owner.Translation - other.Translation) - startingLocation;
                Rotation = other.Rotation.inverse() * Owner.Rotation * startingRotation.inverse();
                joint.setLinearLowerLimit(Vector3.Zero);
                joint.setLinearUpperLimit(Vector3.Zero);
                joint.setAngularLowerLimit(Vector3.Zero);
                joint.setAngularUpperLimit(Vector3.Zero);
            }
        }

        void actorElement_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            TopTooth otherTooth = otherBody.Owner.getElement("Behavior") as TopTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Add(otherTooth);
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            TopTooth otherTooth = otherBody.Owner.getElement("Behavior") as TopTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Remove(otherTooth);
            }
        }
    }
}
