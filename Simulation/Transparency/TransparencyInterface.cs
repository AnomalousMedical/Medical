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

    public class TransparencyInterface : Interface
    {
        [Editable] private String alphaSuffix = "Alpha";
        //An optional alpha material, if this is defined the alpha suffix will be ignored
        //and this material will be used instead.
        [Editable] private String alphaMaterialName = null;
        [Editable] private float currentAlpha = 1.0f;
        [Editable] private String nodeName;
        [Editable] private String entityName;

        [Editable] public String ObjectName { get; private set; }
        [Editable] public RenderGroup RenderGroup { get; private set; }

        Entity entity;

        MaterialPtr alphaMaterial;
        SubEntity subEntity;

        [DoNotSave]
        String baseMaterialName;
        [DoNotSave]
        Color diffuse;

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
            entity.setVisible(alpha != 0.0f);
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
                        entity.setRenderQueueGroup(255);
                        break;
                    case RenderGroup.Teeth:
                        entity.setRenderQueueGroup(0);
                        break;
                    case RenderGroup.Bones:
                        entity.setRenderQueueGroup(20);
                        break;
                    case RenderGroup.Muscles:
                        entity.setRenderQueueGroup(20);
                        break;
                    case RenderGroup.Skin:
                        entity.setRenderQueueGroup(200);
                        break;
                    case RenderGroup.Spine:
                        entity.setRenderQueueGroup(20);
                        break;
                    case RenderGroup.Nasal:
                        entity.setRenderQueueGroup(20);
                        break;
                    case RenderGroup.TMJ:
                        entity.setRenderQueueGroup(10);
                        break;
                }
            }
        }

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode != null)
            {
                entity = sceneNode.getNodeObject(entityName) as Entity;
            }
            bool valid = entity != null;
            if (valid)
            {
                subEntity = entity.getSubEntity(0);
                baseMaterialName = subEntity.getMaterialName();
                MaterialManager materialManager = MaterialManager.getInstance();
                bool useDefinedMaterial = !(alphaMaterialName == null || alphaMaterialName.Equals(String.Empty));
                if (useDefinedMaterial)
                {
                    valid &= materialManager.resourceExists(alphaMaterialName);
                    if (valid)
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
                    valid &= materialManager.resourceExists(baseMaterialName + alphaSuffix);
                    if (valid)
                    {
                        alphaMaterial = materialManager.getByName(baseMaterialName + alphaSuffix);
                    }
                    else
                    {
                        blacklist("Cannot find automatic alpha material {0}.  Please ensure one exists or define a custom alpha behavior.", baseMaterialName + alphaSuffix);
                    }
                }
                if (valid)
                {
                    subEntity.setMaterialName(alphaMaterial.Value.getName());
                    diffuse = alphaMaterial.Value.getTechnique(0).getPass("Color").getDiffuse();
                    TransparencyController.addTransparencyObject(this);
                    setAlpha(currentAlpha);
                }
            }
            else
            {
                blacklist("No entity specified or entity is not found.");
            }
        }

        protected override void destroy()
        {
            if (alphaMaterial != null)
            {
                alphaMaterial.Dispose();
                TransparencyController.removeTransparencyObject(this);
            }
        }

        public float CurrentAlpha
        {
            get
            {
                return currentAlpha;
            }
        }
    }
}
