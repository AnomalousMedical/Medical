using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using BulletPlugin;
using Engine.Editing;
using Engine.Platform;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;
using Engine.Attributes;

namespace Medical
{
    public class LipSection : Behavior
    {
        [Editable]
        private String jointName = "Joint";

        [Editable]
        private String rigidBodyName = "Actor";

        [Editable]
        private String skinSimObjectName = "Skin";

        [Editable]
        private String skinNodeName = "Node";

        [Editable]
        private String skinEntityName = "Entity";

        [Editable]
        private String lipBoneName = "";

        private Generic6DofConstraintElement joint;
        private RigidBody rigidBody;
        private Bone lipBone;
        private SimObject skinObject;
        private bool lipsRigid = true;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 offset;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 lastPosition;

        protected override void constructed()
        {
            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }

            rigidBody = Owner.getElement(rigidBodyName) as RigidBody;
            if (rigidBody == null)
            {
                blacklist("Could not find RigidBody {0}", rigidBodyName);
            }

            skinObject = Owner.getOtherSimObject(skinSimObjectName);
            if (skinObject == null)
            {
                blacklist("Could not find Target skin SimObject {0}.", skinSimObjectName);
            }

            SceneNodeElement node = skinObject.getElement(skinNodeName) as SceneNodeElement;
            if (node == null)
            {
                blacklist("Could not find target skin SceneNodeElement {0}.", skinSimObjectName);
            }

            Entity entity = node.getNodeObject(skinEntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find skin Entity {0}.", skinNodeName);
            }

            if (!entity.hasSkeleton())
            {
                blacklist("Skin Entity {0} does not have a skeleton.", skinEntityName);
            }

            SkeletonInstance skeleton = entity.getSkeleton();
            if (skeleton.hasBone(lipBoneName))
            {
                lipBone = skeleton.getBone(lipBoneName);
                lipBone.setManuallyControlled(true);
                offset = lipBone.getPosition() + skinObject.Translation - this.Owner.Translation;
                lastPosition = this.Owner.Translation;
            }
            else
            {
                blacklist("Cannot find bone {0} in Skin Entity {1} in SimObject {2}", lipBoneName, skinEntityName, skinSimObjectName);
            }
        }

        protected override void link()
        {
            LipController.addCollisionSection(this);
        }

        protected override void destroy()
        {
            LipController.removeTongueCollisionSection(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (Owner.Translation != lastPosition)
            {
                lastPosition = Owner.Translation;
                lipBone.setPosition(lastPosition + offset - skinObject.Translation);
                lipBone.needUpdate(true);
            }
        }

        [DoNotCopy]
        public bool CollisionEnabled
        {
            get
            {
                return (rigidBody.getCollisionFlags() & CollisionFlags.NoContactResponse) == 0;
            }
            set
            {
                if (value)
                {
                    rigidBody.setCollisionFlags(rigidBody.getCollisionFlags() & ~CollisionFlags.NoContactResponse);
                }
                else
                {
                    rigidBody.setCollisionFlags(rigidBody.getCollisionFlags() | CollisionFlags.NoContactResponse);
                }
            }
        }

        [DoNotCopy]
        public bool LipsRigid
        {
            get
            {
                return lipsRigid;
            }
            set
            {
                lipsRigid = value;
                if (lipsRigid)
                {
                    SimObject other = joint.RigidBodyA.Owner;
                    Vector3 offset = Quaternion.quatRotate(other.Rotation.inverse(), Owner.Translation - other.Translation);
                    joint.setFrameOffsetA(offset);

                    Quaternion rotation = other.Rotation.inverse() * Owner.Rotation;
                    joint.setFrameOffsetA(rotation);

                    joint.setLinearLowerLimit(Vector3.Zero);
                    joint.setLinearUpperLimit(Vector3.Zero);
                    joint.setAngularLowerLimit(Vector3.Zero);
                    joint.setAngularUpperLimit(Vector3.Zero);
                }
                else
                {
                    joint.setLinearLowerLimit(new Vector3(-1.0f, -1.0f, -1.0f));
                    joint.setLinearUpperLimit(new Vector3(1.0f, 1.0f, 1.0f));
                    joint.setAngularLowerLimit(new Vector3(-3.14f, -3.14f, -3.14f));
                    joint.setAngularUpperLimit(new Vector3(3.14f, 3.14f, 3.14f));
                }
            }
        }
    }
}
