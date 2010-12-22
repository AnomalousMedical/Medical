using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;

namespace Medical
{
    class PointingHand
    {
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
            propFactory.addDefinition("PointingHandLeft", leftPointingHand);

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
            propFactory.addDefinition("PointingHandRight", rightPointingHand);
        }
    }
}
