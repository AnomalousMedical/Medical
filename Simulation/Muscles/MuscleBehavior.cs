using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using Engine.Platform;
using Engine.ObjectManagement;
using Logging;
using Engine.Renderer;
using BulletPlugin;

namespace Medical
{
    public class MuscleBehavior : Behavior
    {
        [Editable]
        protected String targetSimObject;

        [Editable]
        protected float force = 50.0f;

        [Editable]
        protected String actorName = "Actor";

        protected RigidBody actor;
        protected SimObject targetObject;

        [DoNotSave]
        protected bool selected = false;

        public MuscleBehavior()
        {

        }

        protected override void constructed()
        {
            actor = Owner.getElement(actorName) as RigidBody;
            if (actor == null)
            {
                blacklist("Cannot find actor {0}.", actorName);
            }
            targetObject = Owner.getOtherSimObject(targetSimObject);
            if (targetObject == null)
            {
                blacklist("Cannot find SimObject {0}.", targetSimObject);
            }
            MuscleController.addMuscle(Owner.Name, this);
        }

        protected override void destroy()
        {
            MuscleController.removeMuscle(Owner.Name);
        }

        public override void update(Clock clock, EventManager events)
        {
            if (force != 0.0f)
            {
                Vector3 location = targetObject.Translation - Owner.Translation;
                location.normalize();
                location *= force;
                actor.applyCentralImpulse(location);
                actor.forceActivationState(ActivationState.ActiveTag);
            }
        }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            Vector3 halfwayPoint = targetObject.Translation - Owner.Translation;
            halfwayPoint = halfwayPoint.length() / 2 * halfwayPoint.normalize() + Owner.Translation;
            debugDrawing.begin("Muscle" + Owner.Name, DrawingType.LineList);
            debugDrawing.setColor(Color.Green);
            debugDrawing.drawLine(Owner.Translation, halfwayPoint);
            debugDrawing.setColor(Color.Red);
            debugDrawing.drawLine(halfwayPoint, targetObject.Translation);

            Vector3 location = targetObject.Translation - Owner.Translation;
            location.normalize();
            location *= force;
            debugDrawing.setColor(Color.Blue);
            debugDrawing.drawLine(Owner.Translation, Owner.Translation + location);

            debugDrawing.end();
        }

        public void changeForce(float force)
        {
            this.force = force;
        }

        public float getForce()
        {
            return force;
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }
    }
}
