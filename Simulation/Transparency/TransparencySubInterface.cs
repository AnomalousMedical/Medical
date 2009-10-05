﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    /// <summary>
    /// This class will do transparency based off another TransparencyInterface
    /// </summary>
    class TransparencySubInterface : Interface
    {
        [Editable] RenderGroup parentGroup;
        [Editable] String parentInterfaceName;

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

        MaterialPtr alphaMaterial;
        SubEntity subEntity;

        [DoNotSave]
        String baseMaterialName;
        [DoNotSave]
        Color diffuse;

        [DoNotCopy]
        [DoNotSave]
        TransparencyInterface parentInterface;

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
                    alphaMaterial = materialManager.getByName(alphaMaterialName);
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
                    alphaMaterial = materialManager.getByName(baseMaterialName + alphaSuffix);
                }
                else
                {
                    blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", baseMaterialName + alphaSuffix);
                }
            }
            subEntity.setMaterialName(alphaMaterial.Value.getName());
            diffuse = alphaMaterial.Value.getTechnique(0).getPass("Color").getDiffuse();
            setAlpha(currentAlpha);
        }

        protected override void link()
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(parentGroup);
            if (group == null)
            {
                blacklist("Cannot find parent group {0}.", parentGroup);
            }
            parentInterface = group.getTransparencyObject(parentInterfaceName);
            if (parentInterface == null)
            {
                blacklist("Cannot find parent interface {0} in group {1}.", parentInterfaceName, parentGroup);
            }
            parentInterface.addSubInterface(this);
        }

        protected override void destroy()
        {
            base.destroy();
            if (parentInterface != null)
            {
                parentInterface.removeSubInterface(this);
            }
        }

        internal void setAlpha(float alpha)
        {
            currentAlpha = alpha;
            diffuse.a = alpha;
            alphaMaterial.Value.setDiffuse(ref diffuse);
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
                subEntity.setMaterialName(alphaMaterial.Value.getName());
                switch (RenderGroup)
                {
                    case RenderGroup.None:
                        entity.setRenderQueueGroup((byte)(255 + renderGroupOffset));
                        break;
                    case RenderGroup.Teeth:
                        entity.setRenderQueueGroup((byte)(0 + renderGroupOffset));
                        break;
                    case RenderGroup.Bones:
                        entity.setRenderQueueGroup((byte)(20 + renderGroupOffset));
                        break;
                    case RenderGroup.Muscles:
                        entity.setRenderQueueGroup((byte)(20 + renderGroupOffset));
                        break;
                    case RenderGroup.Skin:
                        entity.setRenderQueueGroup((byte)(100 + renderGroupOffset));
                        break;
                    case RenderGroup.Spine:
                        entity.setRenderQueueGroup((byte)(20 + renderGroupOffset));
                        break;
                    case RenderGroup.Nasal:
                        entity.setRenderQueueGroup((byte)(20 + renderGroupOffset));
                        break;
                    case RenderGroup.TMJ:
                        entity.setRenderQueueGroup((byte)(10 + renderGroupOffset));
                        break;
                }
            }
        }

        /// <summary>
        /// This function should only be called from the TransparencyInterface.
        /// </summary>
        internal void _disconnectFromInterface()
        {
            parentInterface = null;
        }
    }
}
