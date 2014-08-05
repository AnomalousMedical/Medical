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
    class SplintDefiner
    {
        private const short TOP_TEETH_SPLINT_FILTER = ~2;
        private const short BOTTOM_TEETH_SPLINT_FILTER = ~4;

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition splitDefinition;

            //-------Maxillary------
            //MaxillaryFullCoverageFlatPlane
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageFlatPlane", "MaxillaryFullCoverageFlatPlane.mesh", "MaxillaryFullCoverageFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(-2.3913E-07f, -9.86613f, 8.14113f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //MaxillaryFullCoverage
            splitDefinition = createSplitDefinition("MaxillaryFullCoverage", "MaxillaryFullCoverage.mesh", "MaxillaryFullCoverageCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.65886f, 8.20467f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //NTI
            splitDefinition = createSplitDefinition("NTI", "NTI.mesh", "NTICollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.85379f, 10.9718f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //MaxillaryFullCoverageDualShelfTripod
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualShelfTripod", "MaxillaryFullCoverageDualShelfTripod.mesh", "MaxillaryFullCoverageDualShelfTripodCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.70508f, 8.18003f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //AnteriorDeprogrammer
            splitDefinition = createSplitDefinition("AnteriorDeprogrammer", "AnteriorDeprogrammer.mesh", "AnteriorDeprogrammerCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -10.0156f, 9.91062f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //MaxillaryFullCoverageEquilibrated
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageEquilibrated", "MaxillaryFullCoverageEquilibrated.mesh", "MaxillaryFullCoverageEquilibratedCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.80841f, 8.19537f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //MaxillaryFullCoverageDualFlatPlane
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualFlatPlane", "MaxillaryFullCoverageDualFlatPlane.mesh", "MaxillaryFullCoverageDualFlatPlaneCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(2.37629E-07f, -9.69486f, 8.11088f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //MaxillaryFullCoverageDualShelf
            splitDefinition = createSplitDefinition("MaxillaryFullCoverageDualShelf", "MaxillaryFullCoverageDualShelf.mesh", "MaxillaryFullCoverageDualShelfCollision", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3(0f, -9.76966f, 8.35717f), new Quaternion(0f, 0f, 0f, 1f));
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Maxillary"
            });

            //-------Mandibular------
            //MandibularFullCoverage
            splitDefinition = createSplitDefinition("MandibularFullCoverage", "MandibularFullCoverage.mesh", "MandibularFullCoverageCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(-2.14577E-06f, -10.1296f, 8.09898f), new Quaternion(1.490118E-08f, -4.511825E-08f, 6.723153E-16f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //MandibularFullCoverageDualFlatPlane
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualFlatPlane", "MandibularFullCoverageDualFlatPlane.mesh", "MandibularFullCoverageDualFlatPlaneCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.2264f, 8.31641f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //MandibularFullCoverageDualBall
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBall", "MandibularFullCoverageDualBall.mesh", "MandibularFullCoverageDualBallCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //MandibularFullCoverageDualBallTripod
            splitDefinition = createSplitDefinition("MandibularFullCoverageDualBallTripod", "MandibularFullCoverageDualBallTripod.mesh", "MandibularFullCoverageDualBallTripodCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.1498f, 8.11417f), new Quaternion(2.980228E-08f, 0f, 0f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //PivotAppliance
            splitDefinition = createSplitDefinition("PivotAppliance", "PivotAppliance.mesh", "PivotApplianceCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.1558f, 7.77287f), new Quaternion(2.980228E-08f, -3.574548E-08f, 3.574548E-08f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //PartialPosteriorSplint
            splitDefinition = createSplitDefinition("PartialPosteriorSplint", "PartialPosteriorSplint.mesh", "PivotApplianceCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.1558f, 7.77287f), new Quaternion(2.980228E-08f, -3.574548E-08f, 3.574548E-08f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });

            //MandibularFullCoverageFlatPlane
            splitDefinition = createSplitDefinition("MandibularFullCoverageFlatPlane", "MandibularFullCoverageFlatPlane.mesh", "MandibularFullCoverageFlatPlaneCollision", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3(0f, -10.1154f, 8.15084f), new Quaternion(0f, 0f, 0f, 1f));
            jointToMandible(splitDefinition);
            propFactory.addDefinition(new PropDefinition(splitDefinition)
            {
                BrowserPath = "Splints/Mandibular"
            });
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

            RigidBodyDefinition rigidBody = new RigidBodyDefinition(Splint.RigidBodyName);
            rigidBody.ShapeName = collisionName;
            rigidBody.Mass = mass;
            rigidBody.CollisionFilterMask = mask;
            splint.addElement(rigidBody);

            Splint splintBehavior = new Splint();
            splintBehavior.WorldStartTranslation = startTranslation;
            splintBehavior.WorldStartRotation = startRotation;
            BehaviorDefinition splintBehaviorDef = new BehaviorDefinition(Splint.SplintBehaviorName, splintBehavior);
            splint.addElement(splintBehaviorDef);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            splint.addElement(propFadeBehaviorDef);

            return splint;
        }

        public static GenericSimObjectDefinition jointToMandible(GenericSimObjectDefinition splint)
        {
            Generic6DofConstraintDefinition joint = new Generic6DofConstraintDefinition(Splint.JointName);
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

        public static void dumpPositionsToLog(String file)
        {
            PositionCollection positions = new PositionCollection();
            try
            {
                using (XmlTextReader textReader = new XmlTextReader(file))
                {
                    positions.loadPositions(textReader);
                }
                Log.ImportantInfo("//-------Maxillary------");
                foreach (Position position in positions.Positions)
                {
                    Log.ImportantInfo("//{0}", position.Name);
                    Log.ImportantInfo("splitDefinition = createSplitDefinition(\"{0}\", \"{0}.mesh\", \"{0}Collision\", TOP_TEETH_SPLINT_FILTER, 0.0f, new Vector3({1}), new Quaternion({2}));", position.Name, String.Format("{0}f, {1}f, {2}f", position.Translation.x, position.Translation.y, position.Translation.z), String.Format("{0}f, {1}f, {2}f, {3}f", position.Rotation.x, position.Rotation.y, position.Rotation.z, position.Rotation.w));
                    Log.ImportantInfo("propFactory.addDefinition(splitDefinition.Name, splitDefinition);");
                    Log.ImportantInfo("");
                }
                Log.ImportantInfo("//-------Mandibular------");
                foreach (Position position in positions.Positions)
                {
                    Log.ImportantInfo("//{0}", position.Name);
                    Log.ImportantInfo("splitDefinition = createSplitDefinition(\"{0}\", \"{0}.mesh\", \"{0}Collision\", BOTTOM_TEETH_SPLINT_FILTER, 5.0f, new Vector3({1}), new Quaternion({2}));", position.Name, String.Format("{0}f, {1}f, {2}f", position.Translation.x, position.Translation.y, position.Translation.z), String.Format("{0}f, {1}f, {2}f, {3}f", position.Rotation.x, position.Rotation.y, position.Rotation.z, position.Rotation.w));
                    Log.ImportantInfo("jointToMandible(splitDefinition);");
                    Log.ImportantInfo("propFactory.addDefinition(splitDefinition.Name, splitDefinition);");
                    Log.ImportantInfo("");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not load positions file {0} because:\n{1}", file, e.Message);
            }
        }
    }
}
