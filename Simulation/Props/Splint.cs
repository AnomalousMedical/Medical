using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using BulletPlugin;
using Engine.Attributes;

namespace Medical
{
    public class Splint : BehaviorInterface
    {
        public const String JointName = "Joint";
        public const String RigidBodyName = "RigidBody";
        public const String SplintBehaviorName = "SplintBehavior";

        [DoNotCopy]
        [DoNotSave]
        private bool jointLocked = false;

        [DoNotCopy]
        [DoNotSave]
        Vector3 offset;
        [DoNotCopy]
        [DoNotSave]
        Quaternion rotation;

        [DoNotCopy]
        [DoNotSave]
        Vector3 jointStartTranslation;
        [DoNotCopy]
        [DoNotSave]
        Quaternion jointStartRotation;

        private Generic6DofConstraintElement joint;
        private RigidBody rigidBody;

        protected override void constructed()
        {
            rigidBody = Owner.getElement(RigidBodyName) as RigidBody;
            if (rigidBody == null)
            {
                blacklist("Cannot find RigidBody '{0}'.", RigidBodyName);
            }

            joint = (Generic6DofConstraintElement)Owner.getElement(JointName);

            if (joint != null)
            {
                jointStartTranslation = joint.getFrameOffsetOriginA();
                jointStartRotation = joint.getFrameOffsetBasisA();
                rigidBody.raiseCollisionFlag(CollisionFlags.KinematicObject);
                moveOntoMandible();
            }

            JointLocked = true;
        }

        public void moveOntoMandible()
        {
            //Figure out relative starting position to the mandible
            Vector3 translationOffset = WorldStartTranslation - MandibleController.StartTranslation;
            translationOffset = Quaternion.quatRotate(MandibleController.StartRotation.inverse(), translationOffset);
            Quaternion rotationOffset = WorldStartRotation; //probably need * MandibleController.StartRotation.inverse()

            //Move to the current mandible position
            SimObject mandibleObject = MandibleController.Mandible.Owner;
            Vector3 trans = mandibleObject.Translation + Quaternion.quatRotate(mandibleObject.Rotation, translationOffset);
            Quaternion rotation = mandibleObject.Rotation * rotationOffset;
            updatePosition(ref trans, ref rotation);
        }

        public bool JointLocked
        {
            get
            {
                return jointLocked;
            }
            set
            {
                if (joint != null)
                {
                    jointLocked = value;
                    if (jointLocked)
                    {
                        rigidBody.clearCollisionFlag(CollisionFlags.KinematicObject);
                        SimObject other = joint.RigidBodyA.Owner;
                        offset = Quaternion.quatRotate(other.Rotation.inverse(), Owner.Translation - other.Translation) - jointStartTranslation;
                        rotation = other.Rotation.inverse() * Owner.Rotation * jointStartRotation.inverse();
                        joint.setFrameOffsetA(jointStartTranslation + offset);
                        joint.setFrameOffsetA(rotation * jointStartRotation);
                        joint.setLinearLowerLimit(Vector3.Zero);
                        joint.setLinearUpperLimit(Vector3.Zero);
                        joint.setAngularLowerLimit(Vector3.Zero);
                        joint.setAngularUpperLimit(Vector3.Zero);
                    }
                    else
                    {
                        rigidBody.raiseCollisionFlag(CollisionFlags.KinematicObject);
                        joint.setLinearLowerLimit(new Vector3(float.MinValue, float.MinValue, float.MinValue));
                        joint.setLinearUpperLimit(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
                        joint.setAngularLowerLimit(new Vector3(float.MinValue, float.MinValue, float.MinValue));
                        joint.setAngularUpperLimit(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
                    }
                }
            }
        }

        public Vector3 WorldStartTranslation { get; set; }

        public Quaternion WorldStartRotation { get; set; }

        public bool IsNull
        {
            get
            {
                return rigidBody.IsNull;
            }
        }
    }
}
