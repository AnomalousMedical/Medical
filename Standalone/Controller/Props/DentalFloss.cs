using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class DentalFloss
    {
        public const String DefinitionName = "DentalFloss";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition dentalFloss = new GenericSimObjectDefinition(DefinitionName);
            dentalFloss.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "DentalFloss.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            dentalFloss.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            dentalFloss.addElement(propFadeBehaviorDef);

            propFactory.addDefinition(new PropDefinition(dentalFloss)
            {
                BrowserPath = "Tools",
                PrettyName = "Dental Floss"
            });
        }
    }
}
