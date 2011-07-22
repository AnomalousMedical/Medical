using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using BulletPlugin;
using Engine.Attributes;

namespace Medical
{
    class Splint : Interface
    {
        private const short TOP_TEETH_SPLINT_FILTER = ~2;
        private const short BOTTOM_TEETH_SPLINT_FILTER = ~4;

        private const String JointName = "Joint";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition splitDefinition;

            splitDefinition = createSplitDefinition("NTI", "NTI.mesh", "NTICollision", TOP_TEETH_SPLINT_FILTER, 0.0f);
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);

            splitDefinition = createSplitDefinition("PivotAppliance", "PivotAppliance.mesh", "PivotApplianceCollision", BOTTOM_TEETH_SPLINT_FILTER, 1.0f);
            jointToMandible(splitDefinition);
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
        }

        private static GenericSimObjectDefinition createSplitDefinition(String definitionName, String meshName, String collisionName, short mask, float mass)
        {
            GenericSimObjectDefinition splint = new GenericSimObjectDefinition(definitionName);
            splint.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = meshName;

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            splint.addElement(nodeDefinition);

            RigidBodyDefinition rigidBody = new RigidBodyDefinition(PropFactory.RigidBodyName);
            rigidBody.ShapeName = collisionName;
            rigidBody.Mass = mass;
            rigidBody.CollisionFilterMask = mask;
            splint.addElement(rigidBody);

            Splint splintBehavior = new Splint();
            BehaviorDefinition splintBehaviorDef = new BehaviorDefinition("SplintBehavior", splintBehavior);
            splint.addElement(splintBehaviorDef);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            splint.addElement(propFadeBehaviorDef);

            return splint;
        }

        public static GenericSimObjectDefinition jointToMandible(GenericSimObjectDefinition splint)
        {
            Generic6DofConstraintDefinition joint = new Generic6DofConstraintDefinition(JointName);
            joint.RigidBodyASimObject = "Mandible";
            joint.RigidBodyAElement = "Actor";
            joint.RigidBodyBSimObject = "this";
            joint.RigidBodyBElement = PropFactory.RigidBodyName;
            joint.TranslationMotor.LowerLimit = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            joint.TranslationMotor.UpperLimit = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            joint.XRotMotor.LoLimit = float.MinValue;
            joint.XRotMotor.HiLimit = float.MaxValue;
            joint.YRotMotor.LoLimit = float.MinValue;
            joint.YRotMotor.HiLimit = float.MaxValue;
            joint.ZRotMotor.LoLimit = float.MinValue;
            joint.ZRotMotor.HiLimit = float.MaxValue;

            splint.addElement(joint);

            return splint;
        }

        [DoNotCopy][DoNotSave] private bool jointLocked = false;

        [DoNotCopy][DoNotSave] Vector3 offset;
        [DoNotCopy][DoNotSave] Quaternion rotation;

        [DoNotCopy][DoNotSave] Vector3 startingLocation;
        [DoNotCopy][DoNotSave] Quaternion startingRotation;

        private Generic6DofConstraintElement joint;
        private RigidBody rigidBody;

        protected override void constructed()
        {
            rigidBody = Owner.getElement(PropFactory.RigidBodyName) as RigidBody;
            if (rigidBody == null)
            {
                blacklist("Cannot find RigidBody '{0}'.", PropFactory.RigidBodyName);
            }

            joint = (Generic6DofConstraintElement)Owner.getElement(JointName);

            if (joint != null)
            {
                startingLocation = joint.getFrameOffsetOriginA();
                startingRotation = joint.getFrameOffsetBasisA();
                rigidBody.raiseCollisionFlag(CollisionFlags.KinematicObject);
            }

            JointLocked = true;
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
                        offset = Quaternion.quatRotate(other.Rotation.inverse(), Owner.Translation - other.Translation) - startingLocation;
                        rotation = other.Rotation.inverse() * Owner.Rotation * startingRotation.inverse();
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
    }
}
