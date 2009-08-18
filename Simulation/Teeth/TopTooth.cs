using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using BulletPlugin;
using Engine;
using OgreWrapper;

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

        void actorElement_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
            if (otherTooth != null)
            {
                numContacts++;
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
            if (otherTooth != null)
            {
                numContacts--;
            }
        }
    }
}
