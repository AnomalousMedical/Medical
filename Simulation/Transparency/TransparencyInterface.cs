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
        TMJ,
        MouthInterior,
        Nerves,
        ShoulderGirdle,
    }

    public class TransparencyInterface : Behavior
    {
        [Editable] private String alphaSuffix = "Alpha";
        //An optional alpha material, if this is defined the alpha suffix will be ignored
        //and this material will be used instead.
        [Editable] private String alphaMaterialName = null;
        [Editable] private String nodeName;
        [Editable] private String childNodeName;
        [Editable] private String entityName;
        [Editable] private bool disableOnHidden = true;
        [Editable] private int renderGroupOffset = 0;
        [Editable] private uint subEntityIndex = 0;

        [Editable] public String ObjectName { get; private set; }
        [Editable] public RenderGroup RenderGroup { get; private set; }

        Entity entity;

        SubEntity subEntity;

        [DoNotSave]
        String baseMaterialName;
        [DoNotSave]
        Quaternion alphaQuat = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        [DoNotCopy]
        [DoNotSave]
        private List<TransparencySubInterface> subInterfaces;

        [DoNotCopy]
        [DoNotSave]
        private List<TransparencyState> transparencyStates;

        [DoNotCopy]
        [DoNotSave]
        private int activeTransparencyState = 0;

        [DoNotCopy]
        [DoNotSave]
        private String finalAlphaMaterialName;

        public TransparencyInterface()
        {
            ObjectName = "";
            RenderGroup = RenderGroup.None;
        }

        public void smoothBlend(float targetOpacity, float changeMultiplier)
        {
            transparencyStates[activeTransparencyState].smoothBlend(targetOpacity, changeMultiplier);
        }

        public void timedBlend(float targetOpacity, float time)
        {
            float currentAlpha = transparencyStates[activeTransparencyState].WorkingAlpha;
            float delta = Math.Abs(targetOpacity - currentAlpha);
            if (delta != 0.0f)
            {
                float changeMultiplier = delta / time;
                smoothBlend(targetOpacity, changeMultiplier);
            }
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

        internal void createTransparencyState()
        {
            transparencyStates.Add(new TransparencyState());
        }

        internal void removeTransparencyState(int index)
        {
            transparencyStates.RemoveAt(index);
            if (activeTransparencyState >= index)
            {
                --activeTransparencyState;
            }
        }

        internal void applyTransparencyState(int index)
        {
            float workingAlpha = transparencyStates[index].WorkingAlpha;
            if (workingAlpha != alphaQuat.w)
            {
                applyAlphaToMaterial(workingAlpha);
            }
        }

        protected override void constructed()
        {
            transparencyStates = new List<TransparencyState>();
            transparencyStates.Add(new TransparencyState());

            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode != null)
            {
                if (childNodeName != null && childNodeName != String.Empty)
                {
                    sceneNode = sceneNode.findChildNode(childNodeName);
                    if (sceneNode == null)
                    {
                        blacklist("Could not find child node {0}.", childNodeName);
                    }
                }
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
            TransparencyController.addTransparencyObject(this);

            applyAlphaToMaterial(transparencyStates[activeTransparencyState].WorkingAlpha);
        }

        protected override void destroy()
        {
            TransparencyController.removeTransparencyObject(this);
            if (subInterfaces != null)
            {
                foreach (TransparencySubInterface subInterface in subInterfaces)
                {
                    subInterface._disconnectFromInterface();
                }
            }
        }

        /// <summary>
        /// The alpha value for this interface. Note that if it is currently in
        /// transition the target opacity will be returned and not the actual
        /// current value.
        /// </summary>
        [DoNotCopy]
        public float CurrentAlpha
        {
            get
            {
                return transparencyStates[activeTransparencyState].CurrentAlpha;
            }
            set
            {
                transparencyStates[activeTransparencyState].CurrentAlpha = value;
            }
        }

        /// <summary>
        /// This will override the working alpha for the active transparency state.
        /// </summary>
        [DoNotCopy]
        public float OverrideAlpha
        {
            get
            {
                return transparencyStates[activeTransparencyState].OverrideAlpha;
            }
            set
            {
                transparencyStates[activeTransparencyState].OverrideAlpha = value;
            }
        }

        public void clearOverrideAlpha()
        {
            transparencyStates[activeTransparencyState].clearOverrideAlpha();
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

        [DoNotCopy]
        internal int ActiveTransparencyState
        {
            get
            {
                return activeTransparencyState;
            }
            set
            {
                activeTransparencyState = value;
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            foreach (TransparencyState state in transparencyStates)
            {
                state.update(clock);
            }
        }

        private void applyAlphaToMaterial(float alpha)
        {
            alphaQuat.w = alpha;
            subEntity.setCustomParameter(0, alphaQuat);
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
                subEntity.setMaterialName(finalAlphaMaterialName);
                switch (RenderGroup)
                {
                    case RenderGroup.None:
                        entity.setRenderQueueGroup((byte)(95 + renderGroupOffset));
                        break;
                    case RenderGroup.Teeth:
                        entity.setRenderQueueGroup((byte)(0 + renderGroupOffset));
                        break;
                    case RenderGroup.Bones:
                        entity.setRenderQueueGroup((byte)(70 + renderGroupOffset));
                        break;
                    case RenderGroup.Muscles:
                        entity.setRenderQueueGroup((byte)(70 + renderGroupOffset));
                        break;
                    case RenderGroup.Nerves:
                        entity.setRenderQueueGroup((byte)(70 + renderGroupOffset));
                        break;
                    case RenderGroup.Skin:
                        entity.setRenderQueueGroup((byte)(90 + renderGroupOffset));
                        break;
                    case RenderGroup.Spine:
                        entity.setRenderQueueGroup((byte)(70 + renderGroupOffset));
                        break;
                    case RenderGroup.Nasal:
                        entity.setRenderQueueGroup((byte)(70 + renderGroupOffset));
                        break;
                    case RenderGroup.TMJ:
                        entity.setRenderQueueGroup((byte)(60 + renderGroupOffset));
                        break;
                }
            }
            if (subInterfaces != null)
            {
                foreach (TransparencySubInterface subInterface in subInterfaces)
                {
                    subInterface.setAlpha(alpha);
                }
            }
        }
    }
}
