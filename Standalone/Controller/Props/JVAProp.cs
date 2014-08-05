using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    class JVAProp
    {
        public const String DefinitionName = "JVAProp";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition doppler = new GenericSimObjectDefinition(DefinitionName);
            doppler.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "JVAProp.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            doppler.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            doppler.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(new PropDefinition(doppler));
        }
    }
}
