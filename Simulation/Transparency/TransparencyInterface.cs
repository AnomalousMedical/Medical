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
        Airway,
        MAX,
    }

    static class RenderGroupQueue
    {
        private static byte[] queueGroups = new byte[(int)RenderGroup.MAX];
        static RenderGroupQueue()
        {
            queueGroups[(int)RenderGroup.None] = 95;
            queueGroups[(int)RenderGroup.Teeth] = 60;
            queueGroups[(int)RenderGroup.Bones] = 70;
            queueGroups[(int)RenderGroup.Muscles] = 70;
            queueGroups[(int)RenderGroup.Skin] = 90;
            queueGroups[(int)RenderGroup.Spine] = 70;
            queueGroups[(int)RenderGroup.Nasal] = 70;
            queueGroups[(int)RenderGroup.TMJ] = 60;
            queueGroups[(int)RenderGroup.MouthInterior] = 60;
            queueGroups[(int)RenderGroup.Nerves] = 70;
            queueGroups[(int)RenderGroup.ShoulderGirdle] = 60;
            queueGroups[(int)RenderGroup.Airway] = 60;
        }

        public static byte GetQueue(RenderGroup group)
        {
            return queueGroups[(int)group];
        }

        public static byte GetQueue(RenderGroup group, byte offset)
        {
            return (byte)(queueGroups[(int)group] + offset);
        }
    }

    public enum TransparencyStatus
    {
        Solid,
        Transparent,
        Hidden
    }

    public class TransparencyInterface : Behavior
    {
        [Editable] private String alphaSuffix = "Alpha";
        //An optional alpha material, if this is defined the alpha suffix will be ignored
        //and this material will be used instead.
        [Editable] private String alphaMaterialName = null;
        [Editable] private String hiddenMaterialName = null;
        [Editable] private String nodeName;
        [Editable] private String childNodeName;
        [Editable] private String entityName;
        [Editable] private bool disableOnHidden = true;
        [Editable] private int renderGroupOffset = 0;
        [Editable] private uint subEntityIndex = 0;
        [Editable] private bool useDepthCheck = false;

        [Editable] public String ObjectName { get; private set; }
        [Editable] public RenderGroup RenderGroup { get; private set; }

        Entity entity;

        SubEntity subEntity;

        [DoNotCopy]
        [DoNotSave]
        TransparencyStatus status = TransparencyStatus.Solid;

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

        [DoNotCopy]
        [DoNotSave]
        private TransparencyOverrider transparencyOverrider;

        public TransparencyInterface()
        {
            ObjectName = "";
            RenderGroup = RenderGroup.None;
        }

        public void timedBlend(float targetOpacity, float time)
        {
            float currentAlpha = transparencyStates[activeTransparencyState].WorkingAlphaOnly;
            float delta = Math.Abs(targetOpacity - currentAlpha);

            float changeMultiplier = 1000.0f;
            if (delta != 0.0f && time > 0.0f)
            {
                changeMultiplier = delta / time;
            }
            transparencyStates[activeTransparencyState].smoothBlend(targetOpacity, changeMultiplier);
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
            float workingAlpha = getCurrentTransparency(index);
            if (workingAlpha != alphaQuat.w)
            {
                applyAlphaToMaterial(workingAlpha);
            }
        }

        internal void setTransparencyOverrider(TransparencyOverrider overrider)
        {
            if (transparencyOverrider == null)
            {
                transparencyOverrider = overrider;
            }
            else
            {
                throw new Exception(String.Format("{0} already has a transparency overrider.", Name));
            }
        }

        internal void clearTransparencyOverrider(TransparencyOverrider overrider)
        {
            if (transparencyOverrider == overrider)
            {
                transparencyOverrider = null;
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

            applyAlphaToMaterial(getCurrentTransparency(activeTransparencyState));
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

        public float getCurrentTransparency(int transparencyStateIndex)
        {
            float workingAlpha = transparencyStates[transparencyStateIndex].WorkingAlpha;
            if (transparencyOverrider != null)
            {
                return transparencyOverrider.getOverrideTransparency(workingAlpha, transparencyStateIndex);
            }
            return workingAlpha;
        }

        private void applyAlphaToMaterial(float alpha)
        {
            alphaQuat.w = alpha;
            subEntity.setCustomParameter(0, alphaQuat);
            if (alpha == 1.0f)
            {
                if (status != TransparencyStatus.Solid)
                {
                    status = TransparencyStatus.Solid;
                    subEntity.setMaterialName(baseMaterialName);
                    entity.setRenderQueueGroup(0);
                    subEntity.setVisible(true);
                    entity.setMaterialLodBias(1.0f, 0, 0);
                }
            }
            else if (alpha == 0.0f)
            {
                if (status != TransparencyStatus.Hidden)
                {
                    status = TransparencyStatus.Hidden;
                    if (disableOnHidden)
                    {
                        subEntity.setVisible(false);
                    }
                    else if (hiddenMaterialName != null)
                    {
                        entity.setMaterialLodBias(1.0f, 0, 0);
                        subEntity.setMaterialName(hiddenMaterialName);
                        entity.setRenderQueueGroup(0);
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
                    if (useDepthCheck)
                    {
                        entity.setMaterialLodBias(1.0f, 0, 0);
                    }
                    else
                    {
                        entity.setMaterialLodBias(1.0f, 1, 1);
                    }
                    entity.setRenderQueueGroup(RenderGroupQueue.GetQueue(RenderGroup, (byte)renderGroupOffset));
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
