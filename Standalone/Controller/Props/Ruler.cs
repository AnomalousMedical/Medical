using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class Ruler
    {
        public const String DefinitionName = "Ruler";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition rulerSimObject = new GenericSimObjectDefinition(DefinitionName);
            rulerSimObject.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Ruler.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rulerSimObject.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            rulerSimObject.addElement(propFadeBehaviorDef);
            PropDefinition propDefinition = new PropDefinition(rulerSimObject);

            ShowPropTrackInfo rulerData = propDefinition.TrackInfo;
            rulerData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            rulerData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            propFactory.addDefinition(propDefinition);
        }
    }
}
