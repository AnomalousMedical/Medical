using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class Mustache
    {
        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition leftPointingHand = new GenericSimObjectDefinition("Mustache");
            leftPointingHand.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(Arrow.EntityName);
            entityDefinition.MeshName = "Mustache.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(Arrow.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            leftPointingHand.addElement(nodeDefinition);
            propFactory.addDefinition("Mustache", leftPointingHand);
        }
    }
}
