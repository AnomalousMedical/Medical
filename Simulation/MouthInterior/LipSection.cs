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
using Logging;

namespace Medical
{
    public abstract class LipSection : Behavior
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

        [DoNotCopy]
        [DoNotSave]
        private Vector3 originalPosition;

        [DoNotCopy]
        [DoNotSave]
        protected List<Tooth> collidingTeeth = new List<Tooth>(5);

        protected override void constructed()
        {
            joint = Owner.getElement(jointName) as Generic6DofConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }

            originalPosition = joint.getFrameOffsetOriginA();

            rigidBody = Owner.getElement(rigidBodyName) as RigidBody;
            if (rigidBody == null)
            {
                blacklist("Could not find RigidBody {0}", rigidBodyName);
            }
            rigidBody.ContactStarted += new CollisionCallback(rigidBody_ContactStarted);
            rigidBody.ContactEnded += new CollisionCallback(rigidBody_ContactEnded);

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
                if (value)
                {
                    SimObject other = joint.RigidBodyA.Owner;
                    Vector3 offset = Quaternion.quatRotate(other.Rotation.inverse(), Owner.Translation - other.Translation);
                    joint.setFrameOffsetA(offset);

                    joint.setLinearLowerLimit(Vector3.Zero);
                    joint.setLinearUpperLimit(Vector3.Zero);
                    joint.setAngularLowerLimit(Vector3.Zero);
                    joint.setAngularUpperLimit(Vector3.Zero);
                }
                else
                {
                    joint.setLinearLowerLimit(new Vector3(-1.0f, -1.0f, -1.0f));
                    joint.setLinearUpperLimit(new Vector3(1.0f, 1.0f, 1.0f));
                    //joint.setAngularLowerLimit(new Vector3(-3.14f, -3.14f, -3.14f));
                    //joint.setAngularUpperLimit(new Vector3(3.14f, 3.14f, 3.14f));
                }
            }
        }

        [DoNotCopy]
        public float Damping
        {
            get
            {
                return rigidBody.getLinearDamping();
            }
            set
            {
                rigidBody.setDamping(value, rigidBody.getAngularDamping());
            }
        }

        public void setOriginalPosition()
        {
            joint.setFrameOffsetA(originalPosition);
        }

        void rigidBody_ContactStarted(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            Tooth otherTooth = otherBody.Owner.getElement("Behavior") as Tooth;
            if (otherTooth != null)
            {
                if (collidingTeeth.Count == 0)
                {
                    rigidBody.setDamping(0.0f, 0.0f);
                    //Log.Debug("Lip section {0} damping set to 0.", Owner.Name);
                }
                collidingTeeth.Add(otherTooth);
            }
        }

        void rigidBody_ContactEnded(ContactInfo contact, RigidBody sourceBody, RigidBody otherBody, bool isBodyA)
        {
            Tooth otherTooth = otherBody.Owner.getElement("Behavior") as Tooth;
            if (otherTooth != null)
            {
                collidingTeeth.Remove(otherTooth);
                if (collidingTeeth.Count == 0)
                {
                    rigidBody.setDamping(1.0f, 1.0f);
                    //Log.Debug("Lip section {0} damping set to 1.", Owner.Name);
                }
            }
        }
    }
}
