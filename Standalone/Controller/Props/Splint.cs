using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using BulletPlugin;

namespace Medical
{
    class Splint
    {
        private const short TOP_TEETH_SPLINT_FILTER = ~2;
        private const short BOTTOM_TEETH_SPLINT_FILTER = ~4;

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition splitDefinition;

            splitDefinition = createSplitDefinition("NTI", "NTI.mesh", "NTICollision", TOP_TEETH_SPLINT_FILTER);
            propFactory.addDefinition(splitDefinition.Name, splitDefinition);
        }

        private static GenericSimObjectDefinition createSplitDefinition(String definitionName, String meshName, String collisionName, short mask)
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
            rigidBody.CollisionFilterMask = mask;
            splint.addElement(rigidBody);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            splint.addElement(propFadeBehaviorDef);

            return splint;
        }
    }
}
