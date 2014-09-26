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
    public class PoseableHand : BehaviorInterface
    {
        public const String PoseableHandBehavior = "PoseableHandBehavior";

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
            wrist = skeleton.getBone("BWrist" + BoneSuffix);
            if (wrist == null)
            {
                blacklist("Could not find bwrist in PoseableHand");
            }
            wrist.setManuallyControlled(true);
            palm = skeleton.getBone("BPalm" + BoneSuffix);
            if (palm == null)
            {
                blacklist("Could not find bpalm in PoseableHand");
            }
            palm.setManuallyControlled(true);
            thumb = new PoseableThumb(skeleton, "BThumbBase" + BoneSuffix, "BThumb02" + BoneSuffix, "BThumb01" + BoneSuffix);
            index = new PoseableFinger(skeleton, "BIndexKnuckle" + BoneSuffix, "BIndex03" + BoneSuffix, "BIndex02" + BoneSuffix, "BIndex01" + BoneSuffix);
            middle = new PoseableFinger(skeleton, "BMiddleKnuckle" + BoneSuffix, "BMiddle03" + BoneSuffix, "BMiddle02" + BoneSuffix, "BMiddle01" + BoneSuffix);
            ring = new PoseableFinger(skeleton, "BRingKnuckle" + BoneSuffix, "BRing03" + BoneSuffix, "BRing02" + BoneSuffix, "BRing01" + BoneSuffix);
            pinky = new PoseableFinger(skeleton, "BPinkyKnuckle" + BoneSuffix, "BPinky03" + BoneSuffix, "BPinky02" + BoneSuffix, "BPinky01" + BoneSuffix);
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
