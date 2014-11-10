﻿using System;
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
    public delegate void MuscleForceChanged(MuscleBehavior source, float force);

    public class MuscleBehavior : Behavior
    {
        [DoNotSave]
        [DoNotCopy]
        public event MuscleForceChanged ForceChanged;

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

        [DoNotCopy]
        [DoNotSave]
        private BulletScene bulletScene;

        [DoNotSave]
        [DoNotCopy]
        private bool awake = true;

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

            bulletScene = actor.Scene;
            bulletScene.Tick += bulletScene_Tick;

            addToDebugDrawing();
        }

        protected override void destroy()
        {
            bulletScene.Tick -= bulletScene_Tick;
            MuscleController.removeMuscle(Owner.Name);
        }

        public override void update(Clock clock, EventManager events)
        {
            awake = !SleepyActorRepository.IsSleepy;
        }

        void bulletScene_Tick(float timeSpan)
        {
            if (awake && force != 0.0f)
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
            debugDrawing.Color = Color.Green;
            debugDrawing.drawLine(Owner.Translation, halfwayPoint);
            debugDrawing.Color = Color.Red;
            debugDrawing.drawLine(halfwayPoint, targetObject.Translation);

            Vector3 location = targetObject.Translation - Owner.Translation;
            location.normalize();
            location *= force;
            debugDrawing.Color = Color.Blue;
            debugDrawing.drawLine(Owner.Translation, Owner.Translation + location);

            debugDrawing.end();
        }

        public void changeForce(float force)
        {
            if (this.force != force)
            {
                this.force = force;
                SleepyActorRepository.wakeUp();
                if (ForceChanged != null)
                {
                    ForceChanged.Invoke(this, force);
                }
            }
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
