using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using PhysXWrapper;
using Engine.Platform;
using Engine.ObjectManagement;
using PhysXPlugin;
using Logging;
using Engine.Renderer;

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

        protected PhysActorElement actor;
        protected SimObject targetObject;

        [DoNotSave]
        protected bool selected = false;

        public MuscleBehavior()
        {

        }

        protected override void constructed()
        {
            actor = SimObject.getElement(actorName) as PhysActorElement;
            if (actor == null)
            {
                blacklist("Cannot find actor {0}.", actorName);
            }
            targetObject = SimObject.getOtherSimObject(targetSimObject);
            if (targetObject == null)
            {
                blacklist("Cannot find SimObject {0}.", targetSimObject);
            }
            MuscleController.addMuscle(SimObject.Name, this);
        }

        protected override void destroy()
        {
            MuscleController.removeMuscle(SimObject.Name);
        }

        public override void update(Clock clock, EventManager events)
        {
            Vector3 location = SimObject.Translation - targetObject.Translation;
            location.normalize();
            location *= force;
            actor.Actor.addLocalForce(ref location, ForceMode.NX_SMOOTH_IMPULSE, true);
        }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            Vector3 halfwayPoint = targetObject.Translation - SimObject.Translation;
            halfwayPoint = halfwayPoint.length() / 2 * halfwayPoint.normalize() + SimObject.Translation;
            debugDrawing.begin("Muscle" + SimObject.Name, DrawingType.LineList);
            debugDrawing.setColor(Color.Green);
            debugDrawing.drawLine(SimObject.Translation, halfwayPoint);
            debugDrawing.setColor(Color.Red);
            debugDrawing.drawLine(halfwayPoint, targetObject.Translation);
            debugDrawing.end();
        }

        /*public void draw(LineHelper surface)
        {
            if (selected)
            {
                surface.setColor(0.0f, 0.0f, 1.0f, 1.0f);
            }
            else
            {
                surface.setColor(1.0f, 0.0f, 0.0f, 1.0f);
            }
            surface.drawLine(getTranslation(), staticPoint);
        }*/

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
