using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class Doppler
    {
        public const String DefinitionName = "Doppler";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition doppler = new GenericSimObjectDefinition(DefinitionName);
            doppler.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "DopplerWand.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            doppler.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            doppler.addElement(propFadeBehaviorDef);
            PropDefinition propDefinition = new PropDefinition(doppler)
            {
                BrowserPath = "Tools"
            };

            ShowPropTrackInfo dopplerData = propDefinition.TrackInfo;
            dopplerData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            dopplerData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            propFactory.addDefinition(propDefinition);
        }
    }
}
