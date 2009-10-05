using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using OgreWrapper;
using OgrePlugin;

namespace Medical
{
    /// <summary>
    /// This class will make an entire sub entity blank if any alpha is applied.
    /// </summary>
    class TransparencyBlock : TransparencySubInterface
    {
        [Editable]
        private String nodeName;
        [Editable]
        private String entityName;
        [Editable]
        private uint subEntityIndex = 0;

        Entity entity;
        SubEntity subEntity;

        protected override void constructed()
        {
            base.constructed();
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode != null)
            {
                entity = sceneNode.getNodeObject(entityName) as Entity;
            }

            if (entity == null)
            {
                blacklist("No entity specified or entity is not found.");
            }
            if (subEntityIndex >= entity.getNumSubEntities())
            {
                blacklist("Entity {0} only has {1} SubEntities. Index {2} is invalid.", entity.getName(), entity.getNumSubEntities(), subEntityIndex);
            }
            subEntity = entity.getSubEntity(subEntityIndex); 
        }

        internal override void setAlpha(float alpha)
        {
            subEntity.setVisible(alpha >= 1.0f);
        }
    }
}
