using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using OgreWrapper;
using Engine.Platform;
using Engine.Attributes;

namespace Medical
{
    public class Caliper : Behavior
    {
        public const String DefinitionName = "Caliper";
        public const String BehaviorName = "CaliperBehavior";

        private const float CALIPER_MAX_MM = 100;
        private const float PLUNGE_RANGE = 11.86f;//Shoud be able to do CALIPER_MAX_MM / SimulationConfig.UnitsToMM;, but this gives 11.8105, which is too small caliper may not be correct size, but close enough.

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition caliper = new GenericSimObjectDefinition(DefinitionName);
            caliper.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Caliper.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            caliper.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            caliper.addElement(propFadeBehaviorDef);

            Caliper caliperBehavior = new Caliper();
            BehaviorDefinition caliperBehaviorDef = new BehaviorDefinition(BehaviorName, caliperBehavior);
            caliper.addElement(caliperBehaviorDef);

            propFactory.addDefinition(DefinitionName, caliper);

            ShowPropTrackInfo caliperData = new ShowPropTrackInfo();
            caliperData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            caliperData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            caliperData.addTrack(new ShowPropSubActionPrototype(typeof(SetCaliperMeasurement), "Set Measurement"));
            propFactory.addTrackInfo(Caliper.DefinitionName, caliperData);
        }

        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        private Bone sliderBone;

        [DoNotCopy]
        [DoNotSave]
        private bool doPlunge = false;
        [DoNotCopy]
        [DoNotSave]
        private float plungeDuration = 1.0f;
        [DoNotCopy]
        [DoNotSave]
        private float plungeRangePercent = 1.0f;
        [DoNotCopy]
        [DoNotSave]
        private float timeCounter = 0.0f;
        [DoNotCopy]
        [DoNotSave]
        private float currentBonePercent = 0.0f;
        [DoNotCopy]
        [DoNotSave]
        private float startingBonePercent = 0.0f;
        private Vector3 caliperBoneStart;
        private AnimationState lockAnim;
        private AnimationState unlockAnim;
        private float lockAnimLength;
        private float unlockAnimLength;

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Caliper SimObject", PropFactory.NodeName);
            }
            
            entity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Caliper SimObject", PropFactory.EntityName);
            }
            
            subEntity = entity.getSubEntity(0);
            if (subEntity == null)
            {
                blacklist("Could not find sub entity with index 0 in Caliper SimObject");
            }
            
            Skeleton skeleton = entity.getSkeleton();
            if (skeleton == null)
            {
                blacklist("Could not find skeleton for Caliper SimObject");
            }

            sliderBone = skeleton.getBone("BCalSlider");
            if (sliderBone == null)
            {
                blacklist("Could not find 'BCalSlider' for Caliper SimObject");
            }
            sliderBone.setManuallyControlled(true);
            caliperBoneStart = sliderBone.getPosition();

            lockAnim = entity.getAnimationState("Lock");
            if (lockAnim == null)
            {
                blacklist("Could not find animation 'Lock' in caliper entity");
            }
            lockAnim.setLoop(false);
            lockAnimLength = lockAnim.getLength();
            lockAnim.setEnabled(true);
            lockAnim.setTimePosition(lockAnimLength); //The caliper starts unlocked, so set this animation active and at the end to keep it locked.

            unlockAnim = entity.getAnimationState("Unlock");
            if (unlockAnim == null)
            {
                blacklist("Could not find animation 'Unlock' in caliper entity");
            }
            unlockAnim.setLoop(false);
            unlockAnimLength = unlockAnim.getLength();
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (doPlunge)
            {
                timeCounter += clock.DeltaSeconds;

                if (unlockAnim.getEnabled())
                {
                    unlockAnim.addTime(clock.DeltaSeconds);
                    if (unlockAnim.hasEnded())
                    {
                        unlockAnim.setEnabled(false);
                    }
                }
                else if(lockAnim.getEnabled())
                {
                    lockAnim.addTime(clock.DeltaSeconds);
                    if (lockAnim.hasEnded())
                    {
                        doPlunge = false;
                    }
                }
                else
                {
                    float boneInterpolate = (timeCounter - unlockAnimLength) / plungeDuration;
                    if (boneInterpolate > plungeRangePercent)
                    {
                        boneInterpolate = plungeRangePercent;
                        lockAnim.setTimePosition(0.0f);
                        lockAnim.setEnabled(true);
                    }

                    currentBonePercent = startingBonePercent + boneInterpolate;
                    if (currentBonePercent > 1.0f)
                    {
                        currentBonePercent = 1.0f;
                    }

                    Vector3 newPos = caliperBoneStart;
                    newPos.y -= (PLUNGE_RANGE * currentBonePercent);
                    sliderBone.setPosition(newPos);
                    sliderBone.needUpdate(true);
                }
            }
        }

        public void moveToMeasurement(float millimeters, float duration)
        {
            float percent = millimeters / CALIPER_MAX_MM;

            if (duration <= 0.0f)
            {
                duration = 0.001f;
            }
            startingBonePercent = currentBonePercent;
            plungeRangePercent = percent;
            plungeDuration = duration - unlockAnimLength - lockAnimLength;
            doPlunge = true;
            timeCounter = 0.0f;
            unlockAnim.setTimePosition(0.0f);
            lockAnim.setEnabled(false);
            unlockAnim.setEnabled(true);
        }

        public void setMeasurement(float millimeters)
        {
            float percent = millimeters / CALIPER_MAX_MM;

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
            Vector3 newPos = caliperBoneStart;
            newPos.y -= (PLUNGE_RANGE * percent);
            sliderBone.setPosition(newPos);
            sliderBone.needUpdate(true);

            unlockAnim.setEnabled(false);
            lockAnim.setEnabled(true);
            lockAnim.setTimePosition(lockAnimLength); //The caliper starts unlocked, so set this animation active and at the end to keep it locked.
        }
    }
}
