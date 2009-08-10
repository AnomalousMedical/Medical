﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;
using OgreWrapper;
using Engine.Attributes;
using BulletPlugin;

namespace Medical
{
    public abstract class Tooth : Behavior
    {
        [Editable]
        private String sceneNodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private String actorName = "Actor";

        [Editable]
        private String jointName = "Joint";

        [Editable]
        private String transparencyInterface = "Alpha";

        [Editable]
        private bool extracted = false;

        protected bool loose = false;

        protected SceneNodeElement sceneNodeElement;
        protected Entity entity;

        private TransparencyInterface transparency;
        private RigidBody actorElement;
        private Generic6DofConstraintElement joint;

        protected override void constructed()
        {
            TeethController.addTooth(Owner.Name, this);
            sceneNodeElement = Owner.getElement(sceneNodeName) as SceneNodeElement;
            if (sceneNodeElement == null)
            {
                blacklist("Could not find SceneNodeElement {0}.", sceneNodeName);
            }
            else
            {
                entity = sceneNodeElement.getNodeObject(entityName) as Entity;
                if (entity == null)
                {
                    blacklist("Could not find Entity {0}.", entityName);
                }
            }
            actorElement = Owner.getElement(actorName) as RigidBody;
            if (actorElement == null)
            {
                blacklist("Could not find Actor {0}.", actorName);
            }
            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }
            transparency = Owner.getElement(transparencyInterface) as TransparencyInterface;
            if (transparency == null)
            {
                blacklist("Could not find TransparencyInterface {0}", transparencyInterface);
            }
        }

        protected override void destroy()
        {
            TeethController.removeTooth(Owner.Name);
        }

        public override void update(Clock clock, EventManager eventManager)
        {

        }

        protected abstract void looseChanged(bool loose);

        public bool Extracted
        {
            get
            {
                return extracted;
            }
            set
            {
                //Put the tooth back if extracted
                if (this.extracted && !value)
                {
                    extracted = false;
                    actorElement.clearCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(true);
                    transparency.DisableOnHidden = true;
                }
                //Extract the tooth if it is in the scene
                else if (!this.extracted && value)
                {
                    extracted = true;
                    actorElement.raiseCollisionFlag(CollisionFlags.NoContactResponse);
                    entity.setVisible(false);
                    transparency.DisableOnHidden = false;
                }
            }
        }

        [DoNotCopy]
        public bool Loose
        {
            get
            {
                return loose;
            }
            set
            {
                loose = value;
                looseChanged(loose);
            }
        }
    }
}
