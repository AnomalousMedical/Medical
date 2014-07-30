using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using Engine.Attributes;
using Logging;

namespace Medical
{
    public class Syringe : Behavior
    {
        public const String BehaviorName = "Behavior";
        public const String DefinitionName = "Syringe";

        [Editable] private String nodeName = PropFactory.NodeName;
        [Editable] private String entityName = PropFactory.EntityName;
        [Editable] private String plungerBoneName = "BonePlunger";
        private const float PLUNGE_RANGE = 5.1f;

        [DoNotCopy][DoNotSave] private bool doPlunge = false;
        [DoNotCopy][DoNotSave] private float plungeDuration = 1.0f;
        [DoNotCopy][DoNotSave] private float plungeRangePercent = 1.0f;
        private Bone plungerBone;
        [DoNotCopy][DoNotSave] private Vector3 plungerBoneStart;
        [DoNotCopy][DoNotSave] private float timeCounter = 0.0f;
        [DoNotCopy][DoNotSave] private float currentBonePercent = 0.0f;
        [DoNotCopy][DoNotSave] private float startingBonePercent = 0.0f;

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition syringe = new GenericSimObjectDefinition("Syringe");
            syringe.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Syringe.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            syringe.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            syringe.addElement(propFadeBehaviorDef);
            BehaviorDefinition syringeBehavior = new BehaviorDefinition(BehaviorName, new Syringe());
            syringe.addElement(syringeBehavior);
            propFactory.addDefinition(DefinitionName, syringe);

            ShowPropTrackInfo syringeData = new ShowPropTrackInfo();
            syringeData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            syringeData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            syringeData.addTrack(new ShowPropSubActionPrototype(typeof(PushPlungerAction), "Push Plunger"));
            propFactory.addTrackInfo(Syringe.DefinitionName, syringeData);
        }

        protected override void constructed()
        {
            base.constructed();
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Syringe SimObject", nodeName);
            }
            Entity entity = sceneNode.getNodeObject(entityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Syringe SimObject", entityName);
            }
            if (entity.hasSkeleton())
            {
                SkeletonInstance skeleton = entity.getSkeleton();
                if (skeleton.hasBone(plungerBoneName))
                {
                    plungerBone = skeleton.getBone(plungerBoneName);
                    plungerBone.setManuallyControlled(true);
                    plungerBoneStart = plungerBone.getPosition();
                }
                else
                {
                    blacklist("Could not find plunger bone {0} for syringe", plungerBoneName);
                }
            }
            else
            {
                blacklist("Entity has no skeleton in Syringe SimObject");
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (doPlunge)
            {
                timeCounter += clock.DeltaSeconds;
                float boneInterpolate = (timeCounter) / plungeDuration;
                if (boneInterpolate > plungeRangePercent)
                {
                    boneInterpolate = plungeRangePercent;
                    doPlunge = false;
                }

                currentBonePercent = startingBonePercent + boneInterpolate;
                if (currentBonePercent > 1.0f)
                {
                    currentBonePercent = 1.0f;
                    doPlunge = false;
                }

                Vector3 newPos = plungerBoneStart;
                newPos.y -= (PLUNGE_RANGE * currentBonePercent);
                plungerBone.setPosition(newPos);
                plungerBone.needUpdate(true);
            }
        }

        public void plunge(float percent, float duration)
        {
            if (duration <= 0.0f)
            {
                duration = 0.001f;
            }
            startingBonePercent = currentBonePercent;
            plungeRangePercent = percent;
            plungeDuration = duration;
            doPlunge = true;
            timeCounter = 0.0f;
        }

        public void setPlungePosition(float percent)
        {
            doPlunge = false;
            if (percent < 0.0f)
            {
                percent = 0.0f;
            }
            else if (percent > 1.0f)
            {
                percent = 1.0f;
            }
            currentBonePercent = percent;
            Vector3 newPos = plungerBoneStart;
            newPos.y -= (PLUNGE_RANGE * percent);
            plungerBone.setPosition(newPos);
            plungerBone.needUpdate(true);
        }
    }
}
