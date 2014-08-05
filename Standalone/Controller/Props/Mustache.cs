using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    public class Mustache
    {
        public const String DefinitionName = "Mustache";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition mustache = new GenericSimObjectDefinition(DefinitionName);
            mustache.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Mustache.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            mustache.addElement(nodeDefinition);
            propFactory.addDefinition(new PropDefinition(mustache));
        }
    }
}
