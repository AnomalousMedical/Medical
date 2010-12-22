using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgreWrapper;
using OgrePlugin;
using Engine.Attributes;
using Engine.ObjectManagement;

namespace Medical
{
    class Arrow : Interface
    {
        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition arrowSimObject = new GenericSimObjectDefinition("ArrowPrototype");
            arrowSimObject.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Arrow.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            arrowSimObject.addElement(nodeDefinition);
            Arrow arrowBehavior = new Arrow();
            BehaviorDefinition arrowBehaviorDef = new BehaviorDefinition("Behavior", arrowBehavior);
            arrowSimObject.addElement(arrowBehaviorDef);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            arrowSimObject.addElement(propFadeBehaviorDef);
            propFactory.addDefinition("Arrow", arrowSimObject);
        }

        public Arrow()
        {

        }

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Arrow SimObject", PropFactory.NodeName);
            }
            entity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Arrow SimObject", PropFactory.EntityName);
            }
            subEntity = entity.getSubEntity(0);
            if (subEntity == null)
            {
                blacklist("Could not find sub entity with index 0 in Arrow SimObject");
            }
            Color = color;
        }

        protected override void destroy()
        {
            base.destroy();
        }

        [DoNotCopy]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (subEntity != null)
                {
                    subEntity.setCustomParameter(0, new Quaternion(value.r, value.g, value.b, value.a));
                }
            }
        }
    }
}
