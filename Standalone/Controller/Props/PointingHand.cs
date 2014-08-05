using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    public class PointingHand
    {
        public const String LeftHandName = "PointingHandLeft";
        public const String RightHandName = "PointingHandRight";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition leftPointingHand = new GenericSimObjectDefinition(LeftHandName);
            leftPointingHand.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "PointerLeftHand.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            leftPointingHand.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            leftPointingHand.addElement(propFadeBehaviorDef);
            PropDefinition propDefinition = new PropDefinition(leftPointingHand);

            //PointingHandLeft
            ShowPropTrackInfo pointingHandLeftData = propDefinition.TrackInfo;
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            propFactory.addDefinition(propDefinition);

            GenericSimObjectDefinition rightPointingHand = new GenericSimObjectDefinition(RightHandName);
            rightPointingHand.Enabled = true;
            entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "PointerRightHand.mesh";
            nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rightPointingHand.addElement(nodeDefinition);
            propFadeBehavior = new PropFadeBehavior();
            propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            rightPointingHand.addElement(propFadeBehaviorDef);
            propDefinition = new PropDefinition(rightPointingHand);

            //PointingHandRight
            ShowPropTrackInfo pointingRightHandData = propDefinition.TrackInfo;
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            propFactory.addDefinition(propDefinition);
        }
    }
}
