using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    class Doppler
    {
        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition doppler = new GenericSimObjectDefinition("Doppler");
            doppler.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "DopplerProp.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            doppler.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            doppler.addElement(propFadeBehaviorDef);
            propFactory.addDefinition("Doppler", doppler);
        }
    }
}
