using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class Pen
    {
        public const String DefinitionName = "Pen";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition pen = new GenericSimObjectDefinition(DefinitionName);
            pen.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Pen.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            pen.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            pen.addElement(propFadeBehaviorDef);

            propFactory.addDefinition(DefinitionName, pen);
        }
    }
}
