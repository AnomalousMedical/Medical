using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine;
using Logging;
using BulletPlugin;

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
