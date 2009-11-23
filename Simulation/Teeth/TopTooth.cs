using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using BulletPlugin;
using Engine;
using OgreWrapper;
using Engine.ObjectManagement;
using Engine.Attributes;

namespace Medical
{
    class TopTooth : Tooth
    {
        protected override void looseChanged(bool loose)
        {
            //if (loose)
            //{
            //    actorElement.clearCollisionFlag(CollisionFlags.KinematicObject);
            //}
            //else
            //{
            //    actorElement.raiseCollisionFlag(CollisionFlags.KinematicObject);
            //}
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
            BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Add(otherTooth);
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
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
