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
    public enum TongueMode
    {
        Protrude,
        Back,
        Rest,
    }

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

        [Editable]
        private String backAnimationName = "Back";

        [Editable]
        private String restAnimationName = "Rest";

        [Editable]
        private TongueMode tongueMode = TongueMode.Back;

        [Editable]
        private float currentTonguePosition = 0.0f;

        AnimationState protrude;
        AnimationState back;
        AnimationState rest;
        AnimationState current;

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

            protrude = entity.getAnimationState(protrudeAnimationName);
            if (protrude == null)
            {
                blacklist("Tongue Entity {0} does not have an animation for protrusion named {1}.", tongueEntityName, protrudeAnimationName);
            }

            back = entity.getAnimationState(backAnimationName);
            if (back == null)
            {
                blacklist("Tongue Entity {0} does not have an animation for back named {1}.", tongueEntityName, backAnimationName);
            }

            rest = entity.getAnimationState(restAnimationName);
            if (rest == null)
            {
                blacklist("Tongue Entity {0} does not have an animation for rest named {1}.", tongueEntityName, restAnimationName);
            }

            protrude.setLoop(false);
            back.setLoop(false);
            rest.setLoop(false);

            TongueMode = tongueMode;
        }

        protected override void link()
        {
            TongueController.setTongue(this);
        }

        protected override void destroy()
        {
            TongueController.setTongue(null);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            
        }

        [DoNotCopy]
        public TongueMode TongueMode
        {
            get
            {
                return tongueMode;
            }
            set
            {
                this.tongueMode = value;
                switch (tongueMode)
                {
                    case TongueMode.Protrude:
                        current = protrude;
                        protrude.setEnabled(true);
                        back.setEnabled(false);
                        rest.setEnabled(false);
                        break;
                    case TongueMode.Back:
                        current = back;
                        protrude.setEnabled(false);
                        back.setEnabled(true);
                        rest.setEnabled(false);
                        break;
                    case TongueMode.Rest:
                        current = rest;
                        protrude.setEnabled(false);
                        back.setEnabled(false);
                        rest.setEnabled(true);
                        break;
                }
                current.setTimePosition(currentTonguePosition);
            }
        }

        [DoNotCopy]
        public float CurrentTonguePosition
        {
            get
            {
                return currentTonguePosition;
            }
            set
            {
                currentTonguePosition = value;
                current.setTimePosition(currentTonguePosition);
            }
        }
    }
}
