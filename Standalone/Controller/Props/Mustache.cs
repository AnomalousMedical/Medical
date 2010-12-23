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
            GenericSimObjectDefinition mustache = new GenericSimObjectDefinition("Mustache");
            mustache.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Mustache.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            mustache.addElement(nodeDefinition);
            propFactory.addDefinition("Mustache", mustache);
        }
    }
}
