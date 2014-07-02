using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgrePlugin;
using OgreWrapper;
using Engine.Attributes;

namespace Medical
{
    class PropFadeBehavior : Behavior
    {
        protected String nodeName = PropFactory.NodeName;
        private String entityName = PropFactory.EntityName;
        protected String alphaSuffix = "Alpha";
        protected uint subEntityIndex = 0;
        protected bool disableOnHidden = true;
        protected byte renderGroupOffset = 0;

        private Entity entity;
        private SubEntity subEntity;
        protected String baseMaterialName;

        [DoNotSave]
        [DoNotCopy]
        protected String finalAlphaMaterialName;

        [DoNotSave]
        [DoNotCopy]
        protected Quaternion alphaQuat = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        //Transparency controls
        protected float workingAlpha = 1.0f;
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
                finalAlphaMaterialName = alphaMaterialName;
            }
            else
            {
                blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", alphaMaterialName);
            }
        }

        protected override void destroy()
        {
            
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

        [DoNotCopy]
        public float CurrentTransparency
        {
            get
            {
                return workingAlpha;
            }
            set
            {
                workingAlpha = value;
                targetOpacity = value;
                changingOpacity = false;
                applyAlphaToMaterial(workingAlpha);
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
                    workingAlpha -= clock.DeltaSeconds * opacityChangeMultiplier;
                    if (workingAlpha < targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                else
                {
                    workingAlpha += clock.DeltaSeconds * opacityChangeMultiplier;
                    if (workingAlpha > targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                applyAlphaToMaterial(workingAlpha);
            }
        }

        public void changePosition(Vector3 translation, Quaternion rotation)
        {
            bool transValid = translation.isNumber();
            bool rotValid = rotation.isNumber();
            if (transValid && rotValid)
            {
                this.updatePosition(ref translation, ref rotation);
            }
            else if(transValid)
            {
                this.updateTranslation(ref translation);
            }
            else if (rotValid)
            {
                this.updateRotation(ref rotation);
            }
        }

        protected virtual void applyAlphaToMaterial(float alpha)
        {
            workingAlpha = alpha;
            alphaQuat.w = alpha;
            subEntity.setCustomParameter(0, alphaQuat);
            if (disableOnHidden)
            {
                subEntity.setVisible(alpha >= 0.00008f);
            }
            if (alpha >= 0.9999f)
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

        private void smoothBlend(float targetOpacity, float changeMultiplier)
        {
            changingOpacity = true;
            this.targetOpacity = targetOpacity;
            this.opacityChangeMultiplier = changeMultiplier;
        }
    }
}
