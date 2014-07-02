using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using OgreWrapper;
using Engine.Platform;
using Engine.Attributes;

namespace Medical
{
    public class Pen : Behavior
    {
        public const String DefinitionName = "Pen";
        public const String PenBehaviorName = "PenBehavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition pen = new GenericSimObjectDefinition(DefinitionName);
            pen.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Pen.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            pen.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            pen.addElement(propFadeBehaviorDef);

            Pen penBehavior = new Pen();
            BehaviorDefinition penBehaviorDef = new BehaviorDefinition(PenBehaviorName, penBehavior);
            pen.addElement(penBehaviorDef);

            propFactory.addDefinition(DefinitionName, pen);
        }

        private Entity penEntity;
        private AnimationState clickOnAnim;
        private AnimationState clickOffAnim;
        private AnimationState currentAnimation;
        private bool clicked = false;

        public Pen()
        {

        }

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in pen.", PropFactory.NodeName);
            }
            
            penEntity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (penEntity == null)
            {
                blacklist("Could not find Entity {0} in pen.", PropFactory.NodeName);
            }
            
            clickOnAnim = penEntity.getAnimationState("ClickOn");
            if (clickOnAnim == null)
            {
                blacklist("Could not find Animation ClickOn in pen.");
            }
            clickOnAnim.setLoop(false);

            clickOffAnim = penEntity.getAnimationState("ClickOff");
            if (clickOffAnim == null)
            {
                blacklist("Could not find Animation ClickOff in pen.");
            }
            clickOffAnim.setLoop(false);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (currentAnimation != null)
            {
                currentAnimation.addTime(clock.DeltaSeconds);
                if (currentAnimation.getTimePosition() > currentAnimation.getLength())
                {
                    currentAnimation = null;
                    currentAnimation.setEnabled(false);
                }
            }
        }

        public void clickOn()
        {
            clickOffAnim.setEnabled(false);
            clickOnAnim.setEnabled(true);
            clickOnAnim.setTimePosition(0);

            currentAnimation = clickOnAnim;
        }

        public void clickOff()
        {
            clickOnAnim.setEnabled(false);
            clickOffAnim.setEnabled(true);
            clickOffAnim.setTimePosition(0);

            currentAnimation = clickOffAnim;
        }

        [DoNotCopy]
        public bool Clicked
        {
            get
            {
                return clicked;
            }
            set
            {
                clicked = value;
                if (clicked)
                {
                    clickOn();
                }
                else
                {
                    clickOff();
                }
            }
        }
    }
}
