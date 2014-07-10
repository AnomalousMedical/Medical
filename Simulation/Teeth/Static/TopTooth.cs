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
    class TopTooth : StaticTooth
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
                RigidBody other = joint.RigidBodyA;
                Offset = actorElement.PhysicsTranslation - other.PhysicsTranslation - startingLocation;
                Rotation = actorElement.PhysicsRotation * startingRotation.inverse();
                joint.setLinearLowerLimit(Vector3.Zero);
                joint.setLinearUpperLimit(Vector3.Zero);
                joint.setAngularLowerLimit(Vector3.Zero);
                joint.setAngularUpperLimit(Vector3.Zero);
            }
        }

        void actorElement_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            if (otherBody != null)
            {
                BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
                if (otherTooth != null)
                {
                    collidingTeeth.Add(otherTooth);
                }
                else
                {
                    Splint splint = otherBody.Owner.getElement(Splint.SplintBehaviorName) as Splint;
                    if (splint != null)
                    {
                        collidingSplints.Add(splint);
                    }
                }
            }
        }

        void actorElement_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            BottomTooth otherTooth = otherBody.Owner.getElement("Behavior") as BottomTooth;
            if (otherTooth != null)
            {
                collidingTeeth.Remove(otherTooth);
            }
            else
            {
                Splint splint = otherBody.Owner.getElement(Splint.SplintBehaviorName) as Splint;
                if (splint != null)
                {
                    collidingSplints.Remove(splint);
                }
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
