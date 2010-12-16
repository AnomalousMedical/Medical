using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgreWrapper;
using OgrePlugin;
using Engine.Attributes;

namespace Medical
{
    public class ArrowBehavior : Interface
    {
        public const String NodeName = "Node";
        public const String EntityName = "Entity";

        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        public ArrowBehavior()
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
