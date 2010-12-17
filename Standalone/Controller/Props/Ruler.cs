using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class Ruler
    {
        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition rulerSimObject = new GenericSimObjectDefinition("RulerPrototype");
            rulerSimObject.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(Arrow.EntityName);
            entityDefinition.MeshName = "Ruler.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(Arrow.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rulerSimObject.addElement(nodeDefinition);
            propFactory.addDefinition("Ruler", rulerSimObject);
        }
    }
}
