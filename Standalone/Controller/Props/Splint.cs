using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using BulletPlugin;
using Engine.Attributes;
using System.Xml;
using Logging;

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

            //-------Maxillary------
            //MandibularFullCoverage
            splitDefinition = createSplitDefinition("MandibularFullCoverage", "MandibularFullCoverage.mesh", "MandibularFullCoverageCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-2.14577E-06f, -10.1296f, 8.09898f), new Quaternion(1.490118E-08f, -4.511825E-08f, 6.723153E-16f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //BoChin4Skin
            splitDefinition = createSplitDefinition("BoChin4Skin", "BoChin4Skin.mesh", "BoChin4SkinCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-4.74276E-07f, -12.5463f, 10.8493f), new Quaternion(-4.56956E-08f, 3.975241E-08f, -3.97524E-08f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibleGums
            splitDefinition = createSplitDefinition("MandibleGums", "MandibleGums.mesh", "MandibleGumsCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-1.66893E-06f, -11.1689f, 7.81636f), new Quaternion(7.89747E-15f, -1.986665E-07f, -3.97524E-08f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //BoneVeinsArteriesMove
            splitDefinition = createSplitDefinition("BoneVeinsArteriesMove", "BoneVeinsArteriesMove.mesh", "BoneVeinsArteriesMoveCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(1.07722E-06f, -11.6201f, 6.01497f), new Quaternion(7.35137E-08f, 3.975241E-08f, 7.94569E-08f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //RightSubmandibularSublingualGland
            splitDefinition = createSplitDefinition("RightSubmandibularSublingualGland", "RightSubmandibularSublingualGland.mesh", "RightSubmandibularSublingualGlandCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-2.19789f, -12.4169f, 6.83261f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //LeftSubmandibularSublingualGland
            splitDefinition = createSplitDefinition("LeftSubmandibularSublingualGland", "LeftSubmandibularSublingualGland.mesh", "LeftSubmandibularSublingualGlandCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(2.19789f, -12.4169f, 6.83261f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualFlatPlane
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualFlatPlane", "MandibularFullCoverageDualFlatPlane.mesh", "MandibularFullCoverageDualFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.2264f, 8.31641f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualBall
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBall", "MandibularFullCoverageDualBall.mesh", "MandibularFullCoverageDualBallCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualBallTripod
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBallTripod", "MandibularFullCoverageDualBallTripod.mesh", "MandibularFullCoverageDualBallTripodCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualBallColission
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBallColission", "MandibularFullCoverageDualBallColission.mesh", "MandibularFullCoverageDualBallColissionCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualBallCollision
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBallCollision", "MandibularFullCoverageDualBallCollision.mesh", "MandibularFullCoverageDualBallCollisionCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageDualFlatPlaneCollision
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualFlatPlaneCollision", "MandibularFullCoverageDualFlatPlaneCollision.mesh", "MandibularFullCoverageDualFlatPlaneCollisionCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.2264f, 8.31641f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //PivotAppliance
            splitDefinition = createSplitDefinition("PivotAppliance", "PivotAppliance.mesh", "PivotApplianceCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1558f, 7.77287f), new Quaternion(2.980228E-08f, -3.574548E-08f, 3.574548E-08f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MandibularFullCoverageFlatPlane
            splitDefinition = createSplitDefinition("MandibularFullCoverageFlatPlane", "MandibularFullCoverageFlatPlane.mesh", "MandibularFullCoverageFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.1154f, 8.15084f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverageFlatPlane
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageFlatPlane", "MaxillaryFullCoverageFlatPlane.mesh", "MaxillaryFullCoverageFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-2.3913E-07f, -9.86613f, 8.14113f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverage
            splitDefinition = createSplitDefinition("MaxillaryFullCoverage", "MaxillaryFullCoverage.mesh", "MaxillaryFullCoverageCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.65886f, 8.20467f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverageDualShelf
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualShelf", "MaxillaryFullCoverageDualShelf.mesh", "MaxillaryFullCoverageDualShelfCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.70508f, 8.18003f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //NTI
            splitDefinition = createSplitDefinition("NTI", "NTI.mesh", "NTICollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.85379f, 10.9718f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverageDualShelfTripod
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualShelfTripod", "MaxillaryFullCoverageDualShelfTripod.mesh", "MaxillaryFullCoverageDualShelfTripodCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.70508f, 8.18003f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //AnteriorDeprogrammer
            splitDefinition = createSplitDefinition("AnteriorDeprogrammer", "AnteriorDeprogrammer.mesh", "AnteriorDeprogrammerCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.0156f, 9.91062f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverageEquilibrated
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageEquilibrated", "MaxillaryFullCoverageEquilibrated.mesh", "MaxillaryFullCoverageEquilibratedCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.80841f, 8.19537f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            
            //MaxillaryFullCoverageDualFlatPlane
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualFlatPlane", "MaxillaryFullCoverageDualFlatPlane.mesh", "MaxillaryFullCoverageDualFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(2.37629E-07f, -9.69486f, 8.11088f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
            


            //splitDefinition = createSplitDefinition("NTI", "NTI.mesh", "NTICollision", TOP_TEETH_SPLINT_FILTER, 0.0f, Vector3.Zero, Quaternion.Identity);
            //propFactory.addDefinition(splitDefinition.Name, splitDefinition);

            //splitDefinition = createSplitDefinition("PivotAppliance", "PivotAppliance.mesh", "PivotApplianceCollision", BOTTOM_TEETH_SPLINT_FILTER, 1.0f, Vector3.Zero, Quaternion.Identity);
            //jointToMandible(splitDefinition);
            //propFactory.addDefinition(splitDefinition.Name, splitDefinition);
        }

        private static GenericSimObjectDefinition createSplitDefinition(String definitionName, String meshName, String collisionName, short mask, float mass, Vector3 startTranslation, Quaternion startRotation)
        {
            GenericSimObjectDefinition splint = new GenericSimObjectDefinition(definitionName);
            splint.Translation = startTranslation;
            splint.Rotation = startRotation;
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

        public static void dumpPositionsToLog(String file, String mandiblePositionName)
        {
            PositionCollection positions = new PositionCollection();
            try
            {
                using (XmlTextReader textReader = new XmlTextReader(file))
                {
                    positions.loadPositions(textReader);
                }
                Position mandiblePosition = positions.getPosition(mandiblePositionName);
                Log.ImportantInfo("//-------Maxillary------");
                foreach (Position position in positions.Positions)
                {
                    if (position != mandiblePosition)
                    {
                        Log.ImportantInfo("//{0}", position.Name);
                        Log.ImportantInfo("splitDefinition = createSplitDefinition(\"{0}\", \"{0}.mesh\", \"{0}Collision\", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3({1}), new Quaternion({2}));", position.Name, String.Format("{0}f, {1}f, {2}f", position.Translation.x, position.Translation.y, position.Translation.z), String.Format("{0}f, {1}f, {2}f, {3}f", position.Rotation.x, position.Rotation.y, position.Rotation.z, position.Rotation.w));
                        Log.ImportantInfo("propFactory.addDefinition(splitDefinition.Name, splitDefinition);");
                        Log.ImportantInfo("");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not load positions file {0} because:\n{1}", file, e.Message);
            }
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
