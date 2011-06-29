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
    class PoseableHand : Behavior
    {
        public const String DefinitionName = "PoseableHand";
        public const String PoseableHandBehavior = "PoseableHandBehavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition hand = new GenericSimObjectDefinition(DefinitionName);
            hand.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "HandPoseable.mesh";
            
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            hand.addElement(nodeDefinition);

            PoseableHand poseableHand = new PoseableHand();
            BehaviorDefinition poseableHandBehaviorDef = new BehaviorDefinition(PoseableHandBehavior, poseableHand);
            hand.addElement(poseableHandBehaviorDef);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            hand.addElement(propFadeBehaviorDef);

            propFactory.addDefinition(DefinitionName, hand);
        }

        private Entity entity;
        private SubEntity subEntity;

        //Bones
        private Bone wrist;
        private Bone palm;
        private PoseableFinger thumb;
        private PoseableFinger index;
        private PoseableFinger middle;
        private PoseableFinger ring;
        private PoseableFinger pinky;

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
            wrist = skeleton.getBone("bwrist");
            if (wrist == null)
            {
                blacklist("Could not find bwrist in PoseableHand");
            }
            wrist.setManuallyControlled(true);
            palm = skeleton.getBone("bpalm");
            if (palm == null)
            {
                blacklist("Could not find bpalm in PoseableHand");
            }
            palm.setManuallyControlled(true);
            thumb = new PoseableFinger(skeleton, "bthumbBase", "bthumb02", "bthumb01");
            index = new PoseableFinger(skeleton, "bIndexknuckle", "bIndex03", "bIndex02", "bIndex01");
            middle = new PoseableFinger(skeleton, "bMiddleknuckle", "bMiddle03", "bMiddle02", "bMiddle01");
            ring = new PoseableFinger(skeleton, "bringknuckle", "bring03", "bring02", "bring01");
            pinky = new PoseableFinger(skeleton, "bpinkyknuckle", "bpinky03", "bpinky02", "bpinky01");
        }

        public override void update(Engine.Platform.Clock clock, Engine.Platform.EventManager eventManager)
        {
            //thumb.Metacarpal.Yaw += (Degree)(10 * clock.fSeconds);
            //index.Metacarpal.Yaw += (Degree)(10 * clock.fSeconds);
            //middle.Metacarpal.Yaw += (Degree)(10 * clock.fSeconds);
            //ring.Metacarpal.Yaw += (Degree)(10 * clock.fSeconds);
            //pinky.Metacarpal.Yaw += (Degree)(10 * clock.fSeconds);

            //thumb.ProximalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //index.ProximalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //middle.ProximalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //ring.ProximalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //pinky.ProximalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);

            //index.MiddlePhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //middle.MiddlePhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //ring.MiddlePhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            //pinky.MiddlePhalanges.Yaw += (Degree)(10 * clock.fSeconds);

            thumb.DistalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            index.DistalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            middle.DistalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            ring.DistalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);
            pinky.DistalPhalanges.Yaw += (Degree)(10 * clock.fSeconds);

            //thumb.DistalPhalanges.Pitch += (Degree)(10 * clock.fSeconds);
            //index.DistalPhalanges.Pitch += (Degree)(10 * clock.fSeconds);
            //middle.DistalPhalanges.Pitch += (Degree)(10 * clock.fSeconds);
            //ring.DistalPhalanges.Pitch += (Degree)(10 * clock.fSeconds);
            //pinky.DistalPhalanges.Pitch += (Degree)(10 * clock.fSeconds);
        }
    }
}
