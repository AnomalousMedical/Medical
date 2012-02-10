using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    class Plane : Interface
    {
        public const String DefinitionName = "Plane";
        public const String BehaviorName = "Behavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition simObject = new GenericSimObjectDefinition("PlanePrototype");
            simObject.Enabled = true;
            ManualObjectDefinition manualObjectDefinition = new ManualObjectDefinition(PropFactory.ManualObjectName);
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(manualObjectDefinition);
            simObject.addElement(nodeDefinition);
            Plane behavior = new Plane();
            BehaviorDefinition behaviorDef = new BehaviorDefinition(BehaviorName, behavior);
            simObject.addElement(behaviorDef);
            PropFadeBehavior propFadeBehavior = new ManualObjectPropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            simObject.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(DefinitionName, simObject);
        }

        private Size2 size;
        private ManualObject manualObject;
        private Color color = Color.Red;

        public Plane()
        {
            size = new Size2(1.0f, 1.0f);
        }

        protected override void constructed()
        {
            base.constructed();
            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find scene node {0} in CircularHighlight.", PropFactory.NodeName);
            }
            manualObject = sceneNode.getNodeObject("ManualObject") as ManualObject;
            if (manualObject == null)
            {
                blacklist("Cannot find 'ManualObject' in node {0}.", PropFactory.NodeName);
            }
            createObject();
        }

        public void createObject()
        {
            manualObject.clear();
            manualObject.begin("ManualTwoSidedPropShapeMaterial", OperationType.OT_TRIANGLE_STRIP);
            uint vertexCount = (uint)(4);
            uint indexCount = vertexCount + 2;
            manualObject.estimateVertexCount(vertexCount);
            manualObject.estimateIndexCount(indexCount);
            //0       1
            //
            //2       3
            //
            float widthHalfExtent = size.Width / 2.0f;
            float heightHalfExtent = size.Height / 2.0f;
            //Upper Left (0)
            manualObject.position(new Vector3(-widthHalfExtent, heightHalfExtent, 0f));
            manualObject.color(color.r, color.g, color.b, color.a);
            //Upper Right (1)
            manualObject.position(new Vector3(widthHalfExtent, heightHalfExtent, 0f));
            manualObject.color(color.r, color.g, color.b, color.a);
            //Lower Left (2)
            manualObject.position(new Vector3(-widthHalfExtent, -heightHalfExtent, 0f));
            manualObject.color(color.r, color.g, color.b, color.a);
            //Lower Right (3)
            manualObject.position(new Vector3(widthHalfExtent, -heightHalfExtent, 0f));
            manualObject.color(color.r, color.g, color.b, color.a);

            //First Triangle
            manualObject.index(1);
            manualObject.index(0);
            manualObject.index(3);
            //Second Triangle
            manualObject.index(3);
            manualObject.index(2);
            manualObject.index(0);

            manualObject.end();
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
    }
}
