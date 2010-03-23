using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Platform;
using Engine.ObjectManagement;
using OgreWrapper;
using OgrePlugin;
using Engine.Attributes;
using BulletPlugin;

namespace Medical
{
    public class TongueCollisionSection : Behavior
    {
        [Editable]
        private String tongueSimObjectName = "Tongue";

        [Editable]
        private String tongueNodeName = "Node";

        [Editable]
        private String tongueEntityName = "Entity";

        [Editable]
        private String boneName = "TongueBone5a";

        [Editable]
        private String jointName = "Joint";

        private Bone bone;
        protected Generic6DofConstraintElement joint;
        SimObject tongueObject;

        [DoNotSave]
        [DoNotCopy]
        Vector3 offset;

        [DoNotSave]
        [DoNotCopy]
        Vector3 lastPosition;

        protected override void constructed()
        {
            tongueObject = Owner.getOtherSimObject(tongueSimObjectName);
            if (tongueObject == null)
            {
                blacklist("Could not find Target tongue SimObject {0}.", tongueSimObjectName);
            }

            SceneNodeElement node = tongueObject.getElement(tongueNodeName) as SceneNodeElement;
            if (node == null)
            {
                blacklist("Could not find target tongue SceneNodeElement {0}.", tongueSimObjectName);
            }

            Entity entity = node.getNodeObject(tongueEntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find tongue Entity {0}.", tongueNodeName);
            }
            if (!entity.hasSkeleton())
            {
                blacklist("Tongue Entity {0} does not have a skeleton.", tongueEntityName);
            }

            SkeletonInstance skeleton = entity.getSkeleton();
            bone = skeleton.getBone(boneName);
            if (bone == null)
            {
                blacklist("Tongue Entity {0} does not have a bone named {1}.", tongueEntityName, boneName);
            }

            offset = Owner.Translation - tongueObject.Translation - bone.getDerivedPosition();
            //calculate the scale at 100%
            Vector3 adjust = (Vector3.ScaleIdentity - Owner.Scale) * offset;
            offset += adjust;

            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 bonePos = bone.getDerivedPosition();
            if (bonePos != lastPosition)
            {
                Vector3 jointPos = tongueObject.Translation + bonePos + offset * Owner.Scale - joint.RigidBodyA.Owner.Translation;
                joint.setFrameOffsetA(jointPos);
                lastPosition = bonePos;
            }
        }
    }
}
