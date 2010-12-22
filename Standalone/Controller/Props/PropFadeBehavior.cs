using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    class PropFadeBehavior : Behavior
    {
        private String nodeName = PropFactory.NodeName;
        private String entityName = PropFactory.EntityName;
        private String alphaSuffix = "Alpha";
        private uint subEntityIndex = 0;
        private bool disableOnHidden = true;
        private byte renderGroupOffset = 0;

        private Entity entity;
        private SubEntity subEntity;
        private String baseMaterialName;
        private MaterialPtr alphaMaterial;
        private Color diffuse;

        //Transparency controls
        private float workingAlpha = 1.0f;
        private float targetOpacity = 1.0f;
        private bool changingOpacity = false;
        private float opacityChangeMultiplier = 1.0f;

        public PropFadeBehavior()
        {

        }

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find node {0}.", nodeName);
            }
            entity = sceneNode.getNodeObject(entityName) as Entity;
            if (entity == null)
            {
                blacklist("No entity specified or entity is not found.");
            }
            if (subEntityIndex >= entity.getNumSubEntities())
            {
                blacklist("Entity {0} only has {1} SubEntities. Index {3} is invalid.", entity.getName(), entity.getNumSubEntities(), subEntityIndex);
            }
            subEntity = entity.getSubEntity(subEntityIndex);
            baseMaterialName = subEntity.getMaterialName();
            MaterialManager materialManager = MaterialManager.getInstance();
            String alphaMaterialName = baseMaterialName + alphaSuffix;
            if (materialManager.resourceExists(alphaMaterialName))
            {
                alphaMaterial = materialManager.getByName(alphaMaterialName);
            }
            else
            {
                blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", alphaMaterialName);
            }
            diffuse = alphaMaterial.Value.getTechnique(0).getPass("Color").getDiffuse();
        }

        protected override void destroy()
        {
            if (alphaMaterial != null)
            {
                alphaMaterial.Dispose();
                alphaMaterial = null;
            }
        }

        public void fade(float targetOpacity, float duration)
        {
            float delta = Math.Abs(targetOpacity - workingAlpha);
            if (delta != 0.0f)
            {
                float changeMultiplier = delta / duration;
                smoothBlend(targetOpacity, changeMultiplier);
            }
        }

        public void hide()
        {
            applyAlphaToMaterial(0.0f);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (changingOpacity)
            {
                if (workingAlpha > targetOpacity)
                {
                    workingAlpha -= (float)clock.Seconds * opacityChangeMultiplier;
                    if (workingAlpha < targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                else
                {
                    workingAlpha += (float)clock.Seconds * opacityChangeMultiplier;
                    if (workingAlpha > targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                applyAlphaToMaterial(workingAlpha);
            }
        }

        private void applyAlphaToMaterial(float alpha)
        {
            workingAlpha = alpha;
            diffuse.a = alpha;
            alphaMaterial.Value.setDiffuse(diffuse);
            if (disableOnHidden)
            {
                subEntity.setVisible(alpha != 0.0f);
            }
            if (alpha == 1.0f)
            {
                subEntity.setMaterialName(baseMaterialName);
                entity.setRenderQueueGroup(0);
            }
            else
            {
                subEntity.setMaterialName(alphaMaterial.Value.getName());
                entity.setRenderQueueGroup((byte)(95 + renderGroupOffset));
            }
        }

        private void smoothBlend(float targetOpacity, float changeMultiplier)
        {
            changingOpacity = true;
            this.targetOpacity = targetOpacity;
            this.opacityChangeMultiplier = changeMultiplier;
        }
    }
}
