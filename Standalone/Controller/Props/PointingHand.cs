using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class PointingHand
    {
        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition leftPointingHand = new GenericSimObjectDefinition("PointingHandLeft");
            leftPointingHand.Enabled = true;
            EntityDefinition entityDefinition = new EntityDefinition(Arrow.EntityName);
            entityDefinition.MeshName = "PointerLeftHand.mesh";
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(Arrow.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            leftPointingHand.addElement(nodeDefinition);
            propFactory.addDefinition("PointingHandLeft", leftPointingHand);

            GenericSimObjectDefinition rightPointingHand = new GenericSimObjectDefinition("PointingHandRight");
            rightPointingHand.Enabled = true;
            entityDefinition = new EntityDefinition(Arrow.EntityName);
            entityDefinition.MeshName = "PointerRightHand.mesh";
            nodeDefinition = new SceneNodeDefinition(Arrow.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            rightPointingHand.addElement(nodeDefinition);
            propFactory.addDefinition("PointingHandRight", rightPointingHand);
        }
    }
}
