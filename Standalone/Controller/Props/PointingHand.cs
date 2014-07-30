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
            GenericSimObjectDefinition leftPointingHand = new GenericSimObjectDefinition("PointingHandLeft");
            leftPointingHand.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "PointerLeftHand.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            leftPointingHand.addElement(nodeDefinition);
            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            leftPointingHand.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(LeftHandName, leftPointingHand);

            //PointingHandLeft
            ShowPropTrackInfo pointingHandLeftData = new ShowPropTrackInfo();
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            pointingHandLeftData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            propFactory.addTrackInfo(PointingHand.LeftHandName, pointingHandLeftData);

            GenericSimObjectDefinition rightPointingHand = new GenericSimObjectDefinition("PointingHandRight");
            rightPointingHand.Enabled = true;
            entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "PointerRightHand.mesh";
            nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rightPointingHand.addElement(nodeDefinition);
            propFadeBehavior = new PropFadeBehavior();
            propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            rightPointingHand.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(RightHandName, rightPointingHand);

            //PointingHandRight
            ShowPropTrackInfo pointingRightHandData = new ShowPropTrackInfo();
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(MovePropAction), "Move"));
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(SetPropTransparencyAction), "Set Transparency"));
            pointingRightHandData.addTrack(new ShowPropSubActionPrototype(typeof(DetachableFollowerToggleAction), "Attach To Object"));
            propFactory.addTrackInfo(PointingHand.RightHandName, pointingRightHandData);
        }
    }
}
