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
        public const String NodeName = "Node";
        public const String EntityName = "Entity";

        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition arrowSimObject = new GenericSimObjectDefinition("ArrowPrototype");
            arrowSimObject.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(Arrow.EntityName);
            entityDefinition.MeshName = "Arrow.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(Arrow.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            arrowSimObject.addElement(nodeDefinition);
            Arrow arrowBehavior = new Arrow();
            BehaviorDefinition arrowBehaviorDef = new BehaviorDefinition("Behavior", arrowBehavior);
            arrowSimObject.addElement(arrowBehaviorDef);
            propFactory.addDefinition("Arrow", arrowSimObject);
        }

        public Arrow()
        {

        }

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Arrow SimObject", NodeName);
            }
            entity = sceneNode.getNodeObject(EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Arrow SimObject", EntityName);
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
