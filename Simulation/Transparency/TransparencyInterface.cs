using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using OgreWrapper;
using Engine.Editing;
using Engine.Attributes;
using OgrePlugin;
using Logging;
using Engine.Platform;

namespace Medical
{
    [SingleEnum]
    public enum RenderGroup
    {
        None,
        Teeth,
        Bones,
        Muscles,
        Skin,
        Spine,
        Nasal,
        TMJ
    }

    public class TransparencyInterface : Behavior
    {
        [Editable] private String alphaSuffix = "Alpha";
        //An optional alpha material, if this is defined the alpha suffix will be ignored
        //and this material will be used instead.
        [Editable] private String alphaMaterialName = null;
        [Editable] private float currentAlpha = 1.0f;
        [Editable] private String nodeName;
        [Editable] private String entityName;
        [Editable] private bool disableOnHidden = true;
        [Editable] private float targetOpacity = 1.0f;
        [Editable] private bool changingOpacity = false;
        [Editable] private int renderGroupOffset = 0;
        [Editable] private uint subEntityIndex = 0;

        [Editable] public String ObjectName { get; private set; }
        [Editable] public RenderGroup RenderGroup { get; private set; }

        Entity entity;

        MaterialPtr alphaMaterial;
        SubEntity subEntity;

        [DoNotSave]
        String baseMaterialName;
        [DoNotSave]
        Color diffuse;

        [DoNotCopy]
        [DoNotSave]
        private List<TransparencySubInterface> subInterfaces;

        public TransparencyInterface()
        {
            ObjectName = "";
            RenderGroup = RenderGroup.None;
        }

        public void setAlpha(float alpha)
        {
            currentAlpha = alpha;
            diffuse.a = alpha;
            alphaMaterial.Value.setDiffuse(ref diffuse);
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
            if (subInterfaces != null)
            {
                foreach (TransparencySubInterface subInterface in subInterfaces)
                {
                    subInterface.setAlpha(currentAlpha);
                }
            }
        }

        public void smoothBlend(float targetOpacity)
        {
            changingOpacity = true;
            this.targetOpacity = targetOpacity;
        }

        internal void addSubInterface(TransparencySubInterface subInterface)
        {
            if (subInterfaces == null)
            {
                subInterfaces = new List<TransparencySubInterface>();
            }
            subInterfaces.Add(subInterface);
        }

        internal void removeSubInterface(TransparencySubInterface subInterface)
        {
            if (subInterfaces != null)
            {
                subInterfaces.Remove(subInterface);
            }
        }

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
                blacklist("Entity {0} only has {1} SubEntities. Index {3} is invalid.", entity.getName(), entity.getNumSubEntities(), subEntityIndex);
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
            TransparencyController.addTransparencyObject(this);
            setAlpha(currentAlpha);
        }

        protected override void destroy()
        {
            if (alphaMaterial != null)
            {
                alphaMaterial.Dispose();
                TransparencyController.removeTransparencyObject(this);
            }
            if (subInterfaces != null)
            {
                foreach (TransparencySubInterface subInterface in subInterfaces)
                {
                    subInterface._disconnectFromInterface();
                }
            }
        }

        public float CurrentAlpha
        {
            get
            {
                return currentAlpha;
            }
        }

        public bool DisableOnHidden
        {
            get
            {
                return disableOnHidden;
            }
            set
            {
                disableOnHidden = value;
            }
        }

        private const float opacityChangeRate = 1.0f;

        public override void update(Clock clock, EventManager eventManager)
        {
            if (changingOpacity)
            {
                if (currentAlpha > targetOpacity)
                {
                    currentAlpha -= (float)clock.Seconds * opacityChangeRate;
                    if (currentAlpha < targetOpacity)
                    {
                        currentAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                else
                {
                    currentAlpha += (float)clock.Seconds * opacityChangeRate;
                    if (currentAlpha > targetOpacity)
                    {
                        currentAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                setAlpha(currentAlpha);
            }
        }
    }
}
