using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using OgreWrapper;

namespace Medical
{
    public class PoseableHand : Interface
    {
        public const String LeftDefinitionName = "PoseableHandLeft";
        public const String RightDefinitionName = "PoseableHandRight";
        public const String PoseableHandBehavior = "PoseableHandBehavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            createHand(propFactory, LeftDefinitionName, "HandPoseable.mesh", "");
            createHand(propFactory, RightDefinitionName, "HandPoseableRight.mesh", "Right");
        }

        private static void createHand(PropFactory propFactory, String definitionName, String meshName, String boneSuffix)
        {
            GenericSimObjectDefinition hand = new GenericSimObjectDefinition(definitionName);
            hand.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = meshName;

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            hand.addElement(nodeDefinition);

            PoseableHand poseableHand = new PoseableHand();
            poseableHand.BoneSuffix = boneSuffix;
            BehaviorDefinition poseableHandBehaviorDef = new BehaviorDefinition(PoseableHandBehavior, poseableHand);
            hand.addElement(poseableHandBehaviorDef);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            hand.addElement(propFadeBehaviorDef);

            DetachableSimObjectFollower detachableFollower = new DetachableSimObjectFollower();
            BehaviorDefinition detachableFollowerDef = new BehaviorDefinition(PropFactory.DetachableFollowerName, detachableFollower);
            hand.addElement(detachableFollowerDef);

            propFactory.addDefinition(definitionName, hand);
        }

        private Entity entity;
        private SubEntity subEntity;

        //Bones
        private Bone wrist;
        private Bone palm;
        private PoseableThumb thumb;
        private PoseableFinger index;
        private PoseableFinger middle;
        private PoseableFinger ring;
        private PoseableFinger pinky;

        public PoseableHand()
        {
            BoneSuffix = "";
        }

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in PoseableHand SimObject", PropFactory.NodeName);
            }
            entity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in PoseableHand SimObject", PropFactory.EntityName);
            }
            subEntity = entity.getSubEntity(0);
            if (subEntity == null)
            {
                blacklist("Could not find sub entity with index 0 in PoseableHand SimObject");
            }
            Skeleton skeleton = entity.getSkeleton();
            if (skeleton == null)
            {
                blacklist("Could not find skeleton for PoseableHand SimObject");
            }
            wrist = skeleton.getBone("bwrist" + BoneSuffix);
            if (wrist == null)
            {
                blacklist("Could not find bwrist in PoseableHand");
            }
            wrist.setManuallyControlled(true);
            palm = skeleton.getBone("bpalm" + BoneSuffix);
            if (palm == null)
            {
                blacklist("Could not find bpalm in PoseableHand");
            }
            palm.setManuallyControlled(true);
            thumb = new PoseableThumb(skeleton, "bthumbBase" + BoneSuffix, "bthumb02" + BoneSuffix, "bthumb01" + BoneSuffix);
            index = new PoseableFinger(skeleton, "bIndexknuckle" + BoneSuffix, "bIndex03" + BoneSuffix, "bIndex02" + BoneSuffix, "bIndex01" + BoneSuffix);
            middle = new PoseableFinger(skeleton, "bMiddleknuckle" + BoneSuffix, "bMiddle03" + BoneSuffix, "bMiddle02" + BoneSuffix, "bMiddle01" + BoneSuffix);
            ring = new PoseableFinger(skeleton, "bringknuckle" + BoneSuffix, "bring03" + BoneSuffix, "bring02" + BoneSuffix, "bring01" + BoneSuffix);
            pinky = new PoseableFinger(skeleton, "bpinkyknuckle" + BoneSuffix, "bpinky03" + BoneSuffix, "bpinky02" + BoneSuffix, "bpinky01" + BoneSuffix);
        }

        public PoseableThumb Thumb
        {
            get
            {
                return thumb;
            }
        }

        public PoseableFinger Index
        {
            get
            {
                return index;
            }
        }

        public PoseableFinger Middle
        {
            get
            {
                return middle;
            }
        }

        public PoseableFinger Ring
        {
            get
            {
                return ring;
            }
        }

        public PoseableFinger Pinky
        {
            get
            {
                return pinky;
            }
        }

        public String BoneSuffix { get; set; }
    }
}
