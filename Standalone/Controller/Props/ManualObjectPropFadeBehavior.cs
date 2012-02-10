using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    /// <summary>
    /// This is pretty hacky, but it does everything propfadebehavior does, but for manual objects instead.
    /// ManualObjects just have manualObject and ManualObjectSections instead.
    /// </summary>
    class ManualObjectPropFadeBehavior : PropFadeBehavior
    {
        private ManualObject entity;
        private ManualObjectSection subEntity;
        private String entityName = PropFactory.ManualObjectName;

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find node {0}.", nodeName);
            }
            entity = sceneNode.getNodeObject(entityName) as ManualObject;
            if (entity == null)
            {
                blacklist("No entity specified or entity is not found.");
            }
            if (subEntityIndex >= entity.getNumSections())
            {
                blacklist("Entity {0} only has {1} SubEntities. Index {3} is invalid.", entity.getName(), entity.getNumSections(), subEntityIndex);
            }
            subEntity = entity.getSection(subEntityIndex);
            baseMaterialName = subEntity.getMaterialName();
            MaterialManager materialManager = MaterialManager.getInstance();
            String alphaMaterialName = baseMaterialName + alphaSuffix;
            if (materialManager.resourceExists(alphaMaterialName))
            {
                finalAlphaMaterialName = alphaMaterialName;
            }
            else
            {
                blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", alphaMaterialName);
            }
        }

        protected override void applyAlphaToMaterial(float alpha)
        {
            workingAlpha = alpha;
            alphaQuat.w = alpha;
            subEntity.setCustomParameter(0, alphaQuat);
            if (disableOnHidden)
            {
                entity.setVisible(alpha != 0.0f);
            }
            if (alpha == 1.0f)
            {
                subEntity.setMaterialName(baseMaterialName);
                entity.setRenderQueueGroup(0);
            }
            else
            {
                subEntity.setMaterialName(finalAlphaMaterialName);
                entity.setRenderQueueGroup((byte)(95 + renderGroupOffset));
            }
        }
    }
}
