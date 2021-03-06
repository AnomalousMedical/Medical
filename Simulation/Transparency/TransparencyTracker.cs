﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using OgrePlugin;

namespace Medical
{
    public class TransparencyTracker : TransparencySubInterface
    {
        [Editable]
        private String alphaSuffix = "Alpha";
        //An optional alpha material, if this is defined the alpha suffix will be ignored
        //and this material will be used instead.
        [Editable]
        private String alphaMaterialName = null;
        [Editable]
        private float currentAlpha = 1.0f;
        [Editable]
        private String nodeName;
        [Editable]
        private String entityName;
        [Editable]
        private bool disableOnHidden = true;
        [Editable]
        private int renderGroupOffset = 0;
        [Editable]
        private uint subEntityIndex = 0;

        [Editable]
        public String ObjectName { get; private set; }
        [Editable]
        public RenderGroup RenderGroup { get; private set; }

        Entity entity;
        TransparencyStatus status = TransparencyStatus.Solid;

        [DoNotSave]
        [DoNotCopy]
        String finalAlphaMaterialName;
        SubEntity subEntity;

        [DoNotSave]
        String baseMaterialName;
        [DoNotSave]
        Quaternion alphaQuat = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        [DoNotCopy]
        [DoNotSave]
        private byte originalRenderGroup;

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode != null)
            {
                entity = sceneNode.getNodeObject(entityName) as Entity;
            }

            if (entity == null)
            {
                blacklist("No entity specified or entity is not found.");
            }

            originalRenderGroup = entity.getRenderQueueGroup();

            if (subEntityIndex >= entity.getNumSubEntities())
            {
                blacklist("Entity {0} only has {1} SubEntities. Index {2} is invalid.", entity.getName(), entity.getNumSubEntities(), subEntityIndex);
            }
            subEntity = entity.getSubEntity(subEntityIndex);
            baseMaterialName = subEntity.getMaterialName();
            MaterialManager materialManager = MaterialManager.getInstance();
            bool useDefinedMaterial = !(alphaMaterialName == null || alphaMaterialName.Equals(String.Empty));
            if (useDefinedMaterial)
            {
                if (materialManager.resourceExists(alphaMaterialName))
                {
                    finalAlphaMaterialName = alphaMaterialName;
                }
                else
                {
                    blacklist("A custom material {0} is defined that cannot be found.  This object will not be able to be alpha controlled.", this.alphaMaterialName);
                }
            }
            else
            {
                if (materialManager.resourceExists(baseMaterialName + alphaSuffix))
                {
                    finalAlphaMaterialName = baseMaterialName + alphaSuffix;
                }
                else
                {
                    blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", baseMaterialName + alphaSuffix);
                }
            }
            setAlpha(currentAlpha);
        }

        internal override void setAlpha(float alpha)
        {
            currentAlpha = alpha;
            alphaQuat.w = alpha;
            subEntity.setCustomParameter(0, alphaQuat);
            if (alpha >= 0.9999f)
            {
                if (status != TransparencyStatus.Solid)
                {
                    status = TransparencyStatus.Solid;
                    subEntity.setMaterialName(baseMaterialName);
                    entity.setRenderQueueGroup(originalRenderGroup);
                    entity.setVisibilityFlags(TransparencyController.OpaqueVisibilityMask);
                    subEntity.setVisible(true);
                    //entity.setMaterialLodBias(1.0f, 0, 0);
                }
            }
            else if (alpha <= 0.00008f)
            {
                if (status != TransparencyStatus.Hidden)
                {
                    status = TransparencyStatus.Hidden;
                    if (disableOnHidden)
                    {
                        subEntity.setVisible(false);
                    }
                }
            }
            else
            {
                if (status != TransparencyStatus.Transparent)
                {
                    status = TransparencyStatus.Transparent;
                    subEntity.setMaterialName(finalAlphaMaterialName);
                    subEntity.setVisible(true);
                    //entity.setMaterialLodBias(1.0f, 1, 1);
                    entity.setRenderQueueGroup(RenderGroupQueue.GetQueue(RenderGroup, (byte)renderGroupOffset));
                    entity.setVisibilityFlags(TransparencyController.TransparentVisibilityMask);
                }
            }
        }
    }
}
