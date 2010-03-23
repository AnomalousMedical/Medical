using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine.Attributes;
using Logging;

namespace Medical
{
    public class Tongue : Behavior
    {
        [Editable]
        private String tongueSimObjectName = "Tongue";

        [Editable]
        private String tongueNodeName = "Node";

        [Editable]
        private String tongueEntityName = "Entity";

        [Editable]
        private String protrudeAnimationName = "Protrude";

        AnimationState anim;

        protected override void constructed()
        {
            SimObject tongueObject = Owner.getOtherSimObject(tongueSimObjectName);
            if (tongueObject == null)
            {
                blacklist("Could not find Target tongue SimObject {0}.", tongueSimObjectName);
            }

            SceneNodeElement node = tongueObject.getElement(tongueNodeName) as SceneNodeElement;
            if (node == null)
            {
                blacklist("Could not find target tongue SceneNodeElement {0}.", tongueSimObjectName);
            }

            Entity entity = node.getNodeObject(tongueEntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find tongue Entity {0}.", tongueNodeName);
            }

            if (!entity.hasSkeleton())
            {
                blacklist("Tongue Entity {0} does not have a skeleton.", tongueEntityName);
            }

            anim = entity.getAnimationState(protrudeAnimationName);
            if (anim == null)
            {
                blacklist("Tongue Entity {0} does not have an animation for protrusion named {1}.", tongueEntityName, protrudeAnimationName);
            }
            anim.setLoop(true);
            anim.setEnabled(true);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            anim.addTime((float)clock.Seconds / 3.0f);
        }
    }
}
