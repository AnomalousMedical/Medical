using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletPlugin;
using OgreWrapper;
using Engine;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;
using Engine.Renderer;
using OgrePlugin;
using Engine.ObjectManagement;

namespace Medical
{
    class ReshapeableTopTooth : ReshapeableTooth
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
                if (type == ToothType.Top && collidingTeeth.Count > 0)
                {
                    joint.setLinearLowerLimit(new Vector3(-1.0f, -1.0f, -1.0f));
                    joint.setLinearUpperLimit(new Vector3(1.0f, 1.0f, 1.0f));
                    joint.setAngularLowerLimit(new Vector3(-3.14f, -3.14f, -3.14f));
                    joint.setAngularUpperLimit(new Vector3(3.14f, 3.14f, 3.14f));
                }
            }
            else
            {
                SimObject otherSimObject = joint.RigidBodyA.Owner;
                Offset = Owner.Translation - otherSimObject.Translation - startingLocation;
                Rotation = Owner.Rotation * startingRotation.inverse();
                joint.setLinearLowerLimit(Vector3.Zero);
                joint.setLinearUpperLimit(Vector3.Zero);
                joint.setAngularLowerLimit(Vector3.Zero);
                joint.setAngularUpperLimit(Vector3.Zero);
            }
        }

        void actorElement_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            ReshapeableBottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as ReshapeableBottomTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Add(otherTooth);
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            ReshapeableBottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as ReshapeableBottomTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Remove(otherTooth);
            }
        }

        [DoNotCopy]
        public override bool IsTopTooth
        {
            get
            {
                return true;
            }
        }
    }
}
